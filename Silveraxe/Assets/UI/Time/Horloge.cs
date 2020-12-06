using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Horloge : MonoBehaviour
{
	public float daySpeed = 1f;

	public Animator skyAnimator;
	public Transform spinner;

	public TMP_InputField speedFactor;

	Vector3 rotation = Vector3.zero;
	public float horlogeValue;  // updated by animator

	private void Start() {
		speedFactor.text = daySpeed.ToString();
	}

	void Update() {
		skyAnimator.speed = daySpeed;
		rotation.z = -horlogeValue;
		spinner.rotation = Quaternion.Euler(rotation);
		for (int i = 0; i < spinner.childCount; i++) {
			spinner.GetChild(i).rotation = Quaternion.Euler(Vector3.zero);
		}
	}

	public void SetDaySpeed(string speed) {
		try {
			daySpeed = float.Parse(speed);
		}
		catch (Exception e) {
			Debug.LogError(e.Message);
			speedFactor.text = daySpeed.ToString();
		}
	}
}
