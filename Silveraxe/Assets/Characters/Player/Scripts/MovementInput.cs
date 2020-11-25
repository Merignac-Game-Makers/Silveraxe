using UnityEngine;
using UnityEngine.AI;

using static App;

public class MovementInput : MonoBehaviour
{
	public NavMeshAgent navAgent { get; private set; }

	// rotation
	public float rotationSensitivity = 2f;   // How sensitive it with mouse

	// déplacements
	public bool shouldMove { get; private set; }
	public bool shouldMoveStraight { get; set; }
	public bool shouldMoveSide { get; set; }


	public Texture2D cursor;

	Vector2 screenDirection;

	private void Awake() {
		navAgent = GetComponent<NavMeshAgent>();
	}

	private void Start() {
		cursor = uiManager.Resize(cursor, uiManager.defaultCursorSize / 2);
	}

	float distance;
	void Update() {
		//------------------------
		// déplacements au clavier
		//------------------------
		if (GetMove()) {
			if (Input.GetAxis("Vertical") > 0) {
				MoveForward();
			} else if (Input.GetAxis("Vertical") < 0) {
				MoveBackward();
			} else if (Input.GetAxis("Horizontal") < 0) {
				MoveLeft();
			} else if (Input.GetAxis("Horizontal") > 0) {
				MoveRight();
			} else {
				shouldMoveStraight &= navAgent.velocity.magnitude > 10f;
				shouldMoveSide &= navAgent.velocity.magnitude > 1f; ;
			}
		}
	}

	Vector3 dir;
	float k;
	private void FixedUpdate() {
		// rotation en fonction de la direction de la souris
		if (navAgent.velocity.magnitude > .2f || Input.GetKey(KeyCode.S)) {
			//if (navAgent.updateRotation)
				transform.Rotate(Vector3.up, screenDirection.x * rotationSensitivity);
		}
	}

	bool GetMove() {
		screenDirection = ((Vector2)Input.mousePosition - (Vector2)Camera.main.WorldToScreenPoint(playerManager.transform.position)).normalized;
		shouldMove = screenDirection.y > 0.1;// &&  screenDirection.magnitude > .2;
		return shouldMove;
	}

	private void MoveForward() {
		shouldMoveStraight = true;
		shouldMoveSide = false;
		navAgent.updateRotation = true;
		navAgent.SetDestination(transform.position + transform.forward * 1f);
	}
	private void MoveBackward() {
		shouldMoveStraight = true;
		shouldMoveSide = false;
		navAgent.updateRotation = false;
		navAgent.SetDestination(transform.position + transform.forward * -.5f);
	}
	private void MoveLeft() {
		shouldMoveStraight = false;
		shouldMoveSide = true;
		navAgent.updateRotation = false;
		navAgent.SetDestination(transform.position + transform.right * -.5f);
	}
	private void MoveRight() {
		shouldMoveStraight = false;
		shouldMoveSide = true;
		navAgent.updateRotation = false;
		navAgent.SetDestination(transform.position + transform.right * .5f);
	}
}
