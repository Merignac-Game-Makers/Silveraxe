using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static App;

/// <summary>
/// Gestion du personnage joueur
/// </summary>
public class PlayerManager : Character
{
	public GameObject polyartSkin;      // apparence basic = plyart
	public GameObject hpSkin;           // apparence medium = hand painted
	public GameObject pbrSkin;          // apparence high =	PBR

	public GameObject shieldHand;       // 
	public GameObject swordHand;        // 


	public MovementInput movementInput { get; private set; }        // gestionnaire de déplacements

	// Interactions
	InteractableObject interactable = null;                         // objet avec lequel le joueur intéragit

	public override bool IsInteractable() { return true; }          // le joueur peut toujours intéragir avec son environnement

	#region Initialisation
	protected override void Awake() {
		base.Awake();
		playerManager = this;                                       // création de l'instance statique
	}

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();

		movementInput = GetComponentInChildren<MovementInput>();    // gestion des déplacements
		ShowSkin(characterData.equipment.GetSetLevel());            // apparence initiale
	}
	#endregion


	#region Intéractions

	public void OnTriggerStay(Collider other) {
		if (other.gameObject != gameObject) {
			interactable = other.gameObject.GetComponent<InteractableObject>();
			if (interactable != null) {                         // si l'objet rencontré est un 'intéractible'
				interactable.isInPlayerCollider = true;
				interactable.Highlight(true);                   //			montrer le sprite d'action
			}
		}
	}

	public void OnTriggerExit(Collider other) {
		if (other.gameObject != gameObject) {
			interactable = other.gameObject.GetComponent<InteractableObject>();
			if (interactable != null) {                                             // si l'objet rencontré est un 'intéractible'
				interactable.Highlight(false);                                      // éteindre l'objet
				interactable.isInPlayerCollider = false;
			}
			var character = other.GetComponent<Character>();                        // si c'est un PNJ
			if (character && character.isInFightMode) {                             // en mode combat
				SceneModeManager.SetSceneMode(SceneMode.fight, false, character);   // repasser la scène en mode normal (on a pris la fuite)
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
		animatorController.anim.SetBool("Promote", true);   // lancer l'animation
		yield return new WaitForSeconds(1.3f);              // attendre
		ShowSkin(level);                                    // changer l'apparence
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
	/// Sérialiser les infos à sauvegarder
	/// </summary>
	/// <returns></returns>
	public override SSavable Serialize() {
		var result = new SPlayer().Copy(base.Serialize());
		//result.scene = FindObjectOfType<SceneSaver>().gameObject.scene.name;
		result.equipment = new EquipmentData(characterData.equipment);                     // équipement
		return result;
	}


	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		if (((SSavable)serialized).version == App.saveVersion) {
			base.Deserialize(serialized);
			if (serialized is SPlayer) {
				SPlayer s = serialized as SPlayer;
				//App.currentSceneName = s.scene;
				if (s.equipment != null) {
					s.equipment.CopyTo(characterData.equipment);                    // équipement
				}
			}

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