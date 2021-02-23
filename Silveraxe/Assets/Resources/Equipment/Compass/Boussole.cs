using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boussole : Equipment
{
	public string texte;
	protected override void OnTake() {
		App.readableUI.ShowText(texte);
	}
}
