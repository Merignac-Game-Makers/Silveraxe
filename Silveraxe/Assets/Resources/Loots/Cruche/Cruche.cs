using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruche : Loot {
	public Target well;
	public Target grave;
	public RainbowFlower flower;
	public bool filled { get; set; } = false;

	protected override void OnDrop(Target target) {
		if (target == well) {
			itemBase = Resources.Load<ItemBase>("Loots/Cruche/CruchePleine");
			filled = true;
		}

		if (target == grave && filled) {
			flower.gameObject.SetActive(true);          // la fleur arc en ciel devient visible
			gameObject.SetActive(false);				// la cruche disparaît
		}
	}
}
