using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class ReadableUI : UIBase
{
	public TMP_Text text;

	private void Awake() {
		App.readableUI = this;
	}

	private void Start() {
		Show(false);
	}

	public void ShowText(string t) {
		text.text = t;
		Toggle(true);
	}

	public override void Toggle() {
		Show(!panel.activeInHierarchy);
	}

	public void Toggle(bool on) {
		Show(on);
	}
}
