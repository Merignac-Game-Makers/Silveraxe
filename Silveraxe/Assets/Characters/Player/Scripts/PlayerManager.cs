using System.Collections;
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
	/// <summary>
	/// détection de collision avec les objets intéractibles
	/// Si l'objet est un intéractible au statut 'actif'
	///		=> m_TargetInteractable contient l'objet
	///		=> m_TargetCollider contient son collider
	/// </summary>
	/// <param name="other">objet rencontré</param>
	public void OnTriggerEnter(Collider other) {
		if (other.gameObject != gameObject) {
			interactable = other.gameObject.GetComponent<InteractableObject>();
			if (interactable != null) {							// si l'objet rencontré est un 'intéractible'
				interactable.isInPlayerCollider = true;
				if (interactable.IsInteractable()) {			//		si son statut est 'actif'
					interactable.Highlight(true);				//			montrer le sprite d'action
				}
			}
		}
	}

	public void OnTriggerExit(Collider other) {
		if (other.gameObject != gameObject) {
			interactable = other.gameObject.GetComponent<InteractableObject>();
			if (interactable != null) {                 // si l'objet rencontré est un 'intéractible'
				interactable.Highlight(false);
				interactable.isInPlayerCollider = false;
			}
			var character = other.GetComponent<Character>();
			if (character && character.isInFightMode) {
				SceneModeManager.SetSceneMode(SceneMode.fight, false, character);
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

}
