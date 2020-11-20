using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementInput : MonoBehaviour
{
	public float rotationSensitivity = 1f;   // How sensitive it with mouse
	public Texture2D footSteps;
	public int footstepsSize = 32;


	public NavMeshAgent navAgent { get; private set; }
	private Camera playerCam;
	private Quaternion targetLookRotation;

	// déplacements
	NavMeshPath path;
	public bool shouldMove { get; private set; }
	bool isMovingWithKeyboard = false;
	Vector2 playerScreenPos;
	Vector2 mouseScreenPos;
	float dScreen;
	float threshold;
	float speedFactor = 1;
	float standardAgentSpeed;
	Vector2 hotspot;

	// rotation
	RaycastHit mouseTarget = new RaycastHit();                      // résultat du lancer de rayon vers le pointeur de la souris
	int m_NavLayer;                                                 // layer de la navigation

	float playerFeet;
	Texture2D cursor;
	private void Awake() {
		navAgent = GetComponent<NavMeshAgent>();
	}
	// Start is called before the first frame update
	void Start() {
		playerCam = CameraController.Instance.GameplayCamera;               // la caméra du player
		path = new NavMeshPath();                                           // le chemin à suivre
		m_NavLayer = 1 << LayerMask.NameToLayer("Navigation");              // layer de la navigation
		targetLookRotation = Quaternion.identity;                           // rotation initiale = 'pas de rotation'
		playerFeet = playerCam.WorldToScreenPoint(PlayerManager.Instance.transform.position).y / Screen.height;
		cursor = Resize(footSteps, footstepsSize, footstepsSize);

		hotspot = new Vector2(footstepsSize / 2, footstepsSize / 2); 
	}

	Texture2D Resize(Texture2D texture2D, int targetX, int targetY) {
		RenderTexture rt = new RenderTexture(targetX, targetY, 24);
		RenderTexture.active = rt;
		Graphics.Blit(texture2D, rt);
		Texture2D result = new Texture2D(targetX, targetY);
		result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
		result.Apply();
		return result;
	}

	float distance;
	bool hasMouseTarget;
	void Update() {
		//------------------------
		// déplacements au clavier
		//------------------------
		shouldMove = ShouldMove();
		if (shouldMove) {
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
		} 
	}

	Vector3 dir;
	float k;
	private void FixedUpdate() {
		if (navAgent.velocity.magnitude > .2f || Input.GetKey(KeyCode.Space)) {

			dir = mouseTarget.point - transform.position;
			dir.y = 0;

			k = (Camera.main.WorldToScreenPoint(transform.position).x - mouseTarget.point.x) / Screen.width;

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.fixedDeltaTime * rotationSensitivity);
		}
	}

	bool ShouldMove() {
		if (IsMouseInActiveArea()) {
			hasMouseTarget = Physics.Raycast(playerCam.ScreenPointToRay(Input.mousePosition), out mouseTarget, 1000f, m_NavLayer);
			distance = (mouseTarget.point - PlayerManager.Instance.transform.position).magnitude;
		} else {
			hasMouseTarget = false;
			distance = 0;
		}

		bool result = distance > navAgent.radius * 5f;
		if (result != shouldMove) {
			if (result) {
				UIManager.Instance.SetBaseCursor(true);
			} else {
				UIManager.Instance.SetBaseCursor(false);
				UIManager.Instance.ResetCursor();
			}
		}
		return result;
	}

	bool IsMouseInActiveArea() {
		return Input.mousePosition.x > 0 &&
			Input.mousePosition.x < Screen.width &&
			Input.mousePosition.y > Screen.height * playerFeet &&
			Input.mousePosition.y < Screen.height;
	}



	private void MoveForward() {
		isMovingWithKeyboard = true;
		//SetToMouseDirection();
		//navAgent.speed = standardAgentSpeed * speedFactor;
		navAgent.CalculatePath(transform.position + transform.forward * 1f, path);
		navAgent.SetDestination(transform.position + transform.forward * 1f);
	}
	private void MoveBackward() {
		isMovingWithKeyboard = true;
		//SetToMouseDirection();
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
