using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static App;

/// <summary>
/// Keep reference and update the Equipment entry (MagicBook + 2 artefacts)
/// </summary>
public class EquipmentUI : UIBase
{
	public EquipmentEntryUI helmetSot;
	public EquipmentEntryUI armorSlot;
	public EquipmentEntryUI weaponSlot;
	public EquipmentEntryUI shieldSlot;

	public int capacity { get; private set; }

	public EquipmentEntryUI[] entries { get; private set; }
	public EquipmentEntryUI selectedEntry { get; private set; }

	private void Awake() {
		equipmentUI = this;
	}

	void Start() {
		entries = GetComponentsInChildren<EquipmentEntryUI>();
		capacity = 4;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.I)) {
			Toggle();
		}
	}
	public override void Toggle() {
		panel.SetActive(!panel.activeInHierarchy);
	}

	public void Select(EquipmentEntryUI entry) {
		selectedEntry = entry;
	}
	//public void UpdateEquipment(EquipmentSystem equipment, StatSystem system)
	//{
	//    var book = equipment.GetItem(EquipmentItem.EquipmentSlot.Book);
	//    var sun = equipment.GetItem(EquipmentItem.EquipmentSlot.Sun);
	//    var moon = equipment.GetItem(EquipmentItem.EquipmentSlot.Moon);

	//    bookSlot.SetupEquipment(book);
	//    sunSlot.SetupEquipment(sun);
	//    moonSlot.SetupEquipment(moon);
	//}
}
