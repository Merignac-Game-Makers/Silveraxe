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

	public int currentLevelIndex { get; set; } = 0;
	public string currentSceneName { get; set; } = "";

	private void Awake() {
		App.sceneLoader = this;
	}

	private void Start() {
		testing = SceneManager.sceneCount > 1;
		LoadScene(defaultScene);
	}

	public void LoadScene(string scene) {
		if (!testing && !SceneManager.GetSceneByName(scene).isLoaded) {
			App.sceneCrossing = true;
			if (playerNavMesh.enabled) {
				App.playerManager.StopAgent();
				App.playerManager.navAgent.enabled = false;
			}
			StartCoroutine(LoadAsyncScene(scene));

		} else {
			playerNavMesh.enabled = true;
		}
	}

	public void UnloadScene(string scene) {
		StartCoroutine(Unload());
		IEnumerator Unload() {
			yield return new WaitForEndOfFrame();
			SceneManager.UnloadSceneAsync(scene);
		}
	}

	IEnumerator LoadAsyncScene(string scene) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone) {
			yield return null;
		}

		SceneSaver[] sSavers = FindObjectsOfType<SceneSaver>();
		foreach (SceneSaver sSaver in sSavers) {					// décharger toutes les scènes autres que celle qu'on vient de charger
			if (sSaver.gameObject.scene.name != scene) {
				UnloadScene(sSaver.gameObject.scene.name);
			}
		}

		currentSceneName = scene; 
		playerNavMesh.enabled = true;
		SaveLoad.LoadSceneData(scene);
		Game.current.SavePlayer();
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