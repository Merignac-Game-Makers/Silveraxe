using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class InstructionsUI : UIBase
{
	public TMP_Text text;

	private void Awake() {
		App.instructionsUI = this;
	}

	private void Start() {
		Show(false);
	}

	private void Update() {
		if (Input.anyKeyDown) { 
			if (Input.inputString == "?") // Point d'interrogation
				Toggle();
		}
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
