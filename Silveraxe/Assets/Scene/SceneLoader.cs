using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	bool testing = false;

	public int currentLevelIndex { get; set; } = 0;

	private void Start() {

		testing = SceneManager.sceneCount > 1;
		App.sceneLoader = this;
		LoadScene(1);

	}

	public void LoadScene(int scene) {
		if (testing) return;
		if (!SceneManager.GetSceneByBuildIndex(scene).isLoaded) {
			App.screneCrossing = true;
			if (App.playerManager.navAgent)
				App.playerManager.StopAgent();
			App.playerManager.navAgent.enabled = false;
			SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			if (currentLevelIndex != 0) {
				UnloadScene(currentLevelIndex);
			}
			currentLevelIndex = scene;
		}
	}

	public void UnloadScene(int scene) {
		StartCoroutine(Unload());
		IEnumerator Unload() {
			yield return new WaitForEndOfFrame();
			SceneManager.UnloadSceneAsync(scene);
		}
	}

}
