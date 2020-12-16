using UnityEngine;
using System.Collections;

using static App;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Game
{

	public static Game current;

	List<object> sav = new List<object>();

	[System.NonSerialized]
	Dictionary<System.Guid, Character> allCharacters;

	[System.NonSerialized]
	public Dictionary<System.Guid, GameObject> allGuidComponents;

	[System.NonSerialized]
	public Dictionary<System.Guid, InteractableObject> allInteractables;


	public Game() {
		allGuidComponents = GetAllGuidComponents();
	}


	public void Save() {
		SavePlayer();
		SaveScene();
	}

	public void SavePlayer() {
		SPlayer s = playerManager.Serialize();              // sérialiser les informations à sauvegarder
		SaveLoad.SaveData("player", new List<object>() { s });  // enregistrer le fichier
	}

	public void SaveScene() {
		sav = new List<object>();

		GameObject.Find("World").GetComponent<SceneSaver>().Serialize(sav);

		GuidComponent[] items = Object.FindObjectsOfType<GuidComponent>(); // tous les GuidComponents
		foreach (GuidComponent item in items) {
			if (ISave<Loot>(item, sav)) continue;                           // si l'objet est un Loot => sauver et passer à l'objet suivant
			if (ISave<Target>(item, sav)) continue;                         // si l'objet est un Target => sauver et passer à l'objet suivant
			if (!IsType<PlayerManager>(item))                               // l'item N'est PAS le joueur
				if (ISave<Character>(item, sav)) continue;                  // si l'objet est un Character => sauver et passer à l'objet suivant
		}

		string sceneName = sceneLoader.currentSceneName;
		SaveLoad.SaveData(sceneName, sav);    // enregistrer le fichier
	}

	public void LoadPlayer(List<object> data) {
		if (data.Count == 1) {
			playerManager.Deserialize(data[0]);
		}
	}

	public void LoadScene(List<object> data) {
		allGuidComponents = GetAllGuidComponents();
		SceneSaver[] sSavers = GameObject.FindObjectsOfType<SceneSaver>();

		foreach (SceneSaver sSaver in sSavers) {
			if (sSaver.gameObject.scene.name == ((SerializedScene)data[0]).id) {
				sSaver.Deserialize(data[0]);

				data.RemoveAt(0);
				foreach (SInteractable item in data) {
					ILoad(item);
				}
				break;
			}
		}
	}

	Dictionary<System.Guid, GameObject> GetAllGuidComponents() {
		Dictionary<System.Guid, GameObject> allGuidComponents = new Dictionary<System.Guid, GameObject>();
		GuidComponent[] items = Object.FindObjectsOfType<GuidComponent>(); // tous les GuidComponents
		foreach (GuidComponent item in items) {
			allGuidComponents.Add(item.GetGuid(), item.gameObject);
		}
		return allGuidComponents;
	}

	/// <summary>
	/// Sauvegarder un objet du type T
	/// </summary>
	/// <typeparam name="T">le type d'objet à sauvegarder</typeparam>
	/// <param name="item">l'identifiant unique</param>
	/// <param name="sav">la sauvegarde à incrémenter</param>
	/// <returns></returns>
	bool ISave<T>(GuidComponent item, List<object> sav) where T : ISave {
		T iItem = item.GetComponent<T>();       // rechercher un composant de type T sur ce gameObject
		if (iItem != null && iItem is T) {      // si cet item est bien de type T
			iItem.Serialize(sav);               //		sauvegarder ses infos
			return true;                        //		retourner 'vrai'
		} else {                                // sinon
			return false;                       //		retrouner 'faux'
		}
	}
	bool IsType<T>(GuidComponent item) {
		T iItem = item.GetComponent<T>();       // rechercher un composant de type T sur ce gameObject
		return (iItem != null && iItem is T);
	}

	void ILoad(SInteractable serialized) {
		var guid = new System.Guid(serialized.guid);
		InteractableObject iItem = current.allGuidComponents[guid].GetComponent<InteractableObject>();      // retrouver l'objet dans la scène (même guid)
		iItem.Deserialize(serialized);                                                                      // restaurer les valeurs sauvegardées
	}
}
