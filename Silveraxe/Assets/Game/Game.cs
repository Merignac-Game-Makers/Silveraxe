using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class Game {

	public static Game current;
	public static List<string> buildScenes;

	System.Guid guid = System.Guid.NewGuid();
	List<object> sav = new List<object>();

	/// <summary>
	/// Liste des noms des scenes du build
	/// </summary>
	/// <returns></returns>
	public static List<string> BuildScenes() {
		List<string> scenes = new List<string>();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (scene.enabled)
				scenes.Add(Path.GetFileNameWithoutExtension(scene.path));
		}
		return scenes;
	}

	public Game() {
		current = this;
		buildScenes = BuildScenes();
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

		PauseGame();                                    // mise en pause
		current.SaveGame();
		ResumeGame();                                   // fin de pause
	}

	public static void Load() {
		if (current == null) new Game();

		PauseGame();
		current.LoadGame();
		ResumeGame();                                                       // relancer l'exécution du jeu (fin de pause, ...)
	}

	/// <summary>
	/// Restaurer l'état du jeu après son chargement
	/// </summary>
	/// <param name="scene"></param>
	public static void RestoreGame() {
		if (current == null) new Game();
		ResumeGame();                                                       // relancer l'exécution du jeu (fin de pause, ...)
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
		App.playerManager.StopAgent(true);
		Time.timeScale = 0;
	}

	/// <summary>
	/// fin de pause
	/// </summary>
	public static void ResumeGame() {

		Time.timeScale = 1;                                                 // fin de pause
		App.playerManager.StopAgent(false);                                 // redémarrer la navigation du joueur
	}


	#endregion

	/// <summary>
	/// Sauvegarde du jeu 
	/// (version 'OnlyOneScene')
	/// </summary>
	/// <param name="init"></param>
	public void SaveGame(bool init = false) {
		sav = new List<object>();

		string filename = "game";											// nom du fichier de sauvegarde

		if (init) {                                                         // si on sauvegarde l'état initial (pour préparer le choix 'nouvelle partie')
			filename += "Init";                                             //		ajouter "Init" au nom de fichier
		} 

		List<GameObject> items = GuidManager.GetAll();						// récupérer la liste des objets à sérialiser
		foreach (GameObject item in items) {                                // pour chacun d'entre eux
			if (ISave<Loot>(item, sav)) continue;                           //	si l'objet est un Loot => sauver et passer à l'objet suivant
			if (ISave<Target>(item, sav)) continue;                         //	si l'objet est un Target => sauver et passer à l'objet suivant
			if (ISave<Character>(item, sav)) continue;                      //	si l'objet est un Character => sauver et passer à l'objet suivant
			if (ISave<Savable>(item, sav)) continue;                        //	si l'objet est un Savable => sauver et passer à l'objet suivant
		}

		SaveLoad.SaveSceneFile(filename, sav);                              // enregistrer le fichier
	}

	public void LoadGame(bool init = false) {
		string filename = "game";											// le nom du fhchier à charger 
		//System.Guid sGuid = guid;											// l'UID du jeu courant

		if (init) {															// si on restaure le jeu dans son état initial (nouvelle partie)
			filename += "Init";												//		ajouter "Init" au nom de fichier
		}

		List<object> data = SaveLoad.LoadSceneFile(filename);				// lire le ficher de sauvegarde => data
		if (data != null) {
			foreach (SSavable item in data) {								// pour tous les objets de la scène
				ILoad(item);                                                //	restaurer les caractéristiques de l'objet
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
public class SHeader {
	public string version;                              // version de sauvegarde
	public byte[] gameUID;                              // IDs des sauvegardes de scène
}

