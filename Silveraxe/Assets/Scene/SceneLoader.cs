using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System;

public class SceneLoader : MonoBehaviour
{
	public NavMeshAgent playerNavMesh;
	public string defaultScene;

	bool testing = false;

	private void Awake() {
		App.sceneLoader = this;
	}

	private void Start() {
		testing = SceneManager.sceneCount > 1;
		ResourcesManager.Init();
		GetDefaultScene();
	}

	void GetDefaultScene() {
		if (playerNavMesh.enabled) {
			App.playerManager.StopAgent();
			App.playerManager.navAgent.enabled = false;
		}
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(defaultScene, LoadSceneMode.Additive);
		App.currentSceneName = defaultScene;
		StartCoroutine(Callback());

		IEnumerator Callback() {
			while (!asyncLoad.isDone) {
				yield return null;
			}
			playerNavMesh.enabled = true;
		}
	}

	public void LoadScene(string scene) {
		if (scene != null && scene != "" && !SceneManager.GetSceneByName(scene).isLoaded) { //!testing && 
			if (playerNavMesh.enabled) {
				App.playerManager.StopAgent();
				App.playerManager.navAgent.enabled = false;
			}
			StartCoroutine(LoadAsyncScene(scene));
		} else {
			playerNavMesh.enabled = true;
			Game.current.LoadScene(scene);
			Game.current.LoadPlayer();
		}
	}

	public void UnloadScene(string scene) {
		StartCoroutine(Unload());
		IEnumerator Unload() {
			yield return new WaitForEndOfFrame();
			AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
			// Wait until the asynchronous scene fully unloads
			while (!asyncUnload.isDone) {
				yield return null;
			}
			// ??? on fait quoi après ???
		}
	}

	IEnumerator LoadAsyncScene(string scene) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone) {
			yield return null;
		}

		SceneSaver[] sSavers = FindObjectsOfType<SceneSaver>();
		foreach (SceneSaver sSaver in sSavers) {                    // décharger toutes les scènes autres que celle qu'on vient de charger
			if (sSaver.gameObject.scene.name != scene) {
				UnloadScene(sSaver.gameObject.scene.name);
			}
		}

		if (!App.sceneCrossing) {
			App.currentSceneName = scene;
			playerNavMesh.enabled = true;
			Game.current.LoadScene(scene);
			Game.current.LoadPlayer();
			//Game.current.SavePlayer();
		}

	}

}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public abstract class SScene
{
	public string id;         // nom de la scène
}