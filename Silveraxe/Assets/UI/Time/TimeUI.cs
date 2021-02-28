using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUI : UIBase
{
	public RectTransform timeHolder;
	public GameObject days;
	public GameObject buttons;

	bool firstUse = true;

	Rect rect;
	public bool showDays { get; set; } = false;
	public bool showButtons { get; set; } = false;

	private void Awake() {
		App.timeUI = this;
	}

	private void Start() {
		days.SetActive(false);
		buttons.SetActive(false);
	}

	private void Update() {
		rect = timeHolder.rect;
		rect.width = Screen.width / 10f;
		timeHolder.sizeDelta = new Vector2(rect.width, rect.height);

		if (Input.GetKeyDown(KeyCode.T)) {
			Toggle();
		}
	}

	public override void Show(bool on) {
		base.Show(on);
		days.SetActive(showDays);
		buttons.SetActive(showButtons);
		if (firstUse && on) {
			App.instructionsUI.showT = true;
			firstUse = false;
		}
	}
}
