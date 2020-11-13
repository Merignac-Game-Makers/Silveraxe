using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Control the camera, mainly used as a reference to the main camera through the singleton instance, and to handle
/// mouse wheel zooming
/// </summary>
public class CameraController : MonoBehaviour
{
	public static CameraController Instance { get; set; }

	public Camera GameplayCamera;
	public CinemachineVirtualCamera vCam;
	public GameObject CameraTarget;
//	public PostProcessVolume ppv;

	[HideInInspector]
	public Stack<CinemachineVirtualCamera> vCams { get; set; }

	CinemachineBrain cinemachineBrain;
	/// <summary>
	/// Angle in degree (down compared to horizon) the camera will look at when at the closest of the character
	/// </summary>
	public float MinAngle = 5.0f;
	/// <summary>
	/// Angle in degree (down compared to horizon) the camera will look at when at the farthest of the character
	/// </summary>
	public float MaxAngle = 45.0f;
	/// <summary>
	/// Distance at which the camera is from the character when at the closest zoom level
	/// </summary>
	public float MinDistance = 5.0f;
	/// <summary>
	/// Distance at which the camera is from the character when at the max zoom level
	/// </summary>
	public float MaxDistance = 45.0f;
	[HideInInspector]
	public float m_CurrentDistance = 1.0f;

	void Awake() {
		Instance = this;

	}

	void Start() {
		cinemachineBrain = GetComponent<CinemachineBrain>();
		vCams = new Stack<CinemachineVirtualCamera>();
		vCams.Push(vCam);
		Zoom(0);
	}

	/// <summary>
	/// Zoom of the given distance. Note that distance need to be a param between 0...1,a d the distance is a ratio
	/// </summary>
	/// <param name="distance">The distance to zoom, need to be in range [0..1] (will be clamped) </param>
	public void Zoom(float distance) {
		m_CurrentDistance = Mathf.Clamp01(m_CurrentDistance + distance);
		if (vCams.Peek() != null)
			vCams.Peek().m_Lens.FieldOfView = MinAngle + (MaxAngle - MinAngle) * m_CurrentDistance;
	}

	private void Update() {
		//if (ppv)
		//	DOF();
	}

	//void DOF() {
	//	DepthOfField pr;
	//	ppv.sharedProfile.TryGetSettings<DepthOfField>(out pr);
	//	var camPos = GameplayCamera.transform.position;
	//	var targetPos = vCams.Peek().LookAt.transform.position;
		
	//	pr.focusDistance.value = Vector3.Distance(targetPos, camPos);
	//}
}

