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
		if (item != null) return false;                                                                     // ne peut contenir qu'un seul objet d'inventaire
		if (!playerManager.characterData.inventory.HasCompatibleItem(this)) return false;                   // l'inventaire doit contenir un objet compatible		
		return true;
	}

	public override bool IsInteractable() {
		if (!isHighlightable) return false;
		if (inventoryUI.selectedEntry == null) return false;                                                        // un objet d'inventaire doit être sélectionné	
		if (!CompatibleWith(inventoryUI.selectedEntry.item)) return false;                                          // l'objet sélectionné doit être compatible

		//var selectedItem = inventoryUI.selectedEntry.item;
		//if (!selectedItem.dropable) return false;																	// l'objet sélectionné doit être déposable
		//if (filterMode == FilterMode.allow && !filterItems.Contains(selectedItem.lootCategory)) return false;		// si on est en mode 'autorise' => il doit être autorisé sur cette cible
		//if (filterMode == FilterMode.refuse && filterItems.Contains(selectedItem.lootCategory)) return false;		// si on est en mode 'refuse'   => il ne doit pas être interdit sur cette cible
		return base.IsInteractable();
	}

	public bool CompatibleWith(Loot testItem) {
		if (!testItem.dropable) return false;                                                                   // l'objet sélectionné doit être déposable
		if (filterMode == FilterMode.allow && !filterItems.Contains(testItem.lootCategory)) return false;       // si on est en mode 'autorise' => il doit être autorisé sur cette cible
		if (filterMode == FilterMode.refuse && filterItems.Contains(testItem.lootCategory)) return false;       // si on est en mode 'refuse'   => il ne doit pas être interdit sur cette cible
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