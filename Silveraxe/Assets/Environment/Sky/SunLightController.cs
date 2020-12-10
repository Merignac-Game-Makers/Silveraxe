using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// gestion de la lumière du soleil en fonction de l'altitude :
///     intensité et couleur
/// </summary>
public class SunLightController : MonoBehaviour
{
	[Serializable]
	public class SunState
	{
		public float altitude = 0f;
		public float intensity = 1;
		public Color color = new Color(1f, 1f, .8f);
	}

	float alt => transform.position.y;

	public Light sun;
	public Light moon;
	public Light moonspot1;
	public Light moonSpot2;
	public SunState[] sunStates;

	SunState s0, s1;

	void Start() {
		RenderSettings.sun = sun;
	}

	// Update is called once per frame
	void Update() {
		for (int i = 0; i < sunStates.Length-1; i++) {    // pour chaque point de passage
			s0 = sunStates[i];
			s1 = sunStates[i+1];
			if (alt > s0.altitude || alt < s1.altitude) {                 // si le soleil est au dessous du point de passage
				continue;
			}
			var diff = s0.altitude - s1.altitude;
			var k = (s0.altitude - alt) / diff;
			sun.intensity = Mathf.Lerp(s0.intensity, s1.intensity, k);
			sun.color = Color.Lerp(s0.color, s1.color, k);
			Vector3 sunDir = sun.transform.position.normalized;
			Vector3 moonDir = moon.transform.position.normalized;
			var cos = Vector3.Dot(moonDir, sunDir);
			moonSpot2.intensity = (1 + cos) / 2;//sun.intensity;
		}
	}
}

