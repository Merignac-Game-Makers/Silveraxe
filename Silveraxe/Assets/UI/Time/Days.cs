using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Days : MonoBehaviour {
	public Day[] days { get; set; }

	public void SetDay(int day) {
		foreach (Day d in days) {
			if (d.day < day) {
				d.Passed();
			} else {
				d.Clear();
			}
		}
	}
}
