using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

	public int LevelToLoad;
	public Vector3 pos;

	float timer;

	private void Awake() {
		timer = 5;
	}

	void Update() {
		if (timer > 0)
			timer -= Time.deltaTime;
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		if (scene.buildIndex != LevelToLoad) {
			SceneManager.SetActiveScene(scene);
			if (App.playerManager) {
				App.playerManager.transform.position = App.crossScenePosition;
				App.playerManager.transform.rotation = Quaternion.identity;
				//App.playerManager.navAgent.enabled = true;
			}

		}
	}


	private void OnTriggerEnter(Collider other) {
		if (timer <= 0) {
			if (other == App.playerManager.GetComponent<CharacterController>()) {
				App.crossScenePosition = pos;
				App.sceneLoader.LoadScene(LevelToLoad);
			}
		}
	}
}
