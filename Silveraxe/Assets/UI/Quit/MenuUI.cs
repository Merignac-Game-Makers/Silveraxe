using UnityEngine;
using UnityEngine.SceneManagement;
using static App;


public class MenuUI : UIBase
{

	private void Start() {
		Game.current = new Game();
		Show(false);
	}

	public void ContinueButton() {
		ChangeScene[] cScenes = FindObjectsOfType<ChangeScene>();
		foreach(ChangeScene cs in cScenes) {
			cs.Temporize();
		}

		var cScene = FindObjectOfType<SceneSaver>().gameObject.scene.name;
		SaveLoad.LoadPlayerData();
		if (cScene != sceneLoader.currentSceneName) {
			sceneLoader.LoadScene(sceneLoader.currentSceneName);
		} else {
			SaveLoad.LoadSceneData(sceneLoader.currentSceneName);
		}
		Toggle();
	}

	public void NewGameButton() {
		Toggle();
	}

	public void SaveContinueButton() {
		Game.current.Save();
		Toggle();
	}

	public void SaveQuitButton() {
		Game.current.Save();
		Application.Quit();
	}

	public override void Toggle() {
		Show(!panel.activeInHierarchy);
	}
}
