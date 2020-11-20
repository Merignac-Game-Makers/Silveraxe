using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static InteractableObject.Action;


/// <summary>
/// Base class for interactable object, inherit from this class and override InteractWith to handle what happen when
/// the player interact with the object.
/// </summary>
public abstract class InteractableObject : HighlightableObject
{
	public enum Action { take, drop, talk }

	public enum Mode { onClick, onTheFly, onTheFlyOnce }	// modes d'intéraction possibles

	public Mode mode;                                       // le mode d'intéraction de cet objet

	public abstract bool IsInteractable();					// m'objet est-il actif pour l'intéraction ?

	[HideInInspector]
	public bool Clicked;                                    // flag clic sur l'objet ?

	protected override void Start() {
		base.Start();
		// ajouter des MeshColliders si nécessaire
		var meshes = GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter m in meshes) {
			if (m.GetComponent<MeshCollider>()==null)
				m.gameObject.AddComponent<MeshCollider>();

		}
	}

	public virtual void InteractWith(CharacterData character, HighlightableObject target = null, Action action = take) {
		Clicked = false;
		if (mode == Mode.onTheFlyOnce)
			mode = Mode.onClick;
	}

	public virtual void OnTriggerEnter(Collider other) {
		if (!isOn && IsInteractable())
			Highlight(true);
	}

	public virtual void OnTriggerExit(Collider other) {
		if (isOn)
			Highlight(false);
	}

	public virtual void OnMouseUp() {
	}
}
