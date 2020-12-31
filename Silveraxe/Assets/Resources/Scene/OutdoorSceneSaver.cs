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
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	/// <param name="sav">la sauvegarde en cours d'élaboration</param>
	//public override void Serialize(List<object> sav) {
	//	int idx = App.sceneLoader.currentLevelIndex;
	//	sav.Add(new OutdoorScene() {
	//		id = App.sceneLoader.currentSceneName,          // nom de scene
	//		sunTime = horloge.GetSunTime() ,				// heure
	//		sunIntensity = sunLight.intensity,				// intensité de la lumière solaire
	//		sunColor = sunLight.color.ToArray()				// couleur de la lumière solaire
	//	});
	//}
	public override SerializedScene Serialize() {
		var result = new OutdoorScene().Copy(base.Serialize());
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
			Pause(true);
			horloge.SetDateTime(s.sunTime);
			sunLight.intensity = s.sunIntensity;
			sunLight.color = s.sunColor.ToColor();
			Pause(false);
		}
	}

	public override void Pause(bool pause) {
		horloge.Pause(pause);
	}
}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class OutdoorScene : SerializedScene
{
	public float sunTime;
	public float sunIntensity;
	public float[] sunColor;
}