using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

using static App;

public class MovementInput : MonoBehaviour {
	public float maxSpeed = .5f;
	public float accelerationTime = 0.5f;
	public float rotSpeed = 10;                 // vitesse de rotation

	public float vSpeed { get; set; } = 0;      // vitesse avant/arrière
	public float rSpeed { get; set; } = 0;      // vitesse de rotation gauche/droite

	public LayerMask environmentLayer;

	public bool canMove { get; set; } = true;

	public Vector3 velocity { get; set; } = Vector3.zero;


	float k;
	float vTimer = 0;
	float hTimer = 0;

	private float vAxis = 0;
	private float hAxis = 0;
	private float vMouse = 0;

	public float yMin = 0;
	public float yMax = 6;
	private float yNeutral;

	CinemachineComposer c;

	private void Start() {
		c = cameraController.vCamFollow.GetCinemachineComponent<CinemachineComposer>();     // cinemachineComposer
		yNeutral = c.m_TrackedObjectOffset.y;                                               // visée caméra : hauteur par défaut
	}


	public NavMeshAgent navAgent { get; private set; }
	Vector3 dest;

	private void Awake() {
		navAgent = GetComponent<NavMeshAgent>();
		navAgent.updateRotation = false;                                // la navigation ne gère pas la rotation
		navAgent.enabled = false;                                       // navigation inactive au démarrage (le personnage ne touche pas le sol)
	}


	//------------------------
	// déplacements au clavier
	//------------------------	
	void LateUpdate() {

		if (!playerManager.isAlive) return;                             // quand on est mort, on ne bouge plus !
		if (!canMove) return;                                           // si les déplacements sont désactivés, on ne bouge plus ! (transitions entre les scènees)
		if (SceneModeManager.sceneMode == SceneMode.dialogue) return;   // en mode dialogue on ne bouge pas non plus


		vAxis = Input.GetAxis("Vertical");          // déplacement avant/arrière
		hAxis = Input.GetAxis("Horizontal");        // rotation gauche/droite
		vMouse = (Input.mousePosition.y - Screen.height / 2) / Screen.height;	// position verticale de la souris

		velocity = Vector3.zero;					// vitesse nulle
		dest = transform.position;					// pas de déplacement

		// déplacement avant/arrière
		if (vAxis != 0) {											// si on appuie sur Z ou S
			vTimer += Time.deltaTime;								// calculer
			k = Mathf.Min(vTimer / accelerationTime, 1);			// la 
			vSpeed = Mathf.Lerp(0, maxSpeed, k);					// vitesse
			dest += transform.forward * vAxis * 3;					// et la destination
		} else {
			vTimer = 0;
			vSpeed = 0;
		}

		// rotation
		if (hAxis != 0) {
			hTimer += Time.deltaTime;
			k = Mathf.Min(hTimer / accelerationTime, 1);
			rSpeed = Mathf.Lerp(0, maxSpeed, k);
			transform.Rotate(Vector3.up, rSpeed * hAxis * rotSpeed);
		} else {
			hTimer = 0;
			rSpeed = 0;
		}

		if (navAgent.enabled)
			navAgent.SetDestination(dest);
		velocity = navAgent ? transform.InverseTransformVector(navAgent.velocity) : Vector3.zero;

		// rotation haut/bas
		if (vMouse > .25f && Input.GetButton("Fire2")) {
			c.m_TrackedObjectOffset.y = yMax;
		} else if (vMouse < -.25f && Input.GetButton("Fire2")) {
			c.m_TrackedObjectOffset.y = yMin;
		} else {
			c.m_TrackedObjectOffset.y = yNeutral;
		}


	}

	private bool HasGround(Vector3 fromSkyPosition) {
		RaycastHit feetOutHit;
		if (Physics.Raycast(fromSkyPosition, Vector3.down * 2f, out feetOutHit, 2f, environmentLayer)) {
			return true;
		}
		return false;
	}
}
