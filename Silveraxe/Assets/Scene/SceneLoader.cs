using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System;

public class SceneLoader : MonoBehaviour
{
	public string defaultScene;

	private void Awake() {
		App.sceneLoader = this;
	}

	/// <summary>
	/// Début du jeu
	/// </summary>
	private void Start() {
		ResourcesManager.Init();										// sans doute devenu inutile
		//CrossSceneItemsManager.SetActiveScene(defaultScene);			// activer les objets trans-scènes de la scène par défaut 
		Game.NewGame();													// créer un nouvel identifiant de sauvegarde pour une nouvelle partie
		Game.current.SaveSceneData(App.dontDestroyScene, true);         // sauvegarder l'état initial de la scène "NeverUnload" (true = flag "etat initial" => nom de fichier spécifique)
		App.currentSceneName = defaultScene;		                    // la scène courante devient la scène chargée
		LoadScene(defaultScene, false);									// charger la scène de départ du jeu  (false => sans restaurer de sauvegarde)
	}

	/// <summary>
	/// démarrer une nouvelle partie 
	///		=> abandon de la partie en cours et réinitialisation à l'état de départ du jeu
	/// </summary>
	public void Restart() {
		if (SceneManager.sceneCount > 1) {								// on vérifie qu'il y a plus d'une scène chargée (pour éviter une erreur 2 lignes plus bas)
			Game.PauseGame();											// mettre le jeu en pause (pour éviter l'exécution des 'FixedUpdate' qui pourraient requérir des objets supprimés par la réinitialisation)
			UnloadScene(SceneManager.GetSceneAt(1).name, () => {		// décharger la scène courante, puis :
				Game.current.LoadSceneData(App.dontDestroyScene, true); //		réinitialiser la scène 'NeverUnload'  (true => dans son état de départ)
				App.currentSceneName = defaultScene;					//		la scène courante devient la scène chargée
				LoadScene(defaultScene, false);							//		charger la scène de départ du jeu  (false => sans restaurer de sauvegarde)
				Game.NewGame();											//		créer un nouvel identifiant de sauvegarde pour la nouvelle partie
				foreach (string scene in Game.buildScenes) {
					SaveLoad.DeleteSceneFile(scene);
				}
			});
		}
	}

	/// <summary>
	/// Charger une scène Unity
	/// Puis, si demandé, restaurer l'état des objets de la scène d'après une sauvegarde 
	/// </summary>
	/// <param name="scene">La scène à charger</param>
	/// <param name="restore">faut-il restaurer d'après la sauvegarde ?</param>
	public void LoadScene(string scene, bool restore) {
		App.isLoadingData = true;																// flag : chargement en cours
		if (scene != null && scene != "" && !SceneManager.GetSceneByName(scene).isLoaded) {		// si la scène n'est pas déjà chargée
			StartCoroutine(LoadAsyncScene(scene, restore));										//		lancer le chargement asynchrone
		} else {																				// si la scène est déjà chargée
			if (restore)																		//		si la restauration d'une sauvegarde est demandée
				Game.Restore(scene);															//			restaurer la sauvegarde
			else																				//		sinon
				Game.ResumeGame();																//			redémarrer l'exécution du jeu (fin de pause, ...)
		}
	}

	/// <summary>
	/// décharger un scène
	/// </summary>
	/// <param name="scene">La scène à décharger</param>
	/// <param name="callback">Une action à éxécuter après le déchargement asynchrone</param>
	public void UnloadScene(string scene, Action callback = null) {
		StartCoroutine(Unload());																// lancement de la coroutine
		IEnumerator Unload() {
			yield return new WaitForEndOfFrame();												// attendre 1 frame
			AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);					// lancer le déchargement asynchrone
			while (!asyncUnload.isDone) {														// attendre la fin du déchargement
				yield return new WaitForEndOfFrame();
			}
			if (callback != null) {																// si il y a une action à exécuter après
				callback();																		//		exécuter l'action
			}
		}
	}


	/// <summary>
	/// charger une scène Unity en asychrone
	/// </summary>
	/// <param name="scene">la scène à charger</param>
	/// <param name="restore">faut-il restorer l'état des objets d'après une sauvegarde ?</param>
	/// <returns></returns>
	IEnumerator LoadAsyncScene(string scene, bool restore) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);	// lancer le chargement asynchrone
		while (!asyncLoad.isDone) {																// attendre la fin du chargement
			yield return null;
		}

		// décharger toutes les scènes autres que celle qu'on vient de charger		
		SceneSaver[] sSavers = FindObjectsOfType<SceneSaver>();									// lister toutes les cènes chargées (elles on toutes un objet 'SceneSaver')
		foreach (SceneSaver sSaver in sSavers) {												// pour chaque 'SceneSaver'
			if (sSaver.gameObject.scene.name != scene) {										//		s'il n'appartient pas à la scène qu'on vient de charger
				UnloadScene(sSaver.gameObject.scene.name);										//			décharger la scène qui le contient
			}
		}

		App.currentSceneName = scene;															// la scène courante devient la scène chargée
		if (restore)																			// si la restauration d'une sauvegarde est requise
			Game.Restore(scene);																//		restaurer l'état de la scène 
		else																					// sinon
			Game.ResumeGame();																	//		relancer l'exécution du jeu
	}

}