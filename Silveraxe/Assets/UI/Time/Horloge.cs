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
	float prevValue = 0;

	public DateTime sceneDateTime;
	public DateTime sceneStartDateTime = new DateTime();
	float elapsedDays = 0;
	float currentHour;


	float sunTime = 0;

	private void Start() {
		skyAnimator.speed = 0f;


		speedFactor.text = daySpeed.ToString();
		sceneDateTime = sceneStartDateTime;
	}

	void Update() {
		//skyAnimator.speed = daySpeed;
		sunTime += Time.deltaTime * daySpeed / 100;
		skyAnimator.SetFloat("SunTime", sunTime);
		skyAnimator.SetFloat("MoonTime", sunTime*1.05f);

		rotation.z = -horlogeValue;
		spinner.rotation = Quaternion.Euler(rotation);
		for (int i = 0; i < spinner.childCount; i++) {
			spinner.GetChild(i).rotation = Quaternion.Euler(Vector3.zero);
		}

		var nh = horlogeValue;
		if (nh < prevValue) nh += 360;
		var deltaTicks = (nh - prevValue) / 360 * 24 * 60 * 60;
		sceneDateTime = sceneDateTime.AddSeconds(deltaTicks);

		//Debug.Log(sceneDateTime.ToString("yy/MM/dd HH:mm"));
		if (horlogeValue < prevValue)			// si on a fait 1 tour
			elapsedDays += 1;					// ajouter 1 jour
		currentHour = 24 * 360 / horlogeValue;	// heure du jour
		prevValue = horlogeValue;
	}

	public void SetDateTime(DateTime dateTime) {
		sceneDateTime = dateTime;
		sunTime = (float)(dateTime.TimeOfDay.TotalDays) ;
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
