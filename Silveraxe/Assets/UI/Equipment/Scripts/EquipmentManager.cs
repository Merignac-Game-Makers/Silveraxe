using System;
using System.Collections.Generic;
using UnityEngine;

using static App;

/// <summary>
/// This handles the inventory of our character. Each slot can hold one
/// TYPE of object, but those can be stacked without limit (e.g. 1 slot used by health potions, but contains 20
/// health potions)
/// </summary>
[Serializable]
public class EquipmentManager
{

	EquipmentEntry weaponEntry = new EquipmentEntry();
	[System.NonSerialized]
	EquipmentEntry helmetEntry = new EquipmentEntry();
	[System.NonSerialized]
	EquipmentEntry armorEntry = new EquipmentEntry();
	[System.NonSerialized]
	EquipmentEntry shieldEntry = new EquipmentEntry();

	[System.NonSerialized]
	public EquipmentEntry[] entries = new EquipmentEntry[4];

	[System.NonSerialized]
	CharacterData owner;

	public void Init(CharacterData owner) {
		this.owner = owner;
		entries[0] = weaponEntry;
		entries[1] = helmetEntry;
		entries[2] = armorEntry;
		entries[3] = shieldEntry;
	}

	/// <summary>
	/// Ajouter un objet aux entrées d'inventaire
	///		rechercher si une entrée contient déjà un objet identique
	///			si OUI => ajouter 1 à la quantité
	///			si NON => ajouter une entrée
	/// </summary>
	/// <param name="item">l'objet à ajouter</param>
	public void AddItem(Equipment item) {
		EquipmentCategory category = item.equipmentCategory;
		int idx = 0;
		switch (category) {
			case EquipmentCategory.Armor:
				idx = 2;
				break;
			case EquipmentCategory.Helmet:
				idx = 1;
				break;
			case EquipmentCategory.Shield:
				idx = 3;
				break;
			case EquipmentCategory.Weapon:
				idx = 0;
				break;
		}

		entries[idx].Equip(item);
		foreach (EquipmentEntryUI entryUI in equipmentUI.entries) {     // trouver un emplacement d'affichage correspondant (même catégorie)
			if (entryUI.equipmentCategory == item.equipmentCategory) {
				entryUI.Init(entries[idx]);                                 // équiper
				break;
			}
		}

		item.transform.position = new Vector3(0, -50, 0);

		if (IsSetComplete() && GetSetLevel() != null) {
			playerManager.Promote((Equipment.EquipmentLevel)GetSetLevel());
		}
	}

	public void Clear() {
		for (int i=0; i<entries.Length; i++) {
			entries[i].item = null;
			equipmentUI.entries[i].Show(false);
		}
	}

	/// <summary>
	/// This will *try* to use the item. If the item return true when used, this will decrement the stack count and
	/// if the stack count reach 0 this will free the slot. If it return false, it will just ignore that call.
	/// (e.g. a potion will return false if the user is at full health, not consuming the potion in that case)
	/// </summary>
	/// <param name="entry"></param>
	/// <returns></returns>
	public bool UseItem(Entry entry) {
		if (entry is InventoryEntry) {
			//entry = entry as InventoryEntry;
			//if ((entry as InventoryEntry).item.Used(owner)) {							 // si l'objet est utilisable
			//	SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { // jouer le son associé
			//		Clip = (entry as InventoryEntry).item is EquipmentItem ? SFXManager.ItemEquippedSound : SFXManager.ItemUsedSound 
			//	});
			//	(entry as InventoryEntry).count -= 1;                                   // retirer 1 à la quantité
			//	entry.ui.UpdateEntry();													// mettre l'ui à jour
			//	return true;
			//}
		}
		return false;
	}

	public Equipment.EquipmentLevel? GetSetLevel() {
		if (!IsSetComplete())
			return null;
		Equipment.EquipmentLevel? level = entries[0].itemLevel;
		foreach (EquipmentEntry entry in entries) {
			if (entry.itemLevel != level)
				return null;
		}
		return level;
	}

	public bool IsSetComplete() {
		foreach (EquipmentEntry entry in entries) {
			if (entry.item == null)
				return false;
		}
		return true;

	}

	public void RemoveAll() {
		foreach( EquipmentEntry entry in entries) {
			RemoveItem(entry);
		}
	}

	public void RemoveItem(EquipmentEntry entry) {
		entry.item = null;
		entry.ui.UpdateEntry();
	}

	public bool HasCompatibleItem(Target target) {
		foreach (EquipmentEntry entry in entries) {
			if (target.CompatibleWith(entry.item)) return true;
		}
		return false;
	}
}
