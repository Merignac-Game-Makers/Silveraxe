using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeStrip : Scrollbar
{
    Image background;
	Color color = Color.green;
	AspectRatioFitter aspectRatio;

	protected override void Start() {
		base.Start();
		background = GetComponent<Image>();
		color.a = .2f;
		background.color = color;
		aspectRatio = GetComponent<AspectRatioFitter>();
		aspectRatio.aspectRatio = 5;
	}

	public override void OnPointerDown(PointerEventData eventData) {
		base.OnPointerDown(eventData);
		aspectRatio.aspectRatio = 15;
	}

	//public override void OnDrag(PointerEventData eventData) {
	//	base.OnDrag(eventData);
	//	color = Color.green * (1 - value) + Color.red * value;
	//	color.a = .2f;
	//	background.color = color;
	//}

	public override void OnPointerUp(PointerEventData eventData) {
		base.OnPointerUp(eventData);
		value = 0;
		Time.timeScale = 1;
		SetColor(Color.green, .1f);
		aspectRatio.aspectRatio = 5;
	}

	public void OnValueChanged() {
		SetColor(Color.green * (1 - value) + Color.red * value, .2f);
		Time.timeScale = 1 + 19 * value;
	}

	void SetColor(Color color, float alpha) {
		color.a = alpha;
		background.color = color;
	}
}
