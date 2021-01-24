using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portail : MonoBehaviour
{

	public string LevelToLoad;
	public Vector3 pos;

	float timer;

	private void Awake() {
		Temporize();
	}

	public void Temporize() {			// temporisation pour éviter un retour immédiat
		timer = 1;						// lors de l'initialisation
	}

	void Update() {
		if (timer > 0)					// décompte de la teporisation
			timer -= Time.deltaTime;
	}

	//void OnEnable() {
	//	SceneManager.sceneLoaded += OnLevelFinishedLoading;
	//}

	//void OnDisable() {
	//	SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	//}

	//private void OnTriggerEnter(Collider other) {
	//	if (timer <= 0) {									// le portail n'est actif qu'après la teporisation
	//		if (other.gameObject == App.playerManager.gameObject) {

	//			//Game.current.SaveScene(App.currentSceneName); 					// sauvegarder la scène qu'on quitte
	//			Game.Save(); 										// sauvegarder la scène qu'on quitte

	//			App.crossScenePosition = pos;						// coordonnées d'arrivée dans la scène cible
	//			App.sceneCrossing = true;							// flag : utilisation d'un portail
	//			App.sceneLoader.LoadScene(LevelToLoad, true);		// charger la scène cible
	//		}
	//	}
	//}

	//void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
	//	if (scene.name == LevelToLoad) {
	//		Game.Restore(scene.name);
	//	}
	//}

}
