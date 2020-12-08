﻿using System.Collections.Generic;
using UnityEngine;

using static App;


/// <summary>
/// Objet intéractible sur lequel on peut déposer un objet d'inventaire (loot)
/// </summary>
public class Target : InteractableObject, ISave
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
		if (!playerManager) return false; // (attente d'initialisation de la scène)
		if (!playerManager.characterData.inventory.HasCompatibleItem(this)) return false;                   // l'inventaire doit contenir un objet compatible		
		return true;
	}

	public override bool IsInteractable() {
		if (!isHighlightable) return false;
		if (inventoryUI.selectedEntry == null) return false;                                                        // un objet d'inventaire doit être sélectionné	
		if (!CompatibleWith(inventoryUI.selectedEntry.loot)) return false;                                          // l'objet sélectionné doit être compatible
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
			//if (!interactableObjectsManager.MultipleSelection() || isMouseOver) {
			Act();
			//}
		}
	}

	void Act() {
		if (IsInteractable()) {
			Highlight(false);
			item = inventoryUI.selectedEntry.loot;
			inventoryUI.selectedEntry?.Select(false);
			item.Drop(this);
		}
	}


	#region sauvegarde
	public void Serialize(List<object> sav) {
		sav.Add(new STarget() {
			guid = guid.ToByteArray(),
			position = transform.position.toArray(),                // position
			rotation = transform.rotation.toArray(),                 // rotation
			target_ItemGuid = guid.ToByteArray()
		});
	}

	public override void Deserialize(object serialized) {
		base.Deserialize(serialized);
		if (serialized is STarget) {
			STarget s = serialized as STarget;
			item = Game.current.allGuidComponents[new System.Guid(s.target_ItemGuid)]?.GetComponent<Loot>();
		}
	}
	#endregion
}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class STarget : SInteractable
{
	public byte[] target_ItemGuid = System.Guid.Empty.ToByteArray();
}