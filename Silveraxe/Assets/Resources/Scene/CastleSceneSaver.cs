using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleSceneSaver : SceneSaver
{

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