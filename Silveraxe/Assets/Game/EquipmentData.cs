using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentData
{
	public List<EEntryData> entries = new List<EEntryData>();

	public EquipmentData(EquipmentManager equipment) {
		foreach(EquipmentEntry entry in equipment.entries) {
			entries.Add(new EEntryData(entry));
		}
	}

	public void CopyTo(EquipmentManager equipment) {
		//equipment.Clear();
		for (int i=0; i<entries.Count; i++) {
			entries[i].CopyTo(equipment);
		}
	}
}

[System.Serializable]
public class EEntryData
{
	public byte[] itemGuid;
	public int count;
	public EEntryData(EquipmentEntry entry) {
		itemGuid = entry.item.guid.ToByteArray();
	}

	public void CopyTo(EquipmentManager equipment) {
		Equipment item = Game.current.allGuidComponents[new System.Guid(itemGuid)].GetComponent<Equipment>();    // retrouver l'item dans la scène
		equipment.AddItem(item);
	}
}
