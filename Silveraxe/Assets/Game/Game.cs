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
		SHeader();
		SaveScene();
		SavePlayer();
	}

	public void SHeader() {
		SHeader sHeader = new SHeader {					// sérialiser les informations à sauvegarder
			version = App.saveVersion,
			scene = App.currentSceneName
		};
		SaveLoad.SaveHeaderFile(sHeader);				 // enregistrer le fichier
	}


	public void SavePlayer() {
		SPlayer sPlayer = (SPlayer)playerManager.Serialize();    // sérialiser les informations à sauvegarder
		SaveLoad.SavePlayerFile(sPlayer);						 // enregistrer le fichier
	}

	public void SaveScene() {
		sav = new List<object>();

		sav.Add(Object.FindObjectOfType<SceneSaver>().Serialize());

		GuidComponent[] items = Object.FindObjectsOfType<GuidComponent>(); // tous les GuidComponents
		foreach (GuidComponent item in items) {
			if (ISave<Loot>(item, sav)) continue;                           // si l'objet est un Loot => sauver et passer à l'objet suivant
			if (ISave<Target>(item, sav)) continue;                         // si l'objet est un Target => sauver et passer à l'objet suivant
			if (!IsType<PlayerManager>(item))                               // l'item N'est PAS le joueur
				if (ISave<Character>(item, sav)) continue;                  // si l'objet est un Character => sauver et passer à l'objet suivant
		}

		SaveLoad.SaveSceneFile(App.currentSceneName, sav);                  // enregistrer le fichier
	}

	public void LoadHeader() {
		object data = SaveLoad.LoadHeaderFile();
		if (data !=null && data is SHeader) {
			SHeader header = data as SHeader;
			App.currentSceneName = header.scene;
		}
	}

	public void LoadPlayer() {
		object data = SaveLoad.LoadPlayerFile();
		if (data != null && data is SPlayer) {
			playerManager.Deserialize(data);
		}
	}

	public void LoadScene(string scene) {
		List<object> data = SaveLoad.LoadSceneFile(scene);
		allGuidComponents = GetAllGuidComponents();
		SceneSaver[] sSavers = Object.FindObjectsOfType<SceneSaver>();

		foreach (SceneSaver sSaver in sSavers) {
			if (sSaver.gameObject.scene.name == ((SerializedScene)data[0]).id && App.saveVersion == ((SerializedScene)data[0]).version) { // pour la scène à charger (si la sauvegarde est de la version courante)
				sSaver.Deserialize(data[0]);                                    // caractéristiques globales de la scène (heure, position et couleur du soleil,...)
				data.RemoveAt(0);                                               // retirer les caractéristiques globales de la scène

				foreach (SInteractable item in data) {                          // pour tous les objets de la scène
					ILoad(item);                                                //	restaurer les caractéristiques de l'objet
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
												//iItem.Serialize(sav);             //		sauvegarder ses infos
			sav.Add(iItem.Serialize());         //		sauvegarder ses infos
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
		InteractableObject iItem = Find<InteractableObject>(serialized);     // retrouver l'objet dans la scène (même guid)
		iItem.Deserialize(serialized);                                        // restaurer les valeurs sauvegardées
	}

	public static T Find<T>(SInteractable serialized) {
		var i = GuidManager.ResolveGuid(new System.Guid(serialized.guid)).GetComponent<T>();
		if (i==null) {
			Debug.Log("Guid not found : " + serialized);
		}
		return GuidManager.ResolveGuid(new System.Guid(serialized.guid)).GetComponent<T>();
		//var guid = new System.Guid(serialized.guid);
		//return current.allGuidComponents[guid].GetComponent<T>();
	}
	public static T Find<T>(System.Guid guid) {
		return GuidManager.ResolveGuid(guid).GetComponent<T>();
		//return current.allGuidComponents[guid].GetComponent<T>();
	}
	public static T Find<T>(byte[] guid) {
		return GuidManager.ResolveGuid(new System.Guid(guid)).GetComponent<T>();
		//return current.allGuidComponents[new System.Guid(guid)].GetComponent<T>();
	}
}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class SHeader
{
	public string version;      // version de sauvegarde
	public string scene;        // scène dans laquelle se trouve l'objet
}