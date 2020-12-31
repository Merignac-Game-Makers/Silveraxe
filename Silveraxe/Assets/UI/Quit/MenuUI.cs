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
		Portail[] cScenes = FindObjectsOfType<Portail>();
		foreach(Portail cs in cScenes) {
			cs.Temporize();						// temporisation pur éviter un transport immédiat si la reprise de sauvegarde est sur un portail
		}

		Game.current.LoadHeader();							// charger le nom de la scène courante

		sceneLoader.LoadScene(App.currentSceneName);								// charger la scène dans laquelle doit se trouver le joueur ... puis charger le joueur

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
