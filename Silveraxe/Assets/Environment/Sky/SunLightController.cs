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

	public bool paused { get; set; }

	SunState s0, s1;

	void Start() {
		RenderSettings.sun = sun;
	}

	// Update is called once per frame
	void Update() {
		if (!paused) {
			for (int i = 0; i < sunStates.Length - 1; i++) {                    // pour chaque zone 'couleur/intensité' - 1
				s0 = sunStates[i];												// zone courante
				s1 = sunStates[i + 1];											// zone suivante
				if (alt > s0.altitude || alt < s1.altitude) {                   // si le soleil est au dessous du point de passage de la zone 'couleur/intensité' courante
					continue;													//		passer à la zone suivante
				}
				var diff = s0.altitude - s1.altitude;							// hauteur de la zone 'couleur/intensité' courante
				var k = (s0.altitude - alt) / diff;								// % d'avancement dans la zone
				sun.intensity = Mathf.Lerp(s0.intensity, s1.intensity, k);		// intensité du soleil (plus intense au zénith, moins intense près de l'horizon)
				sun.color = Color.Lerp(s0.color, s1.color, k);					// couleur du soleil (orangé près de l'horizon, ...)
				Vector3 sunDir = sun.transform.position.normalized;				// direction du soleil
				Vector3 moonDir = moon.transform.position.normalized;			// direction de la lune
				var cos = Vector3.Dot(moonDir, sunDir);							// le cosinus de l'angle entre 'direction de la lune' et 'direction du solei'
				moonSpot2.intensity = (1 + cos) / 2;							// la part de lumière du soleil réfléchie par la Terre qui éclaire la lune
			}
		}
	}
}

