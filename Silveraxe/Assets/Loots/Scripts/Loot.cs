using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using UnityEngine.UI;
using System.Reflection;
using UnityEngine.EventSystems;

/// <summary>
/// Describes an InteractableObject that can be picked up and grants a specific item when interacted with.
///
/// It will also play a small animation (object going in an arc from spawn point to a random point around) when the
/// object is actually "spawned", and the object becomes interactable only when that animation is finished.
///
/// Finally it will notify the LootUI that a new loot is available in the world so the UI displays the name.
/// </summary>
public class Loot : InteractableObject {
	protected static float AnimationTime = 0.1f;
	protected bool isAnimationRunning => itemBase.animate && m_AnimationTimer < AnimationTime;

	public ItemBase itemBase;

	public Target target { get; set; }                                              // la cible sur laquelle l'objet est posé

	public Entry entry { get; set; } = null;                                        // L'entrée d'inventaire lorsque l'objet a été ramassé

	public override bool IsInteractable() {                                         // l'objet est intéractif si
		if (isAnimationRunning) return false;										// l'animation de mise en place est terminée
		return base.IsInteractable();                                               //  
	}

	Vector3 m_OriginalPosition;
	Vector3 m_TargetPoint;
	float m_AnimationTimer = 0.0f;

	protected override void Start() {
		base.Start();
		if (isAnimationRunning)
			StartAnimation();
	}

	private void OnEnable() {
		//itemBase.animate = false;
	}

	protected virtual void Update() {
		// bouton d'action
		if (Input.GetButtonDown("Fire1") && !App.uiManager.isClicOnUI) {
			Act();
		}
	}

	void Act() {
		if (IsInteractable())
			Take();
	}

	public bool Equals(Loot other) {
		return itemBase == other.itemBase;
		//return GetComponentInChildren<MeshFilter>().mesh.name == other.GetComponentInChildren<MeshFilter>().mesh.name;
		//return itemBase.prefab == other.itemBase.prefab;
	}

	public void StartAnimation() {
		StartCoroutine(CreateAnimation());
	}

	IEnumerator CreateAnimation() {
		float ratio;
		m_OriginalPosition = transform.position + transform.up;     // préparation de l'animation
		m_TargetPoint = transform.position;                         // de l'animation
		m_AnimationTimer = AnimationTime - 0.1f;                    // de mise en place
		while (itemBase.animate && m_AnimationTimer < AnimationTime) {
			m_AnimationTimer += Time.deltaTime * Time.timeScale;
			ratio = Mathf.Clamp01(m_AnimationTimer / AnimationTime);
			Vector3 currentPos = Vector3.Lerp(m_OriginalPosition, m_TargetPoint, ratio);
			currentPos.y += Mathf.Sin(ratio * Mathf.PI) ;
			transform.position = currentPos;
			yield return null;
		}
		Highlight(isInPlayerCollider);
	}

	protected virtual void Take() {
		EventSystem.current.SetSelectedGameObject(null);		// éviter les actions multiples en 1 seul clic
		// on ramasse l'objet
		if (!isAnimationRunning && !App.playerManager.characterData.inventory.isFull) {                                                                // si l'inventaire n'est pas plein
			App.playerManager.characterData.inventory.AddItem(this);                                                            //		ajouter l'objet à l'inventaire
			isInPlayerCollider = false;                                                                                     //		l'objet n'est plus dans le collider du joueur (=> non intéractible)
			SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });      //		son
			App.targetsManager.OnTake();                                                                                        //		extinction de toutes les cibles
			if (target) {                                                                                                   //		mise à jour de la cible (s'il était sur une cible)
				target.item = null;
				target = null;
			}
			OnTake();
		}
	}

	protected virtual void OnTake() { }


	/// <summary>
	/// Déposer un objet d'inventaire
	/// </summary>
	/// <param name="target">le lieu</param>
	/// <param name="entry">l'entrée d'inventaire </param>
	public virtual void Drop(Target target) {
		EventSystem.current.SetSelectedGameObject(null);        // éviter les actions multiples en 1 seul clic
		this.target = target;
		itemBase.animate = true;
		transform.position = target.targetPos;
		StartAnimation();
		App.playerManager.characterData.inventory.RemoveItem(entry as InventoryEntry);       // retirer l'objet déposé de l'inventaire
		OnDrop(target);
	}

	protected virtual void OnDrop(Target target) { }



	#region sauvegarde
	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	public override SSavable Serialize() {
		var result = new SLoot().Copy(base.Serialize());
		result.itemBase = itemBase.name;
		if (target && target.guid != null)
			result.target = ((System.Guid)target.guid).ToByteArray();
		else
			result.target = System.Guid.Empty.ToByteArray();
		return result;
	}

	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		base.Deserialize(serialized);
		SLoot sLoot = (SLoot)serialized;
		//itemBase = App.itemsManager.GetItemBase(sLoot.itemBase);
		if (new System.Guid(sLoot.target) != System.Guid.Empty)
			target = Game.Find<Target>(sLoot.target);
	}

	#endregion
}

/// <summary>
/// Classe pour la sauvegarde
/// 
/// Pour Loot : l'id de la 'target' sur laquelle il est posé
/// </summary>
[System.Serializable]
public class SLoot : SSavable {
	public byte[] target;
	public string itemBase;
}