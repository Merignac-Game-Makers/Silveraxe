using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entrée d'inventaire
/// contient :
///		- 1 Item
///		- le nombre d'exemplaires détenu
///		- la référence à l'interface utilisateur associé
/// </summary>
[System.Serializable]
public class EquipmentEntry : Entry
{

	[System.NonSerialized]
	public Equipment item;

	[System.NonSerialized]
	public new EquipmentEntryUI ui;
	public Equipment.EquipmentLevel? itemLevel => item?.equipmentLevel;

	public void Equip(Equipment item) {
		this.item = item;
	}
}

