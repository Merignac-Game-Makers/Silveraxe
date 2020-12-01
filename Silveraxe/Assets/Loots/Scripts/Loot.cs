using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using static InteractableObject.Action;
using static App;
using UnityEngine.UI;

/// <summary>
/// Describes an InteractableObject that can be picked up and grants a specific item when interacted with.
///
/// It will also play a small animation (object going in an arc from spawn point to a random point around) when the
/// object is actually "spawned", and the object becomes interactable only when that animation is finished.
///
/// Finally it will notify the LootUI that a new loot is available in the world so the UI displays the name.
/// </summary>
public class Loot : InteractableObject
{
	protected static float AnimationTime = 0.1f;

	public string ItemName;
	public GameObject prefab;
	public Sprite ItemSprite;
	public string Description;
	public bool animate = true;
	public bool dropable = true;
	public LootCategory lootCategory;
	public bool usable = false;
	public List<UsageEffect> UsageEffects;


	public Entry entry { get; set; } = null;                             // L'entrée d'inventaire lorsque l'objet a été ramassé

	public override bool IsInteractable() {                         // l'objet est intéractif si
		return (!animate || m_AnimationTimer >= AnimationTime && isInPlayerCollider);       // l'animation de mise en place est terminée ou désactivée et le joueur est proche     && IsPlayerNear(5f)
	}

	protected Vector3 m_OriginalPosition;
	protected Vector3 m_TargetPoint;
	protected float m_AnimationTimer = 0.0f;

	protected Target target;

	//public Loot(Loot item) {
	//	ItemName = item.ItemName;
	//	prefab = item.prefab;
	//	ItemSprite = item.ItemSprite;
	//	Description = item.Description;
	//	animate = item.animate;
	//	dropable = item.dropable;
	//	lootCategory = item.lootCategory;
	//	usable = item.usable;
	//	UsageEffects = item.UsageEffects;
	//}

	void Awake() {
		StartAnimation();
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
		if (!IsPointerOverUIElement() && Input.GetButtonDown("Fire1")) {
			if (!interactableObjectsManager.MultipleSelection() || isMouseOver)
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
		playerManager.StopAgent();
		if (!playerManager.characterData.inventory.isFull) {
			playerManager.characterData.inventory.AddItem(this);
			isInPlayerCollider = false;
			SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });
			targetsManager.OnTake();
			if (target) {
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
	public void Drop(Target target) {
		this.target = target;
		animate = true;
		transform.position = target.targetPos;
		StartAnimation();
		playerManager.characterData.inventory.RemoveItem(entry as InventoryEntry);       // retirer l'objet déposé de l'inventaire
	}
}