using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneLoader : MonoBehaviour
{
	public NavMeshAgent playerNavMesh;

	bool testing = false;

	public int currentLevelIndex { get; set; } = 0;

	private void Start() {

		testing = SceneManager.sceneCount > 1;
		App.sceneLoader = this;
		LoadScene(1);

	}

	public void LoadScene(int scene) {
		if (!testing && !SceneManager.GetSceneByBuildIndex(scene).isLoaded) {
			App.screneCrossing = true;
			if (playerNavMesh.enabled)
				App.playerManager.StopAgent();
			App.playerManager.navAgent.enabled = false;
			StartCoroutine(LoadAsyncScene(scene));
			if (currentLevelIndex != 0) {
				UnloadScene(currentLevelIndex);
			}
			currentLevelIndex = scene;
		} else {
			playerNavMesh.enabled = true;

		}
	}

	public void UnloadScene(int scene) {
		StartCoroutine(Unload());
		IEnumerator Unload() {
			yield return new WaitForEndOfFrame();
			SceneManager.UnloadSceneAsync(scene);
		}
	}

	IEnumerator LoadAsyncScene(int scene) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone) {
			yield return null;
		}

		playerNavMesh.enabled = true;
	}

}
