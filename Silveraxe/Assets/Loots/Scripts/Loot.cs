using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;
using static InteractableObject.Action;

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

	static float AnimationTime = 0.1f;

	public string ItemName;
	public GameObject prefab;
	public Sprite ItemSprite;
	public string Description;
	public bool animate = true;
	public bool dropable = true;
	public LootCategory lootCategory;

	[Header("---")]
	public GameObject lootTemplate;
	public static InventoryUI inventoryUI;
	public InventoryEntry entry { get; set; } = null;                             // L'entrée d'inventaire lorsque l'objet a été ramassé

	public override bool IsInteractable() {                         // l'objet est intéractif si
		return !animate || m_AnimationTimer >= AnimationTime;       // l'animation de mise en place est terminée ou désactivée
	}

	Vector3 m_OriginalPosition;
	Vector3 m_TargetPoint;
	float m_AnimationTimer = 0.0f;

	void Awake() {
		m_OriginalPosition = transform.position;                    // préparation
		m_TargetPoint = transform.position;                         // de l'animation
		m_AnimationTimer = AnimationTime - 0.1f;                    // de mise en place
	}

	protected override void Start() {
		base.Start();
		inventoryUI = InventoryUI.Instance;

		var obj = GetComponentInChildren<MeshFilter>().gameObject;
		obj.AddComponent<MeshCollider>();
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
	}

	/// <summary>
	/// Ramasser / déposer un objet
	/// </summary>
	/// <param name="character">le personnage (joueur, PNJ, ...)</param>
	/// <param name="target">le lieu (lorsqu'on pose un objet)</param>
	/// <param name="action">l'action : prendre ou poser</param>
	public override void InteractWith(CharacterData character, HighlightableObject target = null, Action action = take) {
		base.InteractWith(character, target, action);

		if (action == take) {
			// si on ramasse l'objet
			SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });
			InventoryManager.Instance.AddItem(this);
			Destroy(gameObject);
		} else
			// si on dépose l'objet sur une cible
			if (action == drop && target is Target) {
			if ((target as Target).isAvailable(this)) {                // et que cet emplacement est disponible pour cet objet
				inventoryUI.DropItem(target as Target, entry);         // déposer l'objet d'inventaire
			}
		}
	}

	void CreateWorldRepresentation() {
		var pos = transform.position + Vector3.up * prefab.gameObject.transform.localScale.y / 2;
		// if the item have a world object prefab set use that...
		if (prefab != null) {
			var obj = Instantiate(prefab, pos, new Quaternion());
			obj.transform.parent = transform;
			obj.layer = LayerMask.NameToLayer("Interactable");
		}
	}
}
