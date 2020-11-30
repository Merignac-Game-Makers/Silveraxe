using System.Collections.Generic;
using UnityEngine;

using static App;


/// <summary>
/// Objet intéractible sur lequel on peut déposer un objet d'inventaire (loot)
/// </summary>
public class Target : InteractableObject
{
	public enum FilterMode { allow, refuse }

	public GameObject prefab;

	public FilterMode filterMode = FilterMode.allow;
	public List<LootCategory> filterItems;

	Transform prefabHolder;
	Transform target;
	public Loot item { get; set; }

	public override bool IsHighlightable() {
		if (this.item != null) return false;     // ne peut contenir qu'un seul objet d'inventaire
		if (inventoryUI.selectedEntry == null) return false;
		var item = inventoryUI.selectedEntry.item;
		if (!item.dropable) return false;
		if (filterMode == FilterMode.allow && !filterItems.Contains(item.lootCategory)) return false;
		if (filterMode == FilterMode.refuse && filterItems.Contains(item.lootCategory)) return false;
		return true;
	}

	public Vector3 targetPos => target.position;

	protected override void Start() {
		base.Start();
		// récupération des objets clés
		target = transform.Find("Target");
		prefabHolder = transform.Find("PrefabHolder");
		// masquage de la cible
		target.gameObject.SetActive(false);
		// ajout du navmesh obstacle
		if (prefabHolder) {
			var obj = prefabHolder.GetComponentInChildren<MeshFilter>();
			if (obj != null)
				obj.gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();
		}
	}

	private void Update() {
		if (!IsPointerOverUIElement() && Input.GetButtonDown("Fire1")) {
			if (!interactableObjectsManager.MultipleSelection() || isMouseOver) {
				Act();
			}
		}
	}

	void Act() {
		if (IsInteractable()) {
			Highlight(false);
			item = inventoryUI.selectedEntry.item;
			inventoryUI.selectedEntry?.Select(false);
			item.Drop(this);
		}
	}
}