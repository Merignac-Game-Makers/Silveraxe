using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System;

public class SceneLoader : MonoBehaviour
{
	public NavMeshAgent playerNavMesh;

	bool testing = false;

	public int currentLevelIndex { get; set; } = 0;


	private void Awake() {
				App.sceneLoader = this;
	}

	private void Start() {
		testing = SceneManager.sceneCount > 1;
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


	#region sauvegarde
	/// <summary>
	/// Restaurer les valeurs précédement sérialisées
	/// </summary>
	/// <param name="serialized">les valeurs sérialisées</param>
	public virtual void Deserialize(object serialized) {
		if (serialized is SScene) {
			SScene s = serialized as SScene;
		}
	}
	#endregion


}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public abstract class SScene
{
	public string id;         // nom de la scène
}