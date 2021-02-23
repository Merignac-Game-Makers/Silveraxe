using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
	public RectTransform timeHolder;
	Rect rect;

	private void Update() {
		rect = timeHolder.rect;
		rect.width = Screen.width / 10f;
		timeHolder.sizeDelta = new Vector2(rect.width, rect.height);
	}
}
