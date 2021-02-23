using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBeam : MonoBehaviour
{
	public GameObject glass;
	public Light sun;
	public GameObject pivot;
	public Light spot;


	Vector3 sunDir;
	Vector3 beamDir = Vector3.zero;

	float intensity;
	void Start() {
	}

	void Update() {
		sunDir = sun.transform.TransformDirection(Vector3.forward);
		beamDir = Vector3.Reflect(Vector3.Reflect(sunDir, glass.transform.forward), glass.transform.right);
		pivot.transform.rotation = Quaternion.FromToRotation(Vector3.forward, beamDir);
		spot.color = sun.color;
		if (sunDir.x < 0)
			spot.intensity = sun.intensity * 4;
		else
			spot.intensity = 0;
	}
}
