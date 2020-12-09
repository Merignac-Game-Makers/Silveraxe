using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{

    public LightDetector lightDetector;
    public DoorOrc doorPivot;

    public float thresholdToOpen;
    public float thresholdToClose;
    public bool isOn { get; private set; }

    // Start is called before the first frame update
    private void OnEnable()
    {
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
            //doorOrc.doorAnim.SetTrigger("Stop");
            //doorOrc.doorAnim.StopPlayback();
            //lightDetector.enabled = false;
            //enabled = false;
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
