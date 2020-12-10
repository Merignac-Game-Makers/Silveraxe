using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBeam : MonoBehaviour
{

	public Light sun;
	public GameObject pivot;
	public Light spot;


	Vector3 sunDir;
	Vector3 newDir = Vector3.zero;

	float intensity;
	void Start() {

	}

	void Update() {
		sunDir = sun.transform.TransformDirection(Vector3.forward);
		newDir = Vector3.Reflect(Vector3.Reflect(sunDir, transform.right), transform.forward);
		pivot.transform.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
		spot.color = sun.color;
		if (sunDir.x < 0)
			spot.intensity = sun.intensity * 5;
		else
			spot.intensity = 0;
	}
}
