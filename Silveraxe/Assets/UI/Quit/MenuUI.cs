using UnityEngine;
using UnityEngine.SceneManagement;
using static App;


public class MenuUI : UIBase
{

	private void Start() {
		Show(false);
	}

	public void ContinueButton() {
		Portail[] portals = FindObjectsOfType<Portail>();
		foreach(Portail portal in portals) {
			portal.Temporize();					// temporisation pur éviter un transport immédiat si la reprise de sauvegarde est sur un portail
		}
		Game.Load();							// charger la sauvegarde courante
		Toggle();
	}

	public void NewGameButton() {
		Game.NewGame();
		Toggle();
	}

	public void SaveContinueButton() {
		Game.Save();
		Toggle();
	}

	public void SaveQuitButton() {
		Game.Save();
		Application.Quit();
	}

	public override void Toggle() {
		Show(!panel.activeInHierarchy);
	}
}
