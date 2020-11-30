using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static App;
public class InventoryUI : UIBase
{
	public EntryUI selectedEntry { get; private set; }
	public ItemEntryUI[] entries { get; private set; }
	public int capacity { get; private set; }

	EntryUI hoveredItem;

	private void Awake() {
		inventoryUI = this;
	}
	void Start() {
		entries = GetComponentsInChildren<ItemEntryUI>();
		capacity = entries.Length;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.I)) {
			Toggle();
		}
	}

	public void Select(EntryUI entry) {
		selectedEntry = entry;
	}



	/// <summary>
	/// utiliser un objet (ex: boire une potion...)
	/// (inutilisé pour l'instant)
	/// </summary>
	/// <param name="usedItem"></param>
	public void ObjectDoubleClicked(Entry usedItem) {
		playerManager.characterData.inventory.UseItem(usedItem);
		ObjectHoverExited(hoveredItem);
	}

	/// <summary>
	/// Survol par le pointeur de souris
	/// TODO: pour l'instant sans effet... à conserver ???
	/// </summary>
	/// <param name="hovered">L'entrée sous la souris</param>
	public void ObjectHoveredEnter(EntryUI hovered) {       // début de survol
		hoveredItem = hovered;
	}
	public void ObjectHoverExited(EntryUI exited) {         // fin de survol
		if (hoveredItem == exited) {
			hoveredItem = null;
		}
	}

	public override void Toggle() {
		panel.SetActive(!panel.activeInHierarchy);
		selectedEntry?.Select(false);
	}
}
