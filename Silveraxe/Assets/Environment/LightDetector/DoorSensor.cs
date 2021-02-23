using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour {

	public LightDetector lightDetector;
	public DoorOrc doorPivot;

	public float thresholdToOpen;
	public float thresholdToClose;
	public bool isOn { get; private set; }

	private void Awake() {
		lightDetector.OnTriggerOn += CloseDoor;
		lightDetector.OnTriggerOff += OpenDoor;
	}

	private void OnEnable() {
		lightDetector.OnTriggerOn += CloseDoor;
		lightDetector.OnTriggerOff += OpenDoor;
	}

	private void Start() {
		if (lightDetector.isOn)
			lightDetector.threshold = thresholdToClose;
		else
			lightDetector.threshold = thresholdToOpen;
	}

	private void Update() {
		if (!doorPivot.doorOrc.isAlive) {
			doorPivot.doorAnim.speed = 0;
		}
	}

	private void OnDisable() {
		lightDetector.OnTriggerOn -= OpenDoor;
		lightDetector.OnTriggerOff -= CloseDoor;
	}

	void OpenDoor() {
		doorPivot.OpenDoor();
		//lightDetector.threshold = thresholdToClose;
	}
	void CloseDoor() {
		doorPivot.CloseDoor();
		//lightDetector.threshold = thresholdToOpen;
	}

	public void ToggleThreshold() {
		if (lightDetector.threshold == thresholdToOpen)
			lightDetector.threshold = thresholdToClose;
		else
			lightDetector.threshold = thresholdToOpen;

	}
}
