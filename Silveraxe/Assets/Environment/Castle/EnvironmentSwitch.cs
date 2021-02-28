using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSwitch : MonoBehaviour {

	public GameObject fake;
	public GameObject heavy;

	protected virtual void Start() {
		fake.gameObject.SetActive(true);
		heavy.gameObject.SetActive(false);
	}

	protected virtual void OnTriggerEnter(Collider other) {
		if (other.gameObject == App.playerManager.gameObject) {
			fake.gameObject.SetActive(false);
			heavy.gameObject.SetActive(true);
		}
	}

	protected virtual void OnTriggerExit(Collider other) {
		if (other.gameObject == App.playerManager.gameObject) {
			fake.gameObject.SetActive(true);
			heavy.gameObject.SetActive(false);
		}
	}
}
