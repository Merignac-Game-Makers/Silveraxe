using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

using static App;

public class MovementInput : MonoBehaviour {
	public float maxSpeed = .5f;
	public float accelerationTime = 0.5f;
	public float rotSpeed = 10;					// vitesse de rotation

	public float vSpeed { get; set; } = 0;		// vitesse avant/arrière
	public float hSpeed { get; set; } = 0;      // vitesse gauche/droite


	public LayerMask environmentLayer;

	public bool canMove { get; set; } = true;

	public Vector3 velocity { get; set; } = Vector3.zero;


	float k;
	float vTimer = 0;
	float hTimer = 0;

	private float vAxis = 0;
	private float hAxis = 0;
	private float vMouse = 0;
	private float hMouse = 0;

	public float yMin = 0;
	public float yMax = 6;
	private float yNeutral;

	CinemachineComposer c;

	private void Start() {
		c = cameraController.vCamFollow.GetCinemachineComponent<CinemachineComposer>();
		yNeutral = c.m_TrackedObjectOffset.y;
	}

	void LateUpdate() {
		//------------------------
		// déplacements au clavier
		//------------------------
		if (!playerManager.isAlive) return;                             // quand on est mort, on ne bouge plus !
		if (!canMove) return;                                           // si les déplacements sont désactivés, on ne bouge plus ! (transitions entre les scènees)
		if (SceneModeManager.sceneMode != SceneMode.dialogue) {         // en mode dialogue on ne bouge pas non plus

			vAxis = Input.GetAxis("Vertical");			// déplacement avant/arrière
			hAxis = Input.GetAxis("Horizontal");        // déplacement latéral

			hMouse = (Input.mousePosition.x - Screen.width / 2) / Screen.width;
			vMouse = (Input.mousePosition.y - Screen.height / 2) / Screen.height;

			velocity = Vector3.zero;

			// déplacement avant/arrière
			if (vAxis != 0) {
				vTimer += Time.deltaTime;
				k = Mathf.Min(vTimer / accelerationTime, 1);
				vSpeed = Mathf.Lerp(0, maxSpeed, k);
				Vector3 v = transform.forward * vAxis * vSpeed;
				if (HasGround(transform.position + v + Vector3.up))		// ne pas tomber hors du monde !
					transform.position += v;
				velocity += Vector3.forward * vAxis * vSpeed;
			} else {
				vTimer = 0;
				vSpeed = 0;
			}

			// déplacement latéral
			if (hAxis != 0) {
				hTimer += Time.deltaTime;
				k = Mathf.Min(hTimer / accelerationTime, 1);
				hSpeed = Mathf.Lerp(0, maxSpeed, k);
				Vector3 v = transform.right * hAxis * hSpeed;
				if (HasGround(transform.position + 2*v + Vector3.up))     // ne pas tomber hors du monde !
					transform.position += v;
				velocity += Vector3.right * hAxis * hSpeed;
			} else {
				hTimer = 0;
				hSpeed = 0;
			}

			// rotation gauche/droite
			if (hMouse != 0 && (vSpeed != 0 || hSpeed != 0 || Input.GetButton("Fire2"))) {
				transform.Rotate(Vector3.up, hMouse * rotSpeed);
			}

			// rotation haut/bas
			if (vMouse >.25f && Input.GetButton("Fire2")) {
				c.m_TrackedObjectOffset.y = yMax;
			} else if (vMouse < -.25f && Input.GetButton("Fire2")) {
				c.m_TrackedObjectOffset.y = yMin;
			} else {
				c.m_TrackedObjectOffset.y = yNeutral;
			}


		}





		//// A => regarder le ciel
		//if (Input.GetKeyDown(KeyCode.A)) {
		//	var c = cameraController.vCamFollow.GetCinemachineComponent<CinemachineComposer>();
		//	c.m_TrackedObjectOffset.y = 6;
		//}
		//if (Input.GetKeyUp(KeyCode.A)) {
		//	var c = cameraController.vCamFollow.GetCinemachineComponent<CinemachineComposer>();
		//	c.m_TrackedObjectOffset.y = 2;
		//}
	}

	private bool HasGround(Vector3 fromSkyPosition) {
		RaycastHit feetOutHit;
		if (Physics.Raycast(fromSkyPosition, Vector3.down * 2f, out feetOutHit, 2f, environmentLayer)) {
			return true;
		}
		return false;
	}
}
