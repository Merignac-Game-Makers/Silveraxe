using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class Game
{

	public static Game current;

	System.Guid guid = System.Guid.NewGuid();
	List<object> sav = new List<object>();
	//List<string> buildScenes;

	//Dictionary<string, byte[]> savedScenesID;

	///// <summary>
	///// Liste des noms des scenes du build
	///// </summary>
	///// <returns></returns>
	//public static List<string> BuildScenes() {
	//	List<string> scenes = new List<string>();
	//	foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
	//		if (scene.enabled)
	//			scenes.Add(Path.GetFileNameWithoutExtension(scene.path));
	//	}
	//	return scenes;
	//}

	///// <summary>
	///// Créer un ID unique pour chaque scène du build
	///// </summary>
	///// <returns></returns>
	//private static Dictionary<string, byte[]> NewIDs() {
	//	if (current == null)
	//		current = new Game();

	//	Dictionary<string, byte[]> iDs = new Dictionary<string, byte[]>();
	//	foreach (string scene in current.buildScenes) {
	//		iDs.Add(scene, System.Guid.NewGuid().ToByteArray());
	//	}

	//	return iDs;
	//}

	public Game() {
		current = this;
		//buildScenes = BuildScenes();
		//savedScenesID = NewIDs();
	}

	#region API

	/// <summary>
	/// démarrer un nouveau jeu
	/// </summary>
	public static void NewGame() {
		current = new Game();
	}

	public static void Save() {
		if (current == null) new Game();

		PauseGame();									// mise en pause

		current.SaveHeader();							// sauvegarde du header
		current.SaveSceneData(App.dontDestroyScene);	// sauvegarde de la scène commune
		current.SaveSceneData(App.currentSceneName);	// sauvegarde de la scène courante

		ResumeGame();									// fin de pause
	}

	public static void Load() {
		if (current == null) new Game();

		PauseGame();

		current.LoadHeader();                                       // charger le nom de la scène courante
		App.sceneLoader.LoadScene(App.currentSceneName, true);      // charger la scène (true = restaurer l'état des objets d'après la sauvegarde)
	}

	/// <summary>
	/// Restaurer l'état d'une scène après son chargement
	/// </summary>
	/// <param name="scene"></param>
	public static void Restore(string scene) {
		if (current == null) new Game();

		App.playerManager.StartCoroutine(IRestore());

		IEnumerator IRestore() {
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));    // la scène à restaurer devient la scène active
			App.currentSceneName = scene;                                       // référence statique au nom de la scène
			yield return new WaitForEndOfFrame();                               // attendre 1 frame pour que le Start des GameObjects soit passé après le chargement de la scène

			current.LoadSceneData(App.dontDestroyScene);                        // restaurer l'état du joueur et des objets trans-scènes
			current.LoadSceneData(scene);                                       // restaurer l'état de la scène
			if (App.sceneCrossing) {                                            // si on est sur un portail
				App.playerManager.transform.position = App.crossScenePosition;  //		placer le joueur sur le portail d'arrivée
				App.playerManager.transform.rotation = Quaternion.identity;
				Save();															//		sauvegarder la scène d'arrivée
			}

			ResumeGame();                                                       // relancer l'exécution du jeu (fin de pause, ...)
		}
	}

	public static T Find<T>(SSavable serialized) {
		var i = GuidManager.ResolveGuid(new System.Guid(serialized.guid)).GetComponent<T>();
		if (i == null) {
			Debug.Log("Guid not found : " + serialized);
		}
		return GuidManager.ResolveGuid(new System.Guid(serialized.guid)).GetComponent<T>();
	}
	public static T Find<T>(System.Guid guid) {
		return GuidManager.ResolveGuid(guid).GetComponent<T>();
	}
	public static T Find<T>(byte[] guid) {
		return GuidManager.ResolveGuid(new System.Guid(guid)).GetComponent<T>();
	}

	/// <summary>
	/// mise en pause
	/// </summary>
	public static void PauseGame() {
		if (App.playerManager.navAgent && App.playerManager.navAgent.enabled) {
			App.playerManager.StopAgent();
			App.playerManager.navAgent.enabled = false;
		}
		Time.timeScale = 0;
	}

	/// <summary>
	/// fin de pause
	/// </summary>
	public static void ResumeGame() {
		CrossSceneItemsManager.SetActiveScene(App.currentSceneName);        // activer les objets 'trans-scène' 
		App.itemsManager.GetSceneObjects();                                 // régénérer la liste des objets intéractibles
		App.targetsManager.GetSceneTargets();                               // régénérer la liste des cibles de dépôt d'objets intéractibles

		Time.timeScale = 1;													// fin de pause
		if (App.playerManager.navAgent) {									// redémarrer la navigation du joueur
			App.playerManager.navAgent.enabled = true;
		}
		App.isLoadingData = false;											// flag 'fin de chargement de sauvegarde'

	}


	#endregion


	/// <summary>
	/// Enregistrer le Sommaire de la sauvegarde (version, scène courante, IDs des sauvegardes de scènes)
	/// </summary>
	public void SaveHeader() {
		SHeader sHeader = new SHeader {                 // sérialiser les informations à sauvegarder
			version = App.saveVersion,
			scene = App.currentSceneName,
			gameUID = guid.ToByteArray()
		};
		SaveLoad.SaveHeaderFile(sHeader);                // enregistrer le fichier
	}

	/// <summary>
	/// Sauvegarder un scène 
	/// </summary>
	/// <param name="scene"></param>
	public void SaveSceneData(string scene, bool init = false) {
		sav = new List<object>();

		string filename = scene;                                            // nom du fichier de sauvegarde

		if (init) {                                                         // si on sauvegarde l'état initial (pour préparer le choix 'nouvelle partie')
			filename += "Init";                                             //		ajouter "Init" au nom de fichier
			sav.Add(System.Guid.Empty.ToByteArray());                       //		l'UID de la sauvegarde est vide
		} else {                                                            // sinon
			sav.Add(guid.ToByteArray());                                    //		l'UID de la sauvegarde est ceui du jeu courant
		}

		if (scene != App.dontDestroyScene)                                  // si la scène à sauver n'EST PAS "dontDestroy"
			sav.Add(Object.FindObjectOfType<SceneSaver>().Serialize());     //		sérialiser les caractéristiques globales de la scène

		List<GameObject> items = GuidManager.GetAll(scene);                 // récupérer la liste des objets à sérialiser
		foreach (GameObject item in items) {                                // pour chacun d'entre eux
			if (ISave<Loot>(item, sav)) continue;                           //	si l'objet est un Loot => sauver et passer à l'objet suivant
			if (ISave<Target>(item, sav)) continue;                         //	si l'objet est un Target => sauver et passer à l'objet suivant
			if (ISave<Character>(item, sav)) continue;                      //	si l'objet est un Character => sauver et passer à l'objet suivant
			if (ISave<Savable>(item, sav)) continue;                        //	si l'objet est un Savable => sauver et passer à l'objet suivant
		}

		SaveLoad.SaveSceneFile(filename, sav);                              // enregistrer le fichier
	}

	public void LoadHeader() {
		object data = SaveLoad.LoadHeaderFile();
		if (data != null && data is SHeader) {
			SHeader header = data as SHeader;
			App.currentSceneName = header.scene;
			guid = new System.Guid(header.gameUID);
		}
	}

	public void LoadSceneData(string scene, bool init = false) {
		string filename = scene;                                        // le nom du fhchier à charger 
		System.Guid sGuid = guid;                                       // l'UID du jeu courant

		if (init) {                                                     // si on restaure le jeu dans son état initial (nouvelle partie)
			filename += "Init";                                         //		ajouter "Init" au nom de fichier
			sGuid = System.Guid.Empty;                                  //		l'UID est vide
		}

		List<object> data = SaveLoad.LoadSceneFile(filename);           // lire le ficher de sauvegarde => data
		if (data != null) {
			System.Guid dataGameUID = new System.Guid(data[0] as byte[]);       // récupérer l'UID de la sauvegarde

			if (dataGameUID.Equals(sGuid)) {                                    // si l'UID de la sauvegarde correspond bien à l'UID à charger
				data.RemoveAt(0);                                               // retirer l'UID

				if (scene == App.dontDestroyScene) {
					foreach (SSavable item in data) {                          // pour tous les objets de la scène
						ILoad(item);                                                //	restaurer les caractéristiques de l'objet
					}
				} else {
					SceneSaver[] sSavers = Object.FindObjectsOfType<SceneSaver>();

					foreach (SceneSaver sSaver in sSavers) {
						if (sSaver.gameObject.scene.name == ((SerializedScene)data[0]).name && App.saveVersion == ((SerializedScene)data[0]).version) { // pour la scène à charger (si la sauvegarde est de la version courante)
							sSaver.Deserialize(data[0]);                                    // caractéristiques globales de la scène (heure, position et couleur du soleil,...)
							data.RemoveAt(0);                                               // retirer les caractéristiques globales de la scène

							foreach (SSavable item in data) {                          // pour tous les objets de la scène
								ILoad(item);                                                //	restaurer les caractéristiques de l'objet
							}
							break;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Sauvegarder un objet du type T
	/// </summary>
	/// <typeparam name="T">le type d'objet à sauvegarder</typeparam>
	/// <param name="item">l'identifiant unique</param>
	/// <param name="sav">la sauvegarde à incrémenter</param>
	/// <returns></returns>
	bool ISave<T>(GameObject item, List<object> sav) where T : ISave {
		T iItem = item.GetComponent<T>();       // rechercher un composant de type T sur ce gameObject
		if (iItem != null && iItem is T) {      // si cet item est bien de type T
			sav.Add(iItem.Serialize());         //		sauvegarder ses infos
			return true;                        //		retourner 'vrai'
		} else {                                // sinon
			return false;                       //		retrouner 'faux'
		}
	}
	bool IsType<T>(GameObject item) {
		T iItem = item.GetComponent<T>();       // rechercher un composant de type T sur ce gameObject
		return (iItem != null && iItem is T);
	}

	void ILoad(SSavable serialized) {
		Savable iItem = Find<Savable>(serialized);     // retrouver l'objet dans la scène (même guid)
		iItem.Deserialize(serialized);                                        // restaurer les valeurs sauvegardées
	}

}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class SHeader
{
	public string version;                              // version de sauvegarde
	public string scene;                                // scène dans laquelle se trouve le joueur
	public byte[] gameUID;                              // IDs des sauvegardes de scène
}

