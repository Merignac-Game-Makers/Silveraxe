using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementInput : MonoBehaviour
{
	public float rotationSensitivity = .4f;   // How sensitive it with mouse

	public NavMeshAgent navAgent { get; private set; }
	private Camera playerCam;
	private Quaternion targetLookRotation;

	// déplacements
	NavMeshPath path;
	bool isMovingWithKeyboard = false;
	Vector2 playerScreenPos;
	Vector2 mouseScreenPos;
	float dScreen;
	float threshold;
	float speedFactor = 1;
	float standardAgentSpeed;

	// rotation
	RaycastHit mouseTarget = new RaycastHit();                      // résultat du lancer de rayon vers le pointeur de la souris
	int m_NavLayer;                                                 // layer de la navigation



	private void Awake() {
		navAgent = GetComponent<NavMeshAgent>();
	}
	// Start is called before the first frame update
	void Start() {
		playerCam = CameraController.Instance.GameplayCamera;
		path = new NavMeshPath();
		m_NavLayer = 1 << LayerMask.NameToLayer("Navigation");              // layer de la navigation
		targetLookRotation = Quaternion.identity;
	}



	float distance;
	bool hasMouseTarget;
	void Update() {
		//------------------------
		// déplacements au clavier
		//------------------------
		hasMouseTarget =Physics.Raycast(playerCam.ScreenPointToRay(Input.mousePosition), out mouseTarget, 1000f, m_NavLayer);
		distance = (mouseTarget.point - PlayerManager.Instance.transform.position).magnitude;
		if (hasMouseTarget && distance>navAgent.radius*5) {
			if (Input.GetAxis("Vertical") > 0) {
				MoveForward();
			} else if (Input.GetAxis("Vertical") < 0) {
				MoveBackward();
			} else if (Input.GetAxis("Horizontal") < 0) {
				MoveLeft();
			} else if (Input.GetAxis("Horizontal") > 0) {
				MoveRight();
			} else {
				isMovingWithKeyboard = true;
			}
			SetToMouseDirection();
		}


		//// sensibilité de la rotation à la souris
		//if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus)) {
		//	StartCoroutine(ISensitivity(+1));
		//} else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) {
		//	StartCoroutine(ISensitivity(-1));
		//}

	}

    private void FixedUpdate()
    {
		transform.rotation = Quaternion.Slerp(transform.rotation, targetLookRotation, Time.fixedDeltaTime);
	}

	private void SetToMouseDirection() {
		// Lancer de rayon de la caméra vers le pointeur de souris
		if ((Input.mousePosition.y > (Screen.height * .3)) && Physics.Raycast(playerCam.ScreenPointToRay(Input.mousePosition), out mouseTarget, 1000f, m_NavLayer)) {
				FaceTo(mouseTarget.point);
		}
	}


	Coroutine coroutine;
	Vector3 delta;
	Quaternion rotation;
	float timer = 0f;
	public void FaceTo(Vector3 other) {
		delta = other - transform.position;
		delta.y = 0;
		targetLookRotation = Quaternion.LookRotation(delta);
	}


	//IEnumerator ISensitivity(int k) {
	//	if (k < 0) {
	//		rotationSensitivity /= 1.2f;
	//		//sensitivity.sprite = s_minus;
	//	} else {
	//		rotationSensitivity *= 1.2f;
	//		//sensitivity.sprite = s_plus;
	//	}
	//	//sensitivity.enabled = true;
	//	yield return new WaitForSeconds(0.5f);
	//	//sensitivity.enabled = false;
	//}

	private void MoveForward() {
		isMovingWithKeyboard = true;
		SetToMouseDirection();
		//navAgent.speed = standardAgentSpeed * speedFactor;
		navAgent.CalculatePath(transform.position + transform.forward * 1f, path);
		navAgent.SetDestination(transform.position + transform.forward * 1f);
	}
	private void MoveBackward() {
		isMovingWithKeyboard = true;
		SetToMouseDirection();
		//navAgent.speed = standardAgentSpeed;
		navAgent.CalculatePath(transform.position + transform.forward * -.5f, path);
		navAgent.updateRotation = false;
		GoTo(transform.position + transform.forward * -.5f);
	}
	private void MoveLeft() {
		isMovingWithKeyboard = true;
		//navAgent.speed = standardAgentSpeed;
		navAgent.CalculatePath(transform.position + transform.right * -.5f, path);
		navAgent.updateRotation = false;
		GoTo(transform.position + transform.right * -.5f);
	}
	private void MoveRight() {
		isMovingWithKeyboard = true;
		//navAgent.speed = standardAgentSpeed;
		navAgent.CalculatePath(transform.position + transform.right * .5f, path);
		navAgent.updateRotation = false;
		GoTo(transform.position + transform.right * .5f);
	}

	/// <summary>
	/// Aller à un point donné
	/// </summary>
	/// <param name="destination">le point à atteindre</param>
	public void GoTo(Vector3 destination) {
		navAgent.SetDestination(destination);
	}

}
