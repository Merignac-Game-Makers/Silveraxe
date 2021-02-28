using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ExplanationsUI : UIBase
{
	public TMP_Text title;
	public TMP_Text explanations;
	public TMP_Text bottom;
	public Image picture;

	bool firstUse = true;

	private void Awake() {
		App.explanationsUI = this;
	}

	private void Start() {
		Toggle(false);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.E)) { 
			Toggle();
		}
	}

	public void Show(Explanation explanation) {
		Show(explanation.title, explanation.explanation, explanation.picture, explanation.bottom);
	}

	public void Show(string title, string explanations, Sprite picture, string bottom) {
		if (title!= null && title != "") {
			this.title.text = title;
			this.title.enabled = true;
		} else {
			this.title.enabled = false;
		}
		if (bottom != null && bottom != "") {
			this.bottom.text = bottom;
			this.bottom.enabled = true;
		} else {
			this.bottom.enabled = false;
		}
		if (explanations != null && explanations != "") {
			this.explanations.text = explanations;
			this.explanations.enabled = true;
		} else {
			this.explanations.enabled = false;
		}	
		if (picture != null) {
			this.picture.sprite = picture;
			this.picture.gameObject.SetActive(true);
		} else {
			this.picture.gameObject.SetActive(false);
		}
		Toggle(true);
	}


	public void Toggle(bool on) {
		if (firstUse && on) {
			App.instructionsUI.showE = true;
			firstUse = false;
		}
		Show(on);
	}
}
