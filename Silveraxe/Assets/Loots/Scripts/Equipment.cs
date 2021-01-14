using System.Collections.Generic;
using System.Collections;
using UnityEngine;

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
		playerManager.characterData.equipment.AddItem(this);															//		ajouter l'objet à l'équipement du joueur
		isInPlayerCollider = false;                                                                                     //		l'objet n'est plus dans le collider du joueur (=> non intéractible)
		SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });      //		son
		targetsManager.OnTake();                                                                                        //		extinction de toutes les cibles
		if (target) {                                                                                                   //		mise à jour de la cible (s'il était sur une cible)
			target.item = null;
			target = null;
		}
	}

	/// <summary>
	/// Déposer un objet d'inventaire
	/// </summary>
	/// <param name="target">le lieu</param>
	/// <param name="entry">l'entrée d'inventaire </param>
	public override void Drop(Target target) {
		this.target = target;
		itemBase.animate = true;
		transform.position = target.targetPos;
		StartAnimation();
		playerManager.characterData.equipment.RemoveItem(entry as EquipmentEntry);			// retirer l'objet déposé de l'équipement du joueur
	}


}