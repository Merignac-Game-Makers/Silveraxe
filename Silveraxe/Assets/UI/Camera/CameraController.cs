using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

using static App;

/// <summary>
/// Control the camera, mainly used as a reference to the main camera through the singleton instance, and to handle
/// mouse wheel zooming
/// </summary>
public class CameraController : MonoBehaviour
{
	public Camera GameplayCamera;
	public CinemachineVirtualCamera vCamFollow;
	public CinemachineVirtualCamera vCamDialogue;
	public GameObject CameraTarget;

	CinemachineVirtualCamera activeCamera => cinemachineBrain?.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

	CinemachineBrain cinemachineBrain;

	[Header("Camera Settings")]
	// Angle in degree (down compared to horizon) the camera will look at when at the closest of the character
	public float MinAngle = 5.0f;
	// Angle in degree (down compared to horizon) the camera will look at when at the farthest of the character
	public float MaxAngle = 45.0f;
	// Distance at which the camera is from the character when at the closest zoom level
	public float MinDistance = 5.0f;
	// Distance at which the camera is from the character when at the max zoom level
	public float MaxDistance = 45.0f;
	public float m_CurrentDistance { get; set; } = 1.0f;

	void Awake() {
		cameraController = this;
	}

	void Start() {
		cinemachineBrain = GetComponent<CinemachineBrain>();
		SetCamera(vCamFollow);
		Zoom(0);
	}

	/// <summary>
	/// Zoom of the given distance. Note that distance need to be a param between 0...1,a d the distance is a ratio
	/// </summary>
	/// <param name="distance">The distance to zoom, need to be in range [0..1] (will be clamped) </param>
	public void Zoom(float distance) {
		m_CurrentDistance = Mathf.Clamp01(m_CurrentDistance + distance);
		if (activeCamera)
			activeCamera.m_Lens.FieldOfView = MinAngle + (MaxAngle - MinAngle) * m_CurrentDistance;
	}

	public void SetCamera(CinemachineVirtualCamera vCam) {
		vCamFollow?.gameObject.SetActive(vCam == vCamFollow);
		vCamDialogue?.gameObject.SetActive(vCam == vCamDialogue);
	}
}

