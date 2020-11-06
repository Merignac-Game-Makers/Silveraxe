using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

	public Transform objectTransform;
	[Range(0.01f, 1.0f)]
	public float SmoothFactor = 0.5f;
	public float RotationsSpeed = 5.0f;

	float angleX = 0f;
	float ax, az;

	void LateUpdate() {

		if (Input.GetMouseButton(0)) {
			ax = objectTransform.rotation.eulerAngles.x;
			az = objectTransform.rotation.eulerAngles.z;
			angleX -= Input.GetAxis("Mouse X") * RotationsSpeed;
			objectTransform.rotation = Quaternion.Euler(ax, angleX, az) * transform.rotation;
			//objectTransform.Rotate(0, -Input.GetAxis("Mouse X") * RotationsSpeed, 0);
		}

	}
}
