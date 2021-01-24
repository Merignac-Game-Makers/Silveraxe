﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObjectsManager : MonoBehaviour
{

	public InteractableObject closest { get; private set; }

	List<InteractableObject> objects = new List<InteractableObject>();
	float dist;
	float d;

	private void Awake() {
		App.itemsManager = this;
	}

	private void Start() {
		GetSceneObjects();
	}

	/// <summary>
	/// récupérer tous les InteractableObject de la scène dans la listes 'objects'
	/// </summary>
	public void GetSceneObjects() {
		objects = new List<InteractableObject>(FindObjectsOfType<InteractableObject>());
		objects.Remove(App.playerManager);
		allItemBases = Resources.LoadAll("", typeof(ItemBase)); // lister les Loot et Equipment
	}

	private void FixedUpdate() {
		Closest();
	}

	/// <summary>
	/// trouver l'objet intéractible le plus proche du joueur
	/// </summary>
	/// <returns></returns>
	public InteractableObject Closest() {
		dist = 999999;
		closest = null;
		foreach (InteractableObject obj in objects) {
			if (!obj.IsHighlightable())
				continue;
			d = (obj.transform.position - App.playerManager.transform.position).sqrMagnitude;
			if (d < dist) {
				closest = obj;
				dist = d;
			}
		}
		return closest;
	}

	/// <summary>
	/// Allumer ou éteindre tous les objets qui sont dans le collider du joueur
	/// </summary>
	/// <param name="on"></param>
	public void SelectAll(bool on) {
		foreach (InteractableObject obj in objects) {
			obj.selectionMuted = !on;
			obj.Highlight(on && obj.isInPlayerCollider);
		}
	}

	public Object[] allItemBases;
	public ItemBase GetItemBase(string name) {
		foreach (ItemBase itemBase in allItemBases) {
			if (itemBase.name == name)
				return itemBase as ItemBase;
		}
		return null;
	}
}
