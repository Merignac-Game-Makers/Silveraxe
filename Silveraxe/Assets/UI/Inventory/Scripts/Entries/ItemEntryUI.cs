using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static App;


public class ItemEntryUI : EntryUI
{
	public Image iconeImage;
	public Image plus;

	ItemEntryUI[] all;
	//ChapterManager chapterManager;

	Texture2D cursor;

	public override void Init(Entry entry) {
		this.entry = entry;
		entry.ui = this;
		item = (entry as InventoryEntry).item;
		iconeImage.sprite = item.ItemSprite;

		GetCursor(iconeImage.sprite.texture);

		lowerText.text = "";
		label.text = item.ItemName;
		//plus.enabled = item.combinable;
	}

	void GetCursor(Texture2D source) {
		int targetX = (int)iconeImage.rectTransform.rect.width;
		int targetY = (int)iconeImage.rectTransform.rect.height;
		RenderTexture rt = new RenderTexture(targetX, targetY, 24);
		RenderTexture.active = rt;
		Graphics.Blit(source, rt);
		cursor = new Texture2D(targetX, targetY, TextureFormat.RGBA32, false);
		cursor.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
		cursor.Apply();
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
 		all = inventoryUI.GetComponentsInChildren<ItemEntryUI>();
		foreach (ItemEntryUI entry in all) {                                    // désélectionner toutes les autres entrées de l'inventaire
			if (entry != this && entry.selected)
				entry.Select(false);
		}
		base.Toggle();                                                          // sélectionner/déselectionner cette entrée
		EventSystem.current.SetSelectedGameObject(null);
	}

	//public override void Select(bool on) {
	//	base.Select(on);
	//	if (on) {
	//		uiManager.SetCursor(cursor);
	//	} else {
	//		uiManager.ResetCursor();
	//	}
	//}
}
