using UnityEngine;

using static App;


public class MenuUI : UIBase
{

	private void Start() {
		Game.current = new Game();
		Show(false);
	}

	public void ContinueButton() {
		SaveLoad.Load(true);
		Toggle();
	}

	public void NewGameButton() {
		Toggle();
	}

	public void SaveContinueButton() {
		Game.current.Save(true);
		Toggle();
	}

	public void SaveQuitButton() {
		Game.current.Save(true);
		Application.Quit();
	}

	public override void Toggle() {
		Show(!panel.activeInHierarchy);
	}
}
