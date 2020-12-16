using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutdoorSceneSaver : SceneSaver
{

	public Horloge horloge;

	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	/// <param name="sav">la sauvegarde en cours d'élaboration</param>
	public override void Serialize(List<object> sav) {
		int idx = App.sceneLoader.currentLevelIndex;
		sav.Add(new OutdoorScene() {
			id = App.sceneLoader.currentSceneName,          // nom de scene
			datetime = horloge.sceneDateTime				// heure
		});
	}


	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		if (serialized is OutdoorScene) {
			OutdoorScene s = serialized as OutdoorScene;
			horloge.SetDateTime(s.datetime);
		}
	}
}

	/// <summary>
	/// Classe pour la sauvegarde
	/// </summary>
	[System.Serializable]
    public class OutdoorScene : SerializedScene
    {
		public DateTime datetime;
	}