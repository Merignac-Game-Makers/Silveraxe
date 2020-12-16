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
	/// <param name="sav">la sauvegarde en cours d'élaboration</param>
	public virtual void Serialize(List<object> sav) {

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
}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public abstract class SerializedScene
{
	public string id;         // nom de la scène
}
