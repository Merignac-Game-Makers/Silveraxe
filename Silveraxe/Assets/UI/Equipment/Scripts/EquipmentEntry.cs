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
public class EquipmentEntry : Entry
{
	public Equipment item;
	public new EquipmentEntryUI ui;
	public Equipment.EquipmentLevel? itemLevel => item?.equipmentLevel;

	public void Equip(Equipment item) {
		this.item = item;
	}
}

