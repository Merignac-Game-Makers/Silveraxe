using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemBase", menuName = "Custom/Item base", order = -999)]
/// <summary>
/// Base clase of all items in the game. This is an abstract class and need to be inherited to specify behaviour.
/// The project offer 3 type of items : UsableItem, Equipment and Weapon
/// </summary>
public class ItemBase : ScriptableObject
{
	public string itemName;
	public Sprite itemSprite;
	public string description;
	public GameObject prefab;
	public Target target;
	public bool animate = true;
	public bool dropable = true;
	public LootCategory lootCategory;
	public bool usable = false;
	public List<UsageEffect> usageEffects;

	public Entry entry = null;								// L'entrée d'inventaire lorsque l'objet a été ramassé

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

	/// <summary>
	/// L'entrée d'inventaire lorsque l'objet a été ramassé
	/// </summary>
	/// <returns></returns>
	//public virtual InventoryEntry GetEntry() {
	//	return entry;
	//}

	//public Item Combine(Item other) {
	//	return (other == combineWith) ? obtain : null;
	//}
}


#if UNITY_EDITOR
[CustomEditor(typeof(ItemBase))]
public class ItemBaseEditor : Editor
{
	SerializedProperty pNameProperty;
	SerializedProperty pIconProperty;
	SerializedProperty pDescriptionProperty;
	SerializedProperty pPrefab;
	SerializedProperty pAnimate;
	//SerializedProperty pCombinable;
	//SerializedProperty pCombineWith;
	//SerializedProperty pObtain;
	SerializedProperty pDropable;
	//SerializedProperty pChapter;



	public void Init(SerializedObject target) {

		pNameProperty = target.FindProperty(nameof(ItemBase.itemName));
		pIconProperty = target.FindProperty(nameof(ItemBase.itemSprite));
		pDescriptionProperty = target.FindProperty(nameof(ItemBase.description));
		pPrefab = target.FindProperty(nameof(ItemBase.prefab));
		pAnimate = target.FindProperty(nameof(ItemBase.animate));
		//pCombinable = target.FindProperty(nameof(Item.combinable));
		//pCombineWith = target.FindProperty(nameof(Item.combineWith));
		//pObtain = target.FindProperty(nameof(Item.obtain));
		pDropable = target.FindProperty(nameof(ItemBase.dropable));
		//pChapter = target.FindProperty(nameof(Item.chapter));
	}

	public void GUI(ItemBase item) {

		EditorGUILayout.PropertyField(pIconProperty);
		EditorGUILayout.PropertyField(pNameProperty);
		EditorGUILayout.PropertyField(pDescriptionProperty, GUILayout.MinHeight(128));
		EditorGUILayout.PropertyField(pPrefab);
		//EditorGUILayout.PropertyField(pChapter);
		EditorGUILayout.PropertyField(pAnimate);

		//EditorGUI.BeginChangeCheck();
		//item.combinable = EditorGUILayout.Toggle("Combinable", pCombinable.boolValue);
		//if (item.combinable) {
		//	EditorGUILayout.PropertyField(pCombineWith);
		//	EditorGUILayout.PropertyField(pObtain);
		//} else {
			item.dropable = EditorGUILayout.Toggle("Dropable", pDropable.boolValue);
		//}
	}
}
#endif