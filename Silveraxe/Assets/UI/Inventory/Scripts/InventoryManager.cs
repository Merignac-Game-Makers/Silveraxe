using System.Collections.Generic;
using UnityEngine;

using static App;

/// <summary>
/// This handles the inventory of our character. Each slot can hold one
/// TYPE of object, but those can be stacked without limit (e.g. 1 slot used by health potions, but contains 20
/// health potions)
/// </summary>
public class InventoryManager
{

	// Pas de limite au nombre d'objets en inventaire
	private const int numSlots = 0;
	public List<InventoryEntry> entries = new List<InventoryEntry>();


	CharacterData owner;

	public void Init(CharacterData owner) {
		this.owner = owner;
		inventoryManager = this;
	}

	/// <summary>
	/// Ajouter un objet aux entrées d'inventaire
	///		rechercher si une entrée contient déjà un objet identique
	///			si OUI => ajouter 1 à la quantité
	///			si NON => ajouter une entrée
	/// </summary>
	/// <param name="item">l'objet à ajouter</param>
	public void AddItem(Loot item) {
		bool found = false;
		for (int i = 0; i < entries.Count; ++i) {           // pour chaque entrée existante
			if (entries[i].item == item) {                  // si l'objet contenu est identique
				entries[i].count += 1;                      // ajouter 1 à la quantité
				found = true;                               // trouvé
				entries[i].ui.UpdateEntry();                // mettre l'objet d'interface associé à jour	
				item.entry = entries[i];
				item.transform.position = new Vector3(0, -50, 0);
				break;
			}
		}

		if (!found) {                                       // si on n'a pas trouvé
			InventoryEntry entry = new InventoryEntry(item);// créer une nouvelle entrée
			entry.ui =                                      // créer l'ojet d'interface associé
				inventoryUI.AddItemEntry(entries.Count - 1, entry);
			entry.item.entry = entry;
			entries.Add(entry);
			item.transform.position = new Vector3(0, -50, 0);
		}
		inventoryUI.UpdateEntries();
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


	public void RemoveItem(InventoryEntry entry) {
		entry.count -= 1;                                               // retirer 1 à la quantité
		if (entry.count <= 0) {                                         // si la quantité est nulle
			entries.Remove(entry);                                      // retirer l'entrée de l'inventaire
		}
		entry.ui.UpdateEntry();
	}
}
