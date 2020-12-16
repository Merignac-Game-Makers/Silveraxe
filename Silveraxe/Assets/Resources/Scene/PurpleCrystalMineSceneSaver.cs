using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PurpleCrystalMineSceneSaver : SceneSaver
{


	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	/// <param name="sav">la sauvegarde en cours d'élaboration</param>
	public override void Serialize(List<object> sav) {
		sav.Add(new PurpleCrystalMineScene() {
			id = App.sceneLoader.currentSceneName,         // nom de scene
		});
	}


	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		if (serialized is PurpleCrystalMineScene) {
			PurpleCrystalMineScene s = serialized as PurpleCrystalMineScene;
		}
	}
}

	/// <summary>
	/// Classe pour la sauvegarde
	/// </summary>
	[System.Serializable]
    public class PurpleCrystalMineScene : SerializedScene
    {
	}