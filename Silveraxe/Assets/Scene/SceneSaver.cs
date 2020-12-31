using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneSaver : MonoBehaviour
{

	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	public virtual SerializedScene Serialize() {
		return new SerializedScene() {
			version = App.saveVersion,                      // version de sauvegarde
			id = App.currentSceneName,                      // nom de scene
		};
	}


	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public virtual void Deserialize(object serialized) {
		if (serialized is SerializedScene) {
			SerializedScene s = serialized as SerializedScene;
		}
	}

	public virtual void Pause(bool pause) { 
	}
}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class SerializedScene
{
	public string version = "";
	public string id;         // nom de la scène
}
