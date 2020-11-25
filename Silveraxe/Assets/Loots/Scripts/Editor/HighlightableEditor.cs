using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HighlightableObject))]
public class HighlightableEditor : Editor
{
	SerializedProperty pHighlightable;
	SerializedProperty pUseLight;
	SerializedProperty pUseOutline;

	public void Init(SerializedObject target) {
		pHighlightable = target.FindProperty(nameof(HighlightableObject.isHighlightable));
		pUseLight = target.FindProperty(nameof(HighlightableObject.useLight));
		pUseOutline = target.FindProperty(nameof(HighlightableObject.useOutline));
	}

	public void GUI(HighlightableObject item) {
		if (item.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")) {
			EditorGUILayout.LabelField("Highlightable Options", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			item.isHighlightable = pHighlightable.boolValue = EditorGUILayout.Toggle("Is Highlightable", pHighlightable.boolValue);
			if (item.isHighlightable) {
				item.useLight = pUseLight.boolValue = EditorGUILayout.Toggle("Use Light", pUseLight.boolValue);
				item.useOutline = pUseOutline.boolValue = EditorGUILayout.Toggle("Use Outline", pUseOutline.boolValue);
			}
			EditorGUI.indentLevel--;
		}
	}
}
