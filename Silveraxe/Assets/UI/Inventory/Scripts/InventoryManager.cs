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
public class InventoryManager {

	private int numSlots = 0;                           // capacité de l'inventaire
	public bool isFull => entries.Count == numSlots;    // l'inventaire est-il complet ?

	public List<InventoryEntry> entries = new List<InventoryEntry>();


	//CharacterData owner;
	public void Init(CharacterData owner, int capacity) {
		//this.owner = owner;
		numSlots = capacity;
	}

	/// <summary>
	/// Ajouter un objet aux entrées d'inventaire
	///		rechercher si une entrée contient déjà un objet identique
	///			si OUI => ajouter 1 à la quantité
	///			si NON => ajouter une entrée
	/// </summary>
	/// <param name="item">l'objet à ajouter</param>
	public void AddItem(Loot item) {
		if (isFull) return;

		App.inventoryUI.panel.SetActive(true);
		if (App.inventoryUI.firstUse) {
			App.inventoryUI.OnFirstUse();
		}

		bool found = false;
		for (int i = 0; i < entries.Count; ++i) {           // pour chaque entrée existante
			if (entries[i].item.Equals(item)) {             // si l'objet contenu est identique
				entries[i].ChangeQuantity(1);               // ajouter 1 à la quantité
				found = true;                               // trouvé
				item.entry = entries[i];
				item.transform.position = new Vector3(0, -50, 0);
				break;
			}
		}

		if (!found) {                                               // si on n'a pas trouvé
			InventoryEntry entry = new InventoryEntry(item);            // créer une nouvelle entrée d'inventaire
			foreach (ItemEntryUI entryUI in inventoryUI.entries) {      // trouver un emplacement d'affichage libre
				if (entryUI.isFree) {
					entryUI.Init(entry);
					break;
				}
			}
			entries.Add(entry);
			item.transform.position = new Vector3(0, -50, 0);
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
			var iEntry = entry as InventoryEntry;
			if (iEntry.item.itemBase.usable) {                                                      // si l'objet est utilisable
				SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() {    // jouer le son associé
					Clip = SFXManager.ItemUsedSound
				});
				iEntry.ChangeQuantity(-1);                                                  // retirer 1 à la quantité
				return true;
			}
		}
		return false;
	}

	public void Clear() {
		for (int i = entries.Count - 1; i > -1; i--) {
			entries[i].Clear();
		}
		entries.Clear();
	}

	public void RemoveItem(InventoryEntry entry) {
		entry.ChangeQuantity(-1);               // retirer 1 à la quantité
	}

	public bool HasCompatibleItem(Target target) {
		foreach (InventoryEntry entry in entries) {
			if (target.CompatibleWith(entry.item)) return true;
		}
		return false;
	}
}
