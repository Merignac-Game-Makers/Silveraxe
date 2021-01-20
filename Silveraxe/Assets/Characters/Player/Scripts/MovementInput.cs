using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

using static App;

public class MovementInput : MonoBehaviour {
	public float maxSpeed = .5f;
	public float maxRotationSpeed = 15;
	public float accelerationTime = 0.5f;

	public float trnSpeed { get; set; } = 0;		// vitesse avant/arrière
	public float rotSpeed { get; set; } = 0;        // vitesse de rotation
	public float sdeSpeed { get; set; } = 0;        // vitesse gauche/droite


	public LayerMask environmentLayer;

	public bool canMove { get; set; } = true;

	public Vector3 velocity { get; set; } = Vector3.zero;


	float k;
	float vTimer = 0;
	float hTimer = 0;
	float sTimer = 0;

	private float vAxis = 0;
	private float hAxis = 0;
	private int sideMvt = 0;
	private bool keyA = false;
	private bool keyE = false;
	void LateUpdate() {
		//------------------------
		// déplacements au clavier
		//------------------------
		if (!playerManager.isAlive) return;                             // quand on est mort, on ne bouge plus !

		// reprise de déplacement après un portail
		if (sceneCrossing && !isLoadingData) {
			if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0) {
				sceneCrossing = false;
			} else {
				return;
			}
		}

		if (!canMove) return;                                           // si les déplacements sont désactivés, on ne bouge plus ! (transitions entre les scènees)

		if (SceneModeManager.sceneMode != SceneMode.dialogue) {         // en mode dialogue on ne bouge pas non plus

			vAxis = Input.GetAxis("Vertical");
			hAxis = Input.GetAxis("Horizontal");
			sideMvt = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.E) ? 1 : 0;
			velocity = Vector3.zero;

			// déplacement avant/arrière
			if (vAxis != 0) {
				vTimer += Time.deltaTime;
				k = Mathf.Min(vTimer / accelerationTime, 1);
				trnSpeed = Mathf.Lerp(0, maxSpeed, k);
				Vector3 v = transform.forward * vAxis * trnSpeed;
				if (HasGround(transform.position + v + Vector3.up))		// ne pas tomber hors du monde !
					transform.position += v;
				velocity += Vector3.forward * vAxis * trnSpeed;

				//Debug.Log(v + " - " + velocity);
			} else {
				vTimer = 0;
				trnSpeed = 0;
			}

			// déplacement latéral
			if (sideMvt != 0) {
				sTimer += Time.deltaTime;
				k = Mathf.Min(sTimer / accelerationTime, 1);
				sdeSpeed = Mathf.Lerp(0, maxSpeed, k);
				Vector3 v = transform.right * sideMvt * sdeSpeed;
				if (HasGround(transform.position + v + Vector3.up))     // ne pas tomber hors du monde !
					transform.position += v;
				velocity += Vector3.right * sideMvt * sdeSpeed;
			} else {
				sTimer = 0;
				sdeSpeed = 0;
			}

			// rotation
			if (hAxis != 0) {
				hTimer += Time.deltaTime;
				k = Mathf.Min(hTimer / accelerationTime, 1);
				rotSpeed = Mathf.Lerp(0, maxRotationSpeed, k);
				transform.Rotate(Vector3.up, hAxis * rotSpeed);
			} else {
				hTimer = 0;
			}
		}





		// barre d'espace => regarder le ciel
		if (Input.GetKeyDown(KeyCode.Space)) {
			var c = cameraController.vCamFollow.GetCinemachineComponent<CinemachineComposer>();
			c.m_TrackedObjectOffset.y = 6;
		}
		if (Input.GetKeyUp(KeyCode.Space)) {
			var c = cameraController.vCamFollow.GetCinemachineComponent<CinemachineComposer>();
			c.m_TrackedObjectOffset.y = 2;
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
