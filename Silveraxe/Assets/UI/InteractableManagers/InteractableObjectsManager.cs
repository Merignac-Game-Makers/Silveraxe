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

	List<InteractableObject> objects;


	private void Awake() {
		interactableObjectsManager = this;
		objects = new List<InteractableObject>(FindObjectsOfType<InteractableObject>());
		objects.Remove(playerManager);
	}

	void Start() {
	}

	private void Update() {
		Closest();
	}

	float dist;
	float d;
	public InteractableObject closest { get; private set; }
	public InteractableObject Closest() {
		dist = 999999;
		closest = null;
		foreach (InteractableObject obj in objects) {
			if (!obj.IsHighlightable())
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
