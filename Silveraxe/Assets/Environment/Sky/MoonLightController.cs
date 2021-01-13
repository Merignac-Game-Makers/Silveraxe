using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// gestion de la lumière du soleil en fonction de l'altitude :
///     intensité et couleur
/// </summary>
public class MoonLightController : MonoBehaviour {

	float alt => transform.position.y;

	public Light sun;
	public Light moon;

	public bool paused { get; set; }

	void Start() {
		RenderSettings.sun = sun;
	}

	// Update is called once per frame
	void Update() {
		if (!paused) {
			Vector3 sunDir = sun.transform.position.normalized;             // direction du soleil
			Vector3 moonDir = moon.transform.position.normalized;           // direction de la lune
			var cos = Vector3.Dot(moonDir, sunDir);                         // le cosinus de l'angle entre 'direction de la lune' et 'direction du solei'
			moon.intensity = .1f + .2f * (1 - cos) / 2;									// la part éclairée de lune vu de la Terre
		}
	}
}

