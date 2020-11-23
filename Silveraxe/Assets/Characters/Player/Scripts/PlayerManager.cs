using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

using static InteractableObject.Action;
using static App;

/// <summary>
/// Gestion du personnage joueur
///		- Déplacements
///		- Intéractions
/// </summary>
public class PlayerManager : Character
{
	// navigation
	[HideInInspector]
	public NavMeshAgent m_Agent;                                    // agent de navigation
	bool MoveAcrossNavMeshesStarted = false;                        // flag : est-on sur un nav mesh link ? (pour gérer la vitesse)
	[HideInInspector]
	public bool inTransit;                                          // l'agent est-il dans un transit (chagement de zone assisté)
	public MovementInput movementInput { get; private set; }
	public bool shouldMove => movementInput.shouldMove;

	// Interactions
	InteractableObject interactable = null;							// objet avec lequel le joueur intéragit
																	//Activable m_TargetActivable = null;                             // objet magique avec lequel le joueur intéragit
	Collider m_TargetCollider;                                      // collider de l'objet en cours d'intéraction
	Entry m_DropItem = null;                                        // objet d'inventaire que le juoeur pose
	HighlightableObject m_Highlighted; //    { get; set; }          // objet en surbrillance  sous le pointeur de la souris
	CharacterData m_CurrentTargetCharacterData = null;              // caractéristiques du PNJ en intéraction

	// CharacterData
	[HideInInspector]
	public CharacterData characterData;                           // caractéristiques du joueur (santé, force...)

	// Raycast
	RaycastHit[] m_RaycastHitCache = new RaycastHit[16];            // cache des résultats de lancer de rayon
	RaycastHit m_HitInfo = new RaycastHit();                        // résultat unitaire du lancer de rayon
	int m_InteractableLayer;                                        // layer des objets intéractibles
	int m_PlayerLayer;                                              // layer du personnage
	int m_SasLayer;                                                 // layer des sas
	int raycastableLayers;                                          // tous les layers à tester pour le Raycast


	#region Initialisation
	void Awake() {
		playerManager = this;                                       // création de l'instance statique
	}

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();

		characterData = GetComponent<CharacterData>();              // caractéristiques du joueur
		characterData.Init();                                       // ... initialisation

		m_Agent = GetComponent<NavMeshAgent>();                     // préparation de la navigation
		movementInput = GetComponentInChildren<MovementInput>();

		m_InteractableLayer = 1 << LayerMask.NameToLayer("Interactable");       // layer des objets intéractibles
		m_PlayerLayer = 1 << LayerMask.NameToLayer("Player");                   // layer du joueur

		var postProcessingMask = 1 << LayerMask.NameToLayer("PostProcess");
		var ignoreRaycastMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
		raycastableLayers = ~(postProcessingMask | ignoreRaycastMask);
	}
	#endregion

	public override bool IsInteractable() { return true; }


	//static bool WantsToQuit() {
	//	return false;
	//}

	//static void Quit() {
	//	Debug.Log("Quitting the Player");
	//}

	//[RuntimeInitializeOnLoadMethod]
	//static void RunOnStart() {
	//	Application.wantsToQuit += WantsToQuit;
	//	Application.quitting += Quit;
	//}

	void Update() {
		// quitter le jeu par la touche escape
		if (Input.GetKeyDown(KeyCode.Escape)) {
			uiManager.ShowQuitUi();
		}

		// controler la vitesse sur les NavMesh Links (par défaut elle est trop rapide)
		if (m_Agent.isOnOffMeshLink && !MoveAcrossNavMeshesStarted) {
			MoveAcrossNavMeshesStarted = true;
			StartCoroutine(MoveAcrossNavMeshLink(m_Agent.destination));
		}
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
		}
	}
	#endregion

	#region Navigation
	/// <summary>
	/// interrompre la navigation
	/// </summary>
	public void StopAgent() {
		m_Agent.ResetPath();                    // annulation de la navigation en cours
		m_Agent.velocity = Vector3.zero;        // vitesse nulle
		inTransit = false;                      // on n'est plus en transit
	}

	/// <summary>
	/// contrôler la vitesse sur les NavMesh Links
	/// (par défaut, dans UNITY,  les déplacements sont plus rapides sur les NavLinks... BUG ?)
	/// cette coroutine corrige le phénomène
	/// </summary>
	/// <param name="destination">destination</param>
	/// <returns></returns>
	IEnumerator MoveAcrossNavMeshLink(Vector3 destination) {
		OffMeshLinkData data = m_Agent.currentOffMeshLinkData;
		m_Agent.updateRotation = false;

		Vector3 startPos = m_Agent.transform.position;                      // départ
		Vector3 endPos = data.endPos + Vector3.up * m_Agent.baseOffset;     // arrivée
		float duration = (endPos - startPos).magnitude / m_Agent.speed;     // durée du déplacement
		float t = 0.0f;
		float tStep = 1.0f / duration;                                      // incrément

		while (t < 1.0f) {                                                  // tant qu'on est pas arrivé
			transform.position = Vector3.Lerp(startPos, endPos, t);         // calculer le point de passage
			m_Agent.SetDestination(transform.position);                     // aller au point de passage
			t += tStep * Time.deltaTime;                                    // incrémenter le timer
			yield return null;
		}
		// en fin de déplacement
		transform.position = endPos;                                        // annuler les arrondis de calcul sur la position finale
		m_Agent.updateRotation = true;                                      // orienter le personnage
		m_Agent.CompleteOffMeshLink();                                      // quitter le mode 'NavMesh Link'
		MoveAcrossNavMeshesStarted = false;
		m_Agent.SetDestination(destination);                                // continuer vers la destination initiale
	}

	#endregion

}
