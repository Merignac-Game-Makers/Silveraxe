using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using static InteractableObject.Action;
using static App;
using UnityEngine.UI;
using System.Reflection;

/// <summary>
/// Describes an InteractableObject that can be picked up and grants a specific item when interacted with.
///
/// It will also play a small animation (object going in an arc from spawn point to a random point around) when the
/// object is actually "spawned", and the object becomes interactable only when that animation is finished.
///
/// Finally it will notify the LootUI that a new loot is available in the world so the UI displays the name.
/// </summary>
public class Loot : InteractableObject, ISave
{
	protected static float AnimationTime = 0.1f;

	public ItemBase itemBase;

	public string itemName => itemBase.itemName;
	public GameObject prefab => itemBase.prefab;
	public Sprite itemSprite => itemBase.itemSprite;
	public string description => itemBase.description;
	public bool animate => itemBase.animate;
	public bool dropable => itemBase.dropable;
	public LootCategory lootCategory => itemBase.lootCategory;
	public bool usable => itemBase.usable;
	public List<UsageEffect> usageEffects => itemBase.usageEffects;

	public Target target;

	public Entry entry { get; set; } = null;                                        // L'entrée d'inventaire lorsque l'objet a été ramassé

	public override bool IsInteractable() {                                         // l'objet est intéractif si
		if (animate && m_AnimationTimer < AnimationTime) return false;              // l'animation de mise en place est terminée
		return base.IsInteractable();                                               //  
	}

	protected Vector3 m_OriginalPosition;
	protected Vector3 m_TargetPoint;
	protected float m_AnimationTimer = 0.0f;


	void Awake() {
		StartAnimation();
		target = itemBase.target;
	}


	void Update() {
		// animation de mise en place
		if (animate && m_AnimationTimer < AnimationTime) {
			m_AnimationTimer += Time.deltaTime;
			float ratio = Mathf.Clamp01(m_AnimationTimer / AnimationTime);
			Vector3 currentPos = Vector3.Lerp(m_OriginalPosition, m_TargetPoint, ratio);
			currentPos.y = currentPos.y + Mathf.Sin(ratio * Mathf.PI) * 2.0f;
			transform.position = currentPos;
		}

		// bouton d'action
		if (Input.GetButtonDown("Fire1") && !uiManager.isClicOnUI) {
			Act();

		}
	}

	void Act() {
		if (IsInteractable())
			Take();
	}

	public bool Equals(Loot other) {
		return prefab == other.prefab;
	}

	public void StartAnimation() {
		StartCoroutine("CreateAnimation");
	}

	IEnumerator CreateAnimation() {
		float ratio;
		m_OriginalPosition = transform.position;                    // préparation
		m_TargetPoint = transform.position;                         // de l'animation
		m_AnimationTimer = AnimationTime - 0.1f;                    // de mise en place
		while (animate && m_AnimationTimer < AnimationTime) {
			m_AnimationTimer += Time.deltaTime;
			ratio = Mathf.Clamp01(m_AnimationTimer / AnimationTime);
			Vector3 currentPos = Vector3.Lerp(m_OriginalPosition, m_TargetPoint, ratio);
			currentPos.y += Mathf.Sin(ratio * Mathf.PI) * 2.0f;
			transform.position = currentPos;
			yield return new WaitForEndOfFrame();
		}
		Highlight(isInPlayerCollider);
	}

	protected virtual void Take() {
		// on ramasse l'objet
		playerManager.StopAgent();																							// interrompre le déplacement du joueur
		if (!playerManager.characterData.inventory.isFull) {																// si l'inventaire n'est pas plein
			playerManager.characterData.inventory.AddItem(this);															//		ajouter l'objet à l'inventaire
			isInPlayerCollider = false;																						//		l'objet n'est plus dans le collider du joueur (=> non intéractible)
			SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });		//		son
			targetsManager.OnTake();																						//		extinction de toutes les cibles
			if (target) {																									//		mise à jour de la cible (s'il était sur une cible)
				target.item = null;
				target = null;
			}
		}
	}


	/// <summary>
	/// Déposer un objet d'inventaire
	/// </summary>
	/// <param name="target">le lieu</param>
	/// <param name="entry">l'entrée d'inventaire </param>
	public virtual void Drop(Target target) {
		this.target = target;
		itemBase.animate = true;
		transform.position = target.targetPos;
		StartAnimation();
		playerManager.characterData.inventory.RemoveItem(entry as InventoryEntry);       // retirer l'objet déposé de l'inventaire
	}




	#region sauvegarde
	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	public override SInteractable Serialize() {
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
		itemBase = ResourcesManager.GetItemBase(sLoot.itemBase);
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
public class SLoot : SInteractable
{
	public byte[] target;
	public string itemBase;
}