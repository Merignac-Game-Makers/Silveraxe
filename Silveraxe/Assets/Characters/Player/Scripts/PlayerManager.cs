using System.Collections;
using UnityEngine;
using UnityEngine.AI;

using static App;

/// <summary>
/// Gestion du personnage joueur
/// </summary>
public class PlayerManager : Character
{
	public PlayerMode playerMode { get; set; } = PlayerMode.normal;

	public Collider detectionCollider;


	public MovementInput movementInput { get; private set; }
	public bool shouldMove => movementInput.shouldMove;

	// Interactions
	InteractableObject interactable = null;                         // objet avec lequel le joueur intéragit

	#region Initialisation
	void Awake() {
		playerManager = this;                                       // création de l'instance statique
	}

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();

		characterData = GetComponent<CharacterData>();              // caractéristiques du joueur
		characterData.Init();                                       // ... initialisation

		movementInput = GetComponentInChildren<MovementInput>();

		detectionCollider.enabled = true;
	}
	#endregion

	public override bool IsInteractable() { return true; }

	void Update() {

		// controler la vitesse sur les NavMesh Links (par défaut elle est trop rapide)
		if (navAgent.isOnOffMeshLink && !MoveAcrossNavMeshesStarted) {
			MoveAcrossNavMeshesStarted = true;
			StartCoroutine(MoveAcrossNavMeshLink(navAgent.destination));
		}
	}

	public void SetPlayerMode(PlayerMode mode, bool on, Character other = null) {
		switch (mode) {
			case PlayerMode.normal:
				playerMode = mode;
				cameraController.SetCamera(cameraController.vCamFollow);                // activer la caméra 'follow'
				playerManager.FaceTo(false);                                            // cesser d'orienter le PNJ vers le joueur
				other?.FaceTo(false);                                                   // cesser d'orienter le PNJ vers le joueur
				interactableObjectsManager.SelectAll(true);                             // resélectionner tous les objets intéractibles de la scène quand on commence un dialogue... 
				animatorController?.anim?.SetInteger("Fight", -1);                      // animation 'idle'
				fightController.other = null;
				break;

			case PlayerMode.dialogue:
				// scène
				if (on) {
					playerMode = mode;
					cameraController.SetCamera(cameraController.vCamDialogue);              // activer la caméra 'dialogue'
					interactableObjectsManager.SelectAll(false);								// désélectionner tous les objets intéractibles de la scène quand on commence un dialogue... 
				} else {
					SetPlayerMode(PlayerMode.normal, true, other);
				}
				// personnages
				StopAgent();                                                            // arrêter le déplacement du joueur
				animatorController?.anim.SetBool("Dialogue", on);                     // activer/désactiver l'animation 'dialogue' du joueur
				other?.animatorController?.anim?.SetBool("Dialogue", on);             // activer/désactiver l'animation 'dialogue' du pnj
				break;

			case PlayerMode.fight:
				if (on) {
					playerMode = mode;
					animatorController?.anim?.SetInteger("Fight", 0);                   // animation 'fight idle'
					fightController.other = other;
					navAgent.SetDestination(fightController.FightPosition(other));
				} else {
					SetPlayerMode(PlayerMode.normal, true, other);
				}

				// le PNJ 
				other.isInFightMode = on;
				other.FaceTo(on, gameObject);
				// le joueur
				isInFightMode = on;
				FaceTo(on, other.gameObject);
				// bascule d'affichage des ActionSprites
				interactableObjectsManager.SelectAll(!on);                          // masquer tous les actionsSprites pendant le combat
				other.Highlight(true);
				break;
		}
	}

	public void TestAnimationEvent(int param) {
		Debug.Log(param);
	}

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
			if (interactable != null) {                 // si l'objet rencontré est un 'intéractible'
				interactable.isInPlayerCollider = true;
				if (interactable.IsInteractable()) {    //		si son statut est 'actif'
					interactable.Highlight(true); //			montrer le sprite d'action
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
				SetPlayerMode(PlayerMode.fight, false, character);
			}
		}
	}
	#endregion

	#region Navigation
	/// <summary>
	/// interrompre la navigation
	/// </summary>
	public void StopAgent() {
		navAgent.ResetPath();                    // annulation de la navigation en cours
		navAgent.velocity = Vector3.zero;        // vitesse nulle
	}

	/// <summary>
	/// contrôler la vitesse sur les NavMesh Links
	/// (par défaut, dans UNITY,  les déplacements sont plus rapides sur les NavLinks... BUG ?)
	/// cette coroutine corrige le phénomène
	/// </summary>
	/// <param name="destination">destination</param>
	/// <returns></returns>
	IEnumerator MoveAcrossNavMeshLink(Vector3 destination) {
		OffMeshLinkData data = navAgent.currentOffMeshLinkData;
		navAgent.updateRotation = false;

		Vector3 startPos = navAgent.transform.position;                      // départ
		Vector3 endPos = data.endPos + Vector3.up * navAgent.baseOffset;     // arrivée
		float duration = (endPos - startPos).magnitude / navAgent.speed;     // durée du déplacement
		float t = 0.0f;
		float tStep = 1.0f / duration;                                      // incrément

		while (t < 1.0f) {                                                  // tant qu'on est pas arrivé
			transform.position = Vector3.Lerp(startPos, endPos, t);         // calculer le point de passage
			navAgent.SetDestination(transform.position);                     // aller au point de passage
			t += tStep * Time.deltaTime;                                    // incrémenter le timer
			yield return null;
		}
		// en fin de déplacement
		transform.position = endPos;                                        // annuler les arrondis de calcul sur la position finale
		navAgent.updateRotation = true;                                      // orienter le personnage
		navAgent.CompleteOffMeshLink();                                      // quitter le mode 'NavMesh Link'
		MoveAcrossNavMeshesStarted = false;
		navAgent.SetDestination(destination);                                // continuer vers la destination initiale
	}

	#endregion

}
