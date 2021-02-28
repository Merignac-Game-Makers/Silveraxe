using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boussole : Equipment
{
	public Compass compassUI;
	protected override void OnTake() {
		App.instructionsUI.showM = true;
		App.readableUI.ShowMessage(itemBase.description, itemBase.itemSprite, new System.Action(() =>
			itemBase.ShowExplanation(new System.Action(() =>
				compassUI.Show(true)
			)))
		) ;
	}
}
