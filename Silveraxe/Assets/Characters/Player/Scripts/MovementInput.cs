using UnityEngine;
using UnityEngine.AI;

using static App;

public class MovementInput : MonoBehaviour
{
	public NavMeshAgent navAgent { get; private set; }

	// rotation
	public float rotationSensitivity = 2f;   // How sensitive it with mouse

	// déplacements
	public Texture2D cursor;
	public Vector2 screenDirection { get; private set; }
	public float fTranslation { get; set; }
	public float sTranslation { get; set; }


	private void Awake() {
		navAgent = GetComponent<NavMeshAgent>();
	}

	private void Start() {
		cursor = uiManager.Resize(cursor, uiManager.defaultCursorSize / 2);
	}


	Vector3 move;
	Vector3 dest;
	void LateUpdate() {
		//------------------------
		// déplacements au clavier
		//------------------------
		screenDirection = ((Vector2)Input.mousePosition - (Vector2)Camera.main.WorldToScreenPoint(playerManager.transform.position));//.normalized;

		if (!playerManager.isAlive) return;                             // quand on est mort, on ne bouge plus !

		if (screneCrossing) {
			if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0) {
				screneCrossing = false;
				playerManager.navAgent.enabled = true;
			}
		}

		if (!playerManager.navAgent.enabled) return;                    // si le navAgent est désactivé, on ne bouge plus ! (transitions entre les scènees)

		if (SceneModeManager.sceneMode != SceneMode.dialogue) {         // en mode dialogue on ne bouge pas non plus
			if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)) {
				fTranslation = Input.GetAxis("Vertical") * navAgent.speed * 30;
				sTranslation = Input.GetAxis("Horizontal") * navAgent.speed * 5;
				fTranslation *= Time.deltaTime;
				sTranslation *= Time.deltaTime;
				move = new Vector3(sTranslation, 0, fTranslation);
				dest = transform.TransformPoint(move.normalized * navAgent.radius);
				navAgent.updateRotation = fTranslation > .1;
				navAgent.SetDestination(dest);
			} else {
				fTranslation = 0;
				sTranslation = 0;
			}
		}
	}

	private void FixedUpdate() {
		if (SceneModeManager.sceneMode != SceneMode.dialogue) {     // rotation en fonction de la direction de la souris
			if (fTranslation > .1 || Input.GetAxis("Fire3")>0) {
				transform.Rotate(Vector3.up, screenDirection.normalized.x * rotationSensitivity);
			}
		}
	}
}
