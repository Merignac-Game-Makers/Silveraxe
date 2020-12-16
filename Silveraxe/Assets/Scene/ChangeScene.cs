using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

	public string LevelToLoad;
	public Vector3 pos;

	float timer;

	private void Awake() {
		Temporize();
	}

	public void Temporize() {
		timer = 1;
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
		if (scene.name != LevelToLoad) {
			SceneManager.SetActiveScene(scene);
			if (App.playerManager) {
				App.playerManager.transform.position = App.crossScenePosition;
				App.playerManager.transform.rotation = Quaternion.identity;
			}
		}
	}


	private void OnTriggerEnter(Collider other) {
		if (timer <= 0) {
			if (other == App.playerManager.GetComponent<CharacterController>()) {

				Game.current.SaveScene(); 					// sauvegarder la scène qu'on quitte

				App.crossScenePosition = pos;				// coordonnées d'arrivée dans la scène cible
				App.sceneLoader.LoadScene(LevelToLoad);		// charger la scène cible
			}
		}
	}
}
