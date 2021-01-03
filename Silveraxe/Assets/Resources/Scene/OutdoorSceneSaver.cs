using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutdoorSceneSaver : SceneSaver
{
	public Horloge horloge;
	public Light sunLight;

	/// <summary>
	/// Sérialiser les infos à sauvegarder 
	/// </summary>
	public override SerializedScene Serialize() {
		var result = new OutdoorScene().Copy(base.Serialize());
		result.daySpeed = horloge.daySpeed;					// vitesse d'horloge
		result.sunTime = horloge.GetSunTime();              // heure
		result.sunIntensity = sunLight.intensity;           // intensité de la lumière solaire
		result.sunColor = sunLight.color.ToArray();         // couleur de la lumière solaire
		return result;
	}

	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		if (serialized is OutdoorScene) {
			OutdoorScene s = serialized as OutdoorScene;
			horloge.SetDaySpeed(s.daySpeed);
			horloge.SetDateTime(s.sunTime);
			sunLight.intensity = s.sunIntensity;
			sunLight.color = s.sunColor.ToColor();
		}
	}
}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class OutdoorScene : SerializedScene
{
	public float daySpeed;
	public float sunTime;
	public float sunIntensity;
	public float[] sunColor;
}