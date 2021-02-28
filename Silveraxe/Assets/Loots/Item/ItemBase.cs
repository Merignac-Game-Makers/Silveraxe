﻿using UnityEngine;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemBase", menuName = "Custom/Item base", order = -999)]
/// <summary>
/// Base clase of all items in the game. This is an abstract class and need to be inherited to specify behaviour.
/// The project offer 3 type of items : UsableItem, Equipment and Weapon
/// </summary>
public class ItemBase : ScriptableObject {
	public string itemName;
	public Sprite itemSprite;
	[TextArea(1, 10)]
	public string description;
	public bool animate = true;
	public bool dropable = true;
	public LootCategory lootCategory;
	public bool usable = false;
	public List<UsageEffect> usageEffects;
	public Explanation explanation;

	public Entry entry = null;                              // L'entrée d'inventaire lorsque l'objet a été ramassé

	/// <summary>
	/// Called by the inventory system when the object is "used" (double clicked)
	/// </summary>
	/// <param name="user">The CharacterDate that used that item</param>
	/// <returns>If it was actually used (allow the inventory to know if it can remove the object or not)</returns>
	public virtual bool Used(CharacterData user) {
		return false;
	}

	public virtual string GetDescription() {
		return description;
	}

	public virtual void ShowExplanation(Action callback = null) {
		App.explanationsUI.callback = callback;
		if (explanation) {
			App.explanationsUI.Show(explanation);
		} else {
			App.explanationsUI.Show(false);
		}
	}

}


#if UNITY_EDITOR
[CustomEditor(typeof(ItemBase))]
public class ItemBaseEditor : Editor {
	SerializedProperty pNameProperty;
	SerializedProperty pIconProperty;
	SerializedProperty pDescriptionProperty;
	SerializedProperty pAnimate;
	SerializedProperty pDropable;

	public void Init(SerializedObject target) {
		pNameProperty = target.FindProperty(nameof(ItemBase.itemName));
		pIconProperty = target.FindProperty(nameof(ItemBase.itemSprite));
		pDescriptionProperty = target.FindProperty(nameof(ItemBase.description));
		pAnimate = target.FindProperty(nameof(ItemBase.animate));
		pDropable = target.FindProperty(nameof(ItemBase.dropable));


	}

	public void GUI(ItemBase item) {
		EditorGUILayout.PropertyField(pNameProperty);
		EditorGUILayout.PropertyField(pIconProperty);
		EditorGUILayout.PropertyField(pDescriptionProperty);
		EditorGUILayout.PropertyField(pAnimate);
		EditorGUILayout.PropertyField(pDropable);
	}
}
#endif