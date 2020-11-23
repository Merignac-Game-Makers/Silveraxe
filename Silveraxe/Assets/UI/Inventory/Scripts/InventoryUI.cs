using System.Collections.Generic;
using UnityEngine;

using static App;

/// <summary>
/// Handle all the UI code related to the inventory (drag'n'drop of object, using objects, equipping object etc.)
/// </summary>
public class InventoryUI : UIBase
{
	public GameObject content;

	public ItemEntryUI itemEntryPrefab;
	public RectTransform slotPrefab;

	// Raycast
	readonly RaycastHit[] m_RaycastHitCache = new RaycastHit[16];
	int m_TargetLayer;


	//public GameObject combinePanel { get; private set; }
	public List<ItemEntryUI> entries { get; private set; } = new List<ItemEntryUI>();

	public EntryUI selectedEntry { get; set; }
	EntryUI hoveredItem;
	InventoryPanel iPanel;

	bool? prevStatus = false;

	private void Awake() {
		inventoryUI = this;
	}


	private void Start() {
		iPanel = panel.GetComponent<InventoryPanel>();
		gameObject.SetActive(true);

		m_TargetLayer = 1 << LayerMask.NameToLayer("Interactable");
	}


	void OnEnable() {
		hoveredItem = null;
	}

	public ItemEntryUI AddItemEntry(int idx, InventoryEntry inventoryEntry) {
		RectTransform slot = Instantiate(slotPrefab, content.transform);        // créer un nouvel emplacement
		ItemEntryUI itemEntry = Instantiate(itemEntryPrefab, slot);             // créer une nouvelle entrée d'inventaire dans cet emplacement																				//itemEntry.gameObject.SetActive(true);
		itemEntry.Init(inventoryEntry);
		if (entries.Count == 0)                                                 // si c'est le 1er objet
			Show();                                                             // montrer l'inventaire
		entries.Add(itemEntry);
		return itemEntry;
	}

	/// <summary>
	/// détruire une entrée
	/// </summary>
	/// <param name="entryUi"></param>
	public void RemoveEntry(ItemEntryUI entryUi) {
		Destroy(entryUi.transform.parent.gameObject);       // détruire le slot qui contient l'entrée
		entries.Remove(entryUi);
		if (entries.Count == 0)                                                 // si l'inventaire est vide
			Hide();                                                             // cacher l'inventaire
	}

	/// <summary>
	/// bascule d'affichage
	/// </summary>
	public override void Toggle() {
		//iPanel.Toggle(combinePanel);
		iPanel.Toggle();
	}
	public void Hide() {
		//iPanel.Hide(combinePanel);
		iPanel.Hide();
	}
	public void Show() {
		iPanel.Show();
	}

	/// <summary>
	/// Actualiser l'affichage de toutes les entrés d'iventaire
	/// </summary>
	public void UpdateEntries() {
		//this.item = item;
		for (int i = entries.Count - 1; i > 0; i--) {
			if ((entries[i].entry as InventoryEntry).count <= 0) {
				Destroy(entries[i].gameObject);
				entries.RemoveAt(i);
			} else {
				entries[i].UpdateEntry();
			}
		}
	}

	/// <summary>
	/// utiliser un objet (ex: boire une potion...)
	/// (inutilisé pour l'instant)
	/// </summary>
	/// <param name="usedItem"></param>
	public void ObjectDoubleClicked(Entry usedItem) {
		inventoryManager.UseItem(usedItem);
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


	public void SaveAndHide() {
		Save();
		Hide();
	}

	public void Save() {
		prevStatus = iPanel.isOn;
	}

	public void Restore() {
		if (iPanel.isOn != prevStatus)
			Toggle();
		prevStatus = null;
	}
}
