using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementInput : MonoBehaviour
{
	public float rotationSensitivity = .4f;   //How sensitive it with mouse

	public NavMeshAgent navAgent { get; private set; }

	// déplacements
	private float normalSpeed;
	private float shiftSpeedFactor = 2f;
	NavMeshPath path;
	bool isMovingWithKeyboard = false;
	// rotation
	RaycastHit[] m_RaycastHitCache = new RaycastHit[16];            // cache des résultats de lancer de rayon
	RaycastHit m_HitInfo = new RaycastHit();                        // résultat unitaire du lancer de rayon
	int m_NavLayer;                                                 // layer de la navigation

	private float totalRun = 1.0f;
	private Vector3 p;
	private Vector3 rot;
	float mainSpeed = 1f;           // regular speed
	float shiftAdd = 5.0f;          // multiplied by how long shift is held.  Basically running
	float maxShift = 100.0f;        // Maximum speed when holdin gshift

	private void Awake() {
		navAgent = GetComponent<NavMeshAgent>();
	}
	// Start is called before the first frame update
	void Start() {
		path = new NavMeshPath();
		m_NavLayer = 1 << LayerMask.NameToLayer("Navigation");              // layer de la navigation
		normalSpeed = navAgent.speed;                                       // vitesse du player
	}


	Vector3 direction = Vector3.zero;
	Vector3 point;
	// Update is called once per frame
	void Update() {
		//------------------------
		// déplacements au clavier
		//------------------------
		if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) {
			MoveForward();
		} else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			MoveBackward();
		} else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) {
			MoveLeft();
		} else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			MoveRight();
		} else {
			isMovingWithKeyboard = false;
		}


		//------------------------
		// rotation à la souris
		//------------------------


		// sensibilité de la rotation à la souris
		if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus)) {
			StartCoroutine(ISensitivity(+1));
		} else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) {
			StartCoroutine(ISensitivity(-1));
		}

	}

	private void SetToMouseDirection() {
		// Préparation du lancer de rayon de la caméra vers le pointeur de souris
		Ray screenRay = CameraController.Instance.GameplayCamera.ScreenPointToRay(Input.mousePosition);
		int count = Physics.SphereCastNonAlloc(screenRay, .2f, m_RaycastHitCache, 1000.0f, m_NavLayer);                // objets du calque 'Navigation' sous la souris
		if (count > 0) {
			point = m_RaycastHitCache[0].point;
			direction = point - PlayerManager.Instance.transform.position.normalized;
		}

		if (isMovingWithKeyboard && navAgent.velocity.magnitude > .2f)
			FaceTo(point);
	}


	Coroutine coroutine;
	Vector3 delta;
	Quaternion rotation;
	float timer = 0f;
	public void FaceTo(Vector3 other) {
		delta = other - transform.position;
		delta.y = 0;
		rotation = Quaternion.LookRotation(delta);

		if (coroutine != null)
			StopCoroutine(coroutine);
		coroutine = StartCoroutine(IFaceTo(rotation, 1));
	}

	IEnumerator IFaceTo(Quaternion rot, float s) {
		timer = 0;
		while (timer <= s) {
			timer += Time.deltaTime;
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, timer);
			yield return new WaitForEndOfFrame();
		}
	}


	IEnumerator ISensitivity(int k) {
		if (k < 0) {
			rotationSensitivity /= 1.2f;
			//sensitivity.sprite = s_minus;
		} else {
			rotationSensitivity *= 1.2f;
			//sensitivity.sprite = s_plus;
		}
		//sensitivity.enabled = true;
		yield return new WaitForSeconds(0.5f);
		//sensitivity.enabled = false;
	}

	private void MoveForward() {
		isMovingWithKeyboard = true;
		SetToMouseDirection();
		navAgent.CalculatePath(transform.position + transform.forward * 1f, path);
		if (path.corners.Length <= 4)
			navAgent.SetDestination(transform.position + transform.forward * 1f);
	}
	private void MoveBackward() {
		isMovingWithKeyboard = true;
		SetToMouseDirection();
		navAgent.CalculatePath(transform.position + transform.forward * -.5f, path);
		if (path.corners.Length <= 4)
			navAgent.updateRotation = false;
		GoTo(transform.position + transform.forward * -.5f);
	}
	private void MoveLeft() {
		isMovingWithKeyboard = true;
		navAgent.CalculatePath(transform.position + transform.right * -.5f, path);
		if (path.corners.Length <= 4)
			navAgent.updateRotation = false;
		GoTo(transform.position + transform.right * -.5f);
	}
	private void MoveRight() {
		isMovingWithKeyboard = true;
		navAgent.CalculatePath(transform.position + transform.right * .5f, path);
		if (path.corners.Length <= 4)
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
