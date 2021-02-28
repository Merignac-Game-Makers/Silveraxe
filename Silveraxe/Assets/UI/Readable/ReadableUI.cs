using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class ReadableUI : UIBase
{
	public TMP_Text text;
	public Image picture;

	Rect textRect;
	Rect pictureRect;

	private void Awake() {
		App.readableUI = this;
	}

	private void Start() {
		textRect = text.gameObject.GetComponent<RectTransform>().rect;
		pictureRect = picture.gameObject.GetComponent<RectTransform>().rect;
		Show(false);
	}

	public void ShowMessage(string text, Sprite sprite = null, Action callback = null) {
		if (text==null || text == "") {
			this.text.enabled = false;
			pictureRect.height = textRect.height; 
		} else {
			this.text.enabled = true;
			this.text.text = text;
			pictureRect.height = textRect.height/3f;
			picture.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2( pictureRect.width, pictureRect.height);
		}
		if (sprite) {
			picture.enabled = true;
			picture.sprite = sprite;
		} else {
			picture.enabled = false;
		}
		this.callback = callback;
		Toggle(true);
	}

	public void Toggle(bool on) {
		Show(on);
	}
}
