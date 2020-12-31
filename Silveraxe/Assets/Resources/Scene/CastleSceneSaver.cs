using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleSceneSaver : SceneSaver
{


	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	/// <param name="sav">la sauvegarde en cours d'élaboration</param>
	//public override void Serialize(List<object> sav) {
	//	sav.Add(new CastleScene() {
	//		id = App.sceneLoader.currentSceneName,         // nom de scene
	//	});
	//}
	//public override SerializedScene Serialize() {
	//	var result = new CastleScene().Copy(base.Serialize());
	//	result.id = App.sceneLoader.currentSceneName,         // nom de scene
	//	return result;
	//}


	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		if (serialized is CastleScene) {
			CastleScene s = serialized as CastleScene;
		}
	}
}

	/// <summary>
	/// Classe pour la sauvegarde
	/// </summary>
	[System.Serializable]
    public class CastleScene : SerializedScene
    {
	}