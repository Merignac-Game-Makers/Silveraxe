using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static App;


public class EquipmentEntryUI : EntryUI
{

	public GameObject content;
	public Image iconeImage;
	public Image plus;
	public Image background;


	public EquipmentCategory equipmentCategory;
	public Equipment item { get; set; }

	public bool isFree => entry == null;

	Texture2D cursor;

	private void Start() {
		Show(false);
	}

	public override void Init(Entry entry) {
		EquipmentEntry eEntry = entry as EquipmentEntry;
		eEntry.ui = this;
		this.entry = eEntry;
		item = eEntry.item;
		item.entry = eEntry;
		iconeImage.sprite = item.ItemSprite;

		label.text = item.ItemName;
		background.enabled = false;

		Show(true);
	}

	/// <summary>
	/// mise à jour
	/// </summary>
	public override void UpdateEntry() {
		bool isEnabled = entry != null && (entry as EquipmentEntry)?.item != null;
		EquipmentEntry eEntry = entry as EquipmentEntry;

		if (isEnabled) {
			iconeImage.sprite = eEntry?.item.ItemSprite;
			background.enabled = false;
			Show(true);
		} else {
			background.enabled = true;
			Show(false);
		}
	}

	void Show(bool on) {
		content.SetActive(on);
	}

	public override void Select(bool on) {
		if (on) {
			iconeImage.transform.localPosition = new Vector2(0, 20);
			iconeImage.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			equipmentUI.Select(this);
			targetsManager.OnItemSelect();
		} else {
			iconeImage.transform.localPosition = new Vector2(0, 0);
			iconeImage.transform.localScale = new Vector3(.9f, .9f, .9f);
			targetsManager.OnTake();
			inventoryUI.Select(null);
		}
		label.enabled = on;
		selected = on;
	}
}
