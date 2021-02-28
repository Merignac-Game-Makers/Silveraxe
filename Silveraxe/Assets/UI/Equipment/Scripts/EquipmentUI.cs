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
	public EquipmentEntryUI helmetSlot;
	public EquipmentEntryUI armorSlot;
	public EquipmentEntryUI weaponSlot;
	public EquipmentEntryUI shieldSlot;

	public int capacity { get; private set; }

	public EquipmentEntryUI[] entries { get; private set; }
	public EquipmentEntryUI selectedEntry { get; private set; }

	private void Awake() {
		equipmentUI = this;
		panel.SetActive(true);
	}

	void Start() {
		entries = GetComponentsInChildren<EquipmentEntryUI>(true);
		capacity = 4;
	}

	void Update() {
		//if (Input.GetKeyDown(KeyCode.E)) {
		//	Toggle();
		//}
	}
	public override void Toggle() {
		helmetSlot.gameObject.SetActive(!helmetSlot.gameObject.activeInHierarchy);
		armorSlot.gameObject.SetActive(!armorSlot.gameObject.activeInHierarchy);
		weaponSlot.gameObject.SetActive(!weaponSlot.gameObject.activeInHierarchy);
		shieldSlot.gameObject.SetActive(!shieldSlot.gameObject.activeInHierarchy);
	}

	public void Select(EquipmentEntryUI entry) {
		selectedEntry = entry;
	}
}
