using UnityEngine;

public class QuitUI : MonoBehaviour
{

	public void Show(bool on) {
		gameObject.SetActive(on);
		if (on)
			UIManager.Instance.ManageButtons(UIManager.State.quit);
		else
			UIManager.Instance.RestoreButtonsPreviousState();
	}

	public void YesButton() {
		Application.Quit();
	}

	public void NoButton() {
		Show(false);
	}

}
