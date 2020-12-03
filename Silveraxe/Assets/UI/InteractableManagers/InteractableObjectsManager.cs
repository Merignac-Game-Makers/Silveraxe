using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static App;

public class InteractableObjectsManager : MonoBehaviour
{

	public Texture2D dialogueIcon;
	public Texture2D takeIcon;
	public Texture2D dropIcon;
	public Texture2D fightIcon;

	InteractableObject[] objects;


	private void Awake() {
		interactableObjectsManager = this;
		objects = FindObjectsOfType<InteractableObject>();
	}

	void Start() {
	}

	//public bool MultipleSelection() {
	//	bool found = false;
	//	foreach (InteractableObject obj in objects) {
	//		if (obj.isOn) { //.IsSelected()
	//			if (found)
	//				return true;
	//			found = true;
	//		}
	//	}
	//	return false;
	//}

	float dist;
	float d;
	InteractableObject closest;
	public InteractableObject Closest() {
		dist = 999999;
		closest = null;
		foreach (InteractableObject obj in objects) {
			if (!obj.isInPlayerCollider)
				continue;
			d = (obj.transform.position - playerManager.transform.position).sqrMagnitude;
			if (d < dist) {
				closest = obj;
				dist = d;
			}
		}
		return closest;
	}


	public void SelectAll(bool on) {
		foreach (InteractableObject obj in objects) {
			obj.selectionMuted = !on;
			obj.Highlight(on && obj.isInPlayerCollider);
		}
	}
}
