using UnityEngine;
using UnityEngine.UI;

public class ItemEntryUI : EntryUI
{
	public Image iconeImage;
	public Image plus;

	ItemEntryUI[] all;
	public Loot item;
	//ChapterManager chapterManager;

	private void Start() {
		inventoryUI = InventoryUI.Instance;
	}

	public override void Init(Entry entry) {
		this.entry = entry;
		entry.ui = this;
		item = (entry as InventoryEntry).item;
		iconeImage.sprite = item.ItemSprite;
		lowerText.text = "";
		label.text = item.ItemName;
		//plus.enabled = item.combinable;
	}

	/// <summary>
	/// mise à jour
	/// </summary>
	public override void UpdateEntry() {
		bool isEnabled = entry != null && (entry as InventoryEntry)?.count > 0;

		if (isEnabled) {
			iconeImage.sprite = (entry as InventoryEntry)?.item.ItemSprite;
			if ((entry as InventoryEntry)?.count > 1) {
				lowerText.gameObject.SetActive(true);
				lowerText.text = (entry as InventoryEntry)?.count.ToString();
			} else {
				lowerText.gameObject.SetActive(false);
			}
		} else {
			inventoryUI.RemoveEntry(this);
		}
	}

	public override void Toggle() {
		//var combineItem = inventoryUI.combineUI.item;
		//if (combineItem != null && item == combineItem.combineWith) {               // si une entrée combinable avec l'item est actuellement sélectionnée
		//																			// Debug.Log("Combine");
		//	var combineEntry = inventoryUI.combineUI.entry;                         //		récupérer l'entrée d'inventaire concernée
		//	combineEntry.item = combineItem.obtain;                                 //		remplace l'item de cette entrée par le résultat de la combinaison
		//	combineEntry.count = 1;                                                 //		en 1 exemplaire
		//	combineEntry.ui.Init(combineEntry);                                     //		mettre à jour l'interface de cette entrée
		//	inventoryUI.combineUI.SetObject(combineEntry);                          //		afficher l'objet obtenu dans le pannea 'combine'
		//	inventoryUI.RemoveEntry(this);                                          //		supprimer l'entrée de l'objet utilisé pour la combinaison

		//} else {                                                                    // sinon
			all = inventoryUI.GetComponentsInChildren<ItemEntryUI>();
			foreach (ItemEntryUI entry in all) {                                    // désélectionner toutes les autres entrées de l'inventaire
				if (entry != this && entry.selected)
					entry.Select(false);
			}
			base.Toggle();                                                          // sélectionner/déselectionner cette entrée
			//if (selected && item.combinable) {                                      // si on sélectionne et que l'item est combinable
			//	inventoryUI.combineUI.SetObject(entry as InventoryEntry);           //		afficher le panneau 'combine'
			//} else {                                                                // sinon
			//	inventoryUI.combineUI.Clear();                                      //		masquer le panneau combine
			//}
		//}
	}
}
