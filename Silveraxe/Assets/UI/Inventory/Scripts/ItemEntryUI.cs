using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static App;


public class ItemEntryUI : EntryUI
{
	public GameObject content;
	public Image iconeImage;
	public Image plus;
	public TMP_Text count;

	public Loot loot { get; set; }

	public bool isFree => entry == null || (entry as InventoryEntry).count <= 0;

	Texture2D cursor;

	private void Start() {
		Show(false);
	}

	public override void Init(Entry entry) {
		InventoryEntry iEntry = entry as InventoryEntry;
		iEntry.ui = this;
		this.entry = iEntry;
		loot = iEntry.item;
		loot.entry = iEntry;
		iconeImage.sprite = loot.itemSprite;

		//GetCursor(iconeImage.sprite.texture);

		count.text = "";
		label.text = loot.itemName;
		//plus.enabled = item.combinable;
		Show(true);
	}

	//void GetCursor(Texture2D source) {
	//	int targetX = (int)iconeImage.rectTransform.rect.width;
	//	int targetY = (int)iconeImage.rectTransform.rect.height;
	//	RenderTexture rt = new RenderTexture(targetX, targetY, 24);
	//	RenderTexture.active = rt;
	//	Graphics.Blit(source, rt);
	//	cursor = new Texture2D(targetX, targetY, TextureFormat.RGBA32, false);
	//	cursor.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
	//	cursor.Apply();
	//}

	/// <summary>
	/// mise à jour
	/// </summary>
	public override void UpdateEntry() {
		bool isEnabled = entry != null && (entry as InventoryEntry)?.count > 0;
		InventoryEntry iEntry = entry as InventoryEntry;

		if (isEnabled) {
			iconeImage.sprite = iEntry?.item.itemSprite;

			if (iEntry?.count > 1) {
				count.gameObject.SetActive(true);
				count.text = iEntry?.count.ToString();
			} else {
				count.gameObject.SetActive(false);
			}
			Show(true);
		} else {
			inventoryUI.selectedEntry?.Select(false);
			playerManager.characterData.inventory.entries.Remove(iEntry);
			Show(false);
		}
	}

	public override void Toggle() {
		foreach (ItemEntryUI entry in inventoryUI.entries) {                    // désélectionner toutes les autres entrées de l'inventaire
			if (entry != this && entry.selected)
				entry.Select(false);
		}
		base.Toggle();                                                          // sélectionner/déselectionner cette entrée
		EventSystem.current.SetSelectedGameObject(null);
	}

	void Show(bool on) {
		content.SetActive(on);
	}

	public override void Select(bool on) {
		if (on) {
			iconeImage.transform.localPosition = new Vector2(0, 20);
			iconeImage.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			inventoryUI.Select(this);
			targetsManager.OnItemSelect();
		} else {
			iconeImage.transform.localPosition = new Vector2(0, 0);
			iconeImage.transform.localScale = new Vector3(.9f, .9f, .9f);
			targetsManager.OnTake();
			inventoryUI.Select(null);
		}
		label.enabled = on;
		selected = on;

		//if (on) {
		//	uiManager.SetCursor(cursor);
		//} else {
		//	uiManager.ResetCursor();
		//}
	}
}
