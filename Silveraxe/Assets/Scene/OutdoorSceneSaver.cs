using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutdoorSceneSaver : MonoBehaviour
{

	public Horloge horloge;

	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	/// <param name="sav">la sauvegarde en cours d'élaboration</param>
	public void Serialize(List<object> sav) {
		int idx = App.sceneLoader.currentLevelIndex;
		sav.Add(new SerializedScene() {
			id = SceneManager.GetSceneAt(idx).name,                                                  // identifiant unique
			datetime = horloge.sceneDateTime
		});
	}


	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public void Deserialize(object serialized) {
		if (serialized is SerializedScene) {
			SerializedScene s = serialized as SerializedScene;
			horloge.SetDateTime(s.datetime);
		}
	}




	/// <summary>
	/// Classe pour la sauvegarde
	/// </summary>
	[System.Serializable]
    public class SerializedScene : SScene
    {
		public DateTime datetime;
	}
}


