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

	public bool MultipleSelection() {
		bool found = false;
		foreach (InteractableObject obj in objects) {
			if (obj.IsSelected()) {
				if (found)
					return true;
				found = true;
			}
		}
		return false;
	}

	public void showActionSprites(bool on) {
		foreach (InteractableObject obj in objects) {
			if (obj.isOn && obj.actionSprite!=null) {
				obj.actionSprite.enabled = on;
				obj.mutedActionSprite = !on;
			}
		}
	}
}
