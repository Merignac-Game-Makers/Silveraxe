using UnityEngine;

using static App;


public class MenuUI : UIBase
{

	private void Start() {
		Game.current = new Game();
	}

	public void ContinueButton() {
		SaveLoad.Load();
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
		//throw new System.NotImplementedException();
	}
}
