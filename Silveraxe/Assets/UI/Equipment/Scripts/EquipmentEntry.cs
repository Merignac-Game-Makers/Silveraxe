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


	public void Equip(Equipment item) {
		this.item = item;
	}
}

