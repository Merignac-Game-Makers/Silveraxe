using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static App;
/// <summary>
/// Entrée d'inventaire
/// contient :
///		- 1 Item
///		- le nombre d'exemplaires détenu
///		- la référence à l'interface utilisateur associé
/// </summary>
[System.Serializable]
public class InventoryEntry : Entry
{
	public Loot item;
	public int count = 1;
	[System.NonSerialized]
	public new ItemEntryUI ui;

	public InventoryEntry(Loot item) {
		this.item = item;
	}

	public void ChangeQuantity(int amount) {
		count += amount;
		ui.UpdateEntry();
	}

	public void Clear() {
		count = 0;
		item = null;
		ui = null;
	}
}

