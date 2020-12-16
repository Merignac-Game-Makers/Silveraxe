using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static App;

/// <summary>
/// Gestion du personnage joueur
/// </summary>
public class PlayerManager : Character
{
	public GameObject polyartSkin;		// apparence basic = plyart
	public GameObject hpSkin;			// apparence medium = hand painted
	public GameObject pbrSkin;          // apparence high =	PBR

	public GameObject shieldHand;		// 
	public GameObject swordHand;		// 


	public MovementInput movementInput { get; private set; }		// gestionnaire de déplacements

	// Interactions
	InteractableObject interactable = null;                         // objet avec lequel le joueur intéragit

	public override bool IsInteractable() { return true; }			// le joueur peut toujours intéragir avec son environnement

	#region Initialisation
	void Awake() {
		playerManager = this;                                       // création de l'instance statique
	}

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();

		movementInput = GetComponentInChildren<MovementInput>();	// gestion des déplacements
		ShowSkin(characterData.equipment.GetSetLevel());			// apparence initiale
	}
	#endregion


	#region Intéractions

	public void OnTriggerStay(Collider other) {
		if (other.gameObject != gameObject) {
			interactable = other.gameObject.GetComponent<InteractableObject>();
			if (interactable != null) {                         // si l'objet rencontré est un 'intéractible'
				interactable.isInPlayerCollider = true;
				interactable.Highlight(true);					//			montrer le sprite d'action
			}
		}
	}

	public void OnTriggerExit(Collider other) {
		if (other.gameObject != gameObject) {
			interactable = other.gameObject.GetComponent<InteractableObject>();
			if (interactable != null) {												// si l'objet rencontré est un 'intéractible'
				interactable.Highlight(false);										// éteindre l'objet
				interactable.isInPlayerCollider = false;
			}
			var character = other.GetComponent<Character>();						// si c'est un PNJ
			if (character && character.isInFightMode) {								// en mode combat
				SceneModeManager.SetSceneMode(SceneMode.fight, false, character);	// repasser la scène en mode normal (on a pris la fuite)
			}
		}
	}
	public override void Act() { }
	#endregion

	#region Navigation
	/// <summary>
	/// interrompre la navigation
	/// </summary>
	public void StopAgent() {
		navAgent.ResetPath();                    // annulation de la navigation en cours
		navAgent.velocity = Vector3.zero;        // vitesse nulle
	}
	#endregion

	#region Equipements
	/// <summary>
	/// Changer l'apparence du joueur
	/// </summary>
	/// <param name="level"></param>
	public void Promote(Equipment.EquipmentLevel level) {
		StartCoroutine(Ipromote(level));
	}
	IEnumerator Ipromote(Equipment.EquipmentLevel level) {
		animatorController.anim.SetBool("Promote", true);	// lancer l'animation
		yield return new WaitForSeconds(1.3f);				// attendre
		ShowSkin(level);									// changer l'apparence
	}

	void ShowSkin(Equipment.EquipmentLevel? level) {
		// la peau
		GameObject activeSkin = polyartSkin;
		switch (level) {
			case Equipment.EquipmentLevel.medium:
				activeSkin = hpSkin;
				break;
			case Equipment.EquipmentLevel.high:
				activeSkin = pbrSkin;
				break;
		}
		polyartSkin.SetActive(activeSkin == polyartSkin);
		hpSkin.SetActive(activeSkin == hpSkin);
		pbrSkin.SetActive(activeSkin == pbrSkin);
		// les équipements
		EquipmentSet set = activeSkin.GetComponent<EquipmentSet>();
		set.Equip(set.shield, shieldHand);
		set.Equip(set.sword, swordHand);
	}
	#endregion


	#region sauvegarde
	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	/// <param name="sav">la sauvegarde en cours d'élaboration</param>
	//public new SPlayer serialized;
	public override  void Serialize(List<object> sav) {
		sav.Add(new SPlayer() {
			guid = guid.ToByteArray(),                                                  // identifiant unique
			position = transform.position.toArray(),                                    // position
			rotation = transform.rotation.toArray(),                                    // rotation
			stats0 = characterData.stats.baseStats,                                     // statistiques de base
			stats1 = characterData.stats.stats,                                         // statistiques de base
			currentHealth = characterData.stats.CurrentHealth,                          // points de vie courants						
			inventory = new InventoryData(characterData.inventory),                     // inventaire
			equipment = new EquipmentData(characterData.equipment),                     // équipement
			navAgentDestination = navAgent ? navAgent.destination.toArray() : null      // destination de navigation
		}); ;
	}

	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		base.Deserialize(serialized);
		if (serialized is SPlayer) {
			SPlayer s = serialized as SPlayer;
			if (s.equipment != null)
				s.equipment.CopyTo(characterData.equipment);                    // équipement
		}
	}
	#endregion

}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class SPlayer : SCharacter
{
	public EquipmentData equipment;             // équipement

}