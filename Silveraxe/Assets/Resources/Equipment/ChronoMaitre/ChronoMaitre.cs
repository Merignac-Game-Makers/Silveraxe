using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChronoMaitre : Equipment {
	protected override void OnTake() {
		App.instructionsUI.showM = true;
		App.readableUI.ShowMessage(itemBase.description, itemBase.itemSprite, new System.Action(() =>
			itemBase.ShowExplanation(Callback)
		));
	}

	void Callback() {
		App.timeUI.showButtons = true;
		App.timeUI.showDays = true;
		App.timeUI.Show(true);

	}
}
