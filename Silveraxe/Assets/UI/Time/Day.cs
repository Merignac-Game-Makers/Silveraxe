using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day : MonoBehaviour {
	public int day;
	public Image badge;
	public TextMeshProUGUI number;

	public Color clearColor = Color.green;
	public Color passedColor = Color.red;

	Image frame;


	void Awake() {
		frame = GetComponent<Image>();
		number.text = day.ToString();
		Clear();
	}

	public void Clear() {
		badge.enabled = false;
		frame.color = clearColor;
		//badge.color = clearColor;
		number.color = Color.white;
	}

	public void Passed() {
		badge.enabled = true;
		frame.color = passedColor;
		badge.color = passedColor;
		setAlpha(badge, 1f);
		number.color = passedColor;

	}

	Color c;
	void setAlpha(Image i, float alpha) {
		c = i.color;
		c.a = alpha;
		i.color = c;
	}
	void setAlpha(TextMeshProUGUI i, float alpha) {
		c = i.color;
		c.a = alpha;
		i.color = c;
	}
}
