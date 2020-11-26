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
	Vector2 screenDirection;
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
		screenDirection = ((Vector2)Input.mousePosition - (Vector2)Camera.main.WorldToScreenPoint(playerManager.transform.position)).normalized;

		if (screenDirection.y > 0.1 && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)) {
			fTranslation = Input.GetAxis("Vertical") * navAgent.speed * 30;
			sTranslation = Input.GetAxis("Horizontal") * navAgent.speed * 10;
			fTranslation *= Time.deltaTime;
			sTranslation *= Time.deltaTime;
			move = new Vector3(sTranslation / 2, 0, fTranslation);
			dest = transform.TransformPoint(move);
			navAgent.updateRotation = fTranslation > .1;
			navAgent.SetDestination(dest);
		} else {
			fTranslation = 0;
			sTranslation = 0;
		}
	}

	private void FixedUpdate() {
		// rotation en fonction de la direction de la souris
		if (fTranslation > .1 || Input.GetKey(KeyCode.S)) {
			transform.Rotate(Vector3.up, screenDirection.x * rotationSensitivity);
		}
	}
}
