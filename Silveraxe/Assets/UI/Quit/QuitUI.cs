using UnityEngine;

using static App;


public class QuitUI : MonoBehaviour
{

	public void Show(bool on) {
		gameObject.SetActive(on);
		if (on)
			uiManager.ManageButtons(UIManager.State.quit);
		else
			uiManager.RestoreButtonsPreviousState();
	}

	public void YesButton() {
		Application.Quit();
	}

	public void NoButton() {
		Show(false);
	}

}
