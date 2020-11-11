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
public class InventoryEntry : Entry
{
	public Loot item;
	public int count = 1;
	public new ItemEntryUI ui;

	public InventoryEntry(Loot item) {
		this.item = item;
		//this.item = GameObject.Instantiate(item);
		//this.item = new Loot(item);
	}
}

