using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamZone : MonoBehaviour
{
	public Cinemachine.CinemachineVirtualCamera vCam;
	public MeshRenderer helper;

	GameObject player;
	CamZones camZones;

	Color colorOff = new Color(1f, 1f, 1f, .05f);
	Color colorOn = new Color(1f, 1f, 1f, .5f);

	public void Awake() {
		ToggleCamera(false);
	}

	// Start is called before the first frame update
	void Start() {
		player = PlayerManager.Instance.gameObject;
		camZones = GetComponentInParent<CamZones>();
	}

	// Update is called once per frame
	void Update() {

	}

	public void OnTriggerEnter(Collider other) {
		if (other.gameObject == player) {
			camZones.SwapCams(this);
		}
	}

	public void ToggleCamera(bool on) {
		vCam.gameObject.SetActive(on);
		if (helper) {
			if (on)
				helper.material.SetColor("_Color", colorOn);
			else
				helper.material.SetColor("_Color", colorOff);
		}

	}
}
