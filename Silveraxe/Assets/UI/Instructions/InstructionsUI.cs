using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class InstructionsUI : UIBase
{
	public TMP_Text title;
	public Image D;
	public Image I;
	public Image M;
	public Image T;
	public Image E;

	public bool showD { get; set; } = true;
	public bool showI { get; set; } = false;
	public bool showM { get; set; } = false;
	public bool showT { get; set; } = false;
	public bool showE { get; set; } = false;

	private void Awake() {
		App.instructionsUI = this;
	}

	private void Start() {
		Toggle(false);
	}

	private void Update() {
		if (Input.anyKeyDown) { 
			if (Input.inputString == "?") // Point d'interrogation
				Toggle();
		}
	}

	public void ShowTitle(bool on) {
		title.enabled = on;
	}
	public void ShowImages() {
		D.enabled = showD;
		I.enabled = showI;
		M.enabled = showM;
		T.enabled = showT;
		E.enabled = showE;
	}

	public override void Toggle() {
		ShowImages();
		base.Toggle();
	}

	public void Toggle(bool on) {
		ShowImages();
		Show(on);
	}
}
