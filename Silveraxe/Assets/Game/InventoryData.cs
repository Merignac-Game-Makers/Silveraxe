using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
	public List<IEntryData> entries = new List<IEntryData>();

	public InventoryData(InventoryManager inventory) {
		foreach (InventoryEntry entry in inventory.entries) {
			entries.Add(new IEntryData(entry));
		}
	}

	public void CopyTo(InventoryManager inventory) {
		inventory.Clear();
		for (int i = 0; i < entries.Count; i++) {
			entries[i].CopyTo(inventory);
		}
	}
}

[System.Serializable]
public class IEntryData
{
	public byte[] itemGuid;
	public int count;
	public IEntryData(InventoryEntry entry) {
		if (entry.item.guid != null) {
			itemGuid = ((System.Guid)entry.item.guid).ToByteArray();
			count = entry.count;
		}
	}

	public void CopyTo(InventoryManager inventory) {
		Loot item = Game.Find<Loot>(itemGuid);    // retrouver l'item dans la scène
		//Loot item = Game.current.allGuidComponents[new System.Guid(itemGuid)].GetComponent<Loot>();    // retrouver l'item dans la scène

		if (item) {
			inventory.entries.Add(new InventoryEntry(item));
			var i = inventory.entries.Count - 1;
			inventory.entries[i].count = count;
			App.inventoryUI.entries[i].Init(inventory.entries[i]);

		}
	}
}
