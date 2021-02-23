using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{

	public float fast = 10;
	private bool isFast = false;

	public void ToggleTime() {
		isFast = !isFast;
		if (isFast) {
			Time.timeScale = fast;
		} else {
			Time.timeScale = 1;
		}
	}


}
