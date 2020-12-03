using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using static InteractableObject.Action;
using static App;
using UnityEngine.UI;

/// <summary>
/// Describes an InteractableObject that can be picked up and grants a specific item when interacted with.
///
/// It will also play a small animation (object going in an arc from spawn point to a random point around) when the
/// object is actually "spawned", and the object becomes interactable only when that animation is finished.
///
/// Finally it will notify the LootUI that a new loot is available in the world so the UI displays the name.
/// </summary>
public class Equipment : Loot
{

	public enum EquipmentLevel { basic, medium, high}

	public EquipmentCategory equipmentCategory;
	public EquipmentLevel equipmentLevel;

	protected override void Take() {
		// on ramasse l'objet
		playerManager.StopAgent();
		playerManager.characterData.equipment.AddItem(this);
		isInPlayerCollider = false;
		SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });
		targetsManager.OnTake();
		if (target) {
			target.item = null;
			target = null;
		}
	}


}