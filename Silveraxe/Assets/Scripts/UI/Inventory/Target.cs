using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//using static QuestBase.QuestStatus;

/// <summary>
/// Objet intéractible sur lequel on peut déposer un objet d'inventaire (loot)
/// </summary>
public class Target : InteractableObject
{
	public List<Item> avalableItems;
	public List<Item> forbiddenItems;

	public override bool IsInteractable() => true;				// toujours actif

	public bool isFree => !GetComponentInChildren<Loot>();      // ne peut contenir qu'un seul objet d'inventaire

	public bool isAvailable(Item item) {
		if (!isFree) return false;
		//if (item.combinable) return false;
		if (!item.dropable) return false;
		if (avalableItems.Count > 0 && !avalableItems.Contains(item)) return false;
		if (forbiddenItems.Count > 0 && forbiddenItems.Contains(item)) return false;
		return true;
	}

	protected override void Start() {
		base.Start();											// initialisation des effets de surbrillance
	}

}

///// <summary>
///// Masquer l'éditeur par défaut lié à InterractableObject
///// => éditeur vide (pas de paramètre)
///// => commentaire
///// </summary>
//#if UNITY_EDITOR
//[CustomEditor(typeof(Target))]
//public class TargetObjectEditor : Editor
//{
//	public override void OnInspectorGUI() {
//		EditorStyles.label.wordWrap = true;
//		EditorGUILayout.LabelField("Objet intéractible sur lequel on peut déposer un objet d'inventaire (loot)");
//	}
//}
//#endif
