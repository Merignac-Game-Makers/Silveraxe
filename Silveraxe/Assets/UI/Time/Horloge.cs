using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class Horloge : MonoBehaviour {
	public Days days;

	public Animator skyAnimator;
	public Transform spinner;

	public TMP_Text hour;

	public int startHour;

	Vector3 rotation = Vector3.zero;
	[HideInInspector]
	public float horlogeValue;  // updated by animator [0..360]
	float prevValue = 0;

	public DateTime sceneDateTime;
	public DateTime sceneStartDateTime;
	int elapsedDays = 0;
	float currentHour;
	float prevHour;

	int daySpeed = 1;
	float sunTime = 0;

	private void Start() {
		sceneStartDateTime = new DateTime().AddHours(startHour);
		SetDateTime(sceneStartDateTime);
		skyAnimator.speed = 0f;
		daySpeed = 1;
		skyAnimator.SetFloat("SunTime", sunTime);
		skyAnimator.SetFloat("MoonTime", sunTime * 1.05f);

		days.days = days.GetComponentsInChildren<Day>(true);
		days.SetDay(0);

	}

	void Update() {
		sunTime += Time.deltaTime / 500;
		skyAnimator.SetFloat("SunTime", sunTime);
		skyAnimator.SetFloat("MoonTime", sunTime * 1.15f);


		rotation.z = -horlogeValue;
		spinner.rotation = Quaternion.Euler(rotation);
		for (int i = 0; i < spinner.childCount; i++) {
			spinner.GetChild(i).rotation = Quaternion.Euler(Vector3.zero);
		}

		var nh = horlogeValue;
		if (nh < prevValue) nh += 360;
		var deltaSeconds = (nh - prevValue) * 240;		// nombre de secondes écoulées depuis l'image précédente
		sceneDateTime = sceneDateTime.AddSeconds(deltaSeconds);

		////Debug.Log(sceneDateTime.ToString("yy/MM/dd HH:mm"));
		//if (horlogeValue < prevValue)						// si on a fait 1 tour
		//	elapsedDays += 1;                               // ajouter 1 jour

		currentHour = (14 + 24 * horlogeValue / 360) % 24;  // heure du jour
		if (currentHour < prevHour) {
			elapsedDays++;
			days.SetDay(elapsedDays);
		}

		prevHour = currentHour;
		prevValue = horlogeValue;

		var ch = (int)(currentHour * 4) / 4f + 1 / 60f;
		var ts = new TimeSpan((long)(ch * 36000000000));
		hour.text = ts.ToString(@"hh\:mm", CultureInfo.CurrentCulture);

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			//speedFactor.text = "x2";
			Time.timeScale = 2;
		} else if (Input.GetKeyUp(KeyCode.LeftShift)) {
			ToggleTime(daySpeed);
		}
	}

	public void SetDateTime(DateTime dateTime) {
		sceneDateTime = dateTime;
		sunTime = dateTime.AddHours(10).Ticks / 10000000f / 60 / 60 / 24;
	}
	public void SetDateTime(float stime) {
		//sceneDateTime = new DateTime().AddSeconds(stime);
		sunTime = stime;
	}

	public DateTime GetSceneDateTime() {
		return new DateTime(1, 1, 1).AddSeconds(sunTime);
	}

	public float GetSunTime() {
		return sunTime;
	}

	// Boutton accélération du temps
	public void ToggleTime(int speed) {
		daySpeed = speed;
		//speedFactor.text = "x" + daySpeed.ToString();
		Time.timeScale = speed;
	}

}
