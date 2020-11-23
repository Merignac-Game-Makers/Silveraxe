﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Target))]
public class TargetEditor : Editor
{
	Target m_Target;

	SerializedProperty pPrefab;
	SerializedProperty pCursor;
	SerializedProperty pFilterMode;
	SerializedProperty pFilterItems;

	void OnEnable() {
		m_Target = target as Target;

		//m_Target.usable = false;
		//m_Target.UsageEffects.Clear();
		//serializedObject.Update();

		pPrefab = serializedObject.FindProperty(nameof(Target.prefab));
		pCursor = serializedObject.FindProperty(nameof(Target.cursor));
		pFilterMode = serializedObject.FindProperty(nameof(Target.filterMode));
		pFilterItems = serializedObject.FindProperty(nameof(Target.filterItems));

	}

	public override void OnInspectorGUI() {
		//serializedObject.Update();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(pPrefab);
		if (EditorGUI.EndChangeCheck()) {
			Debug.Log("change prefab");
			var holder = m_Target.transform.Find("PrefabHolder");
			foreach (Transform t in holder) {
				DestroyImmediate(t.gameObject);
			}
			var obj = Instantiate(pPrefab.objectReferenceValue, holder) as GameObject;
			obj.layer = holder.gameObject.layer;
		}

		EditorGUILayout.PropertyField(pCursor);
		EditorGUILayout.PropertyField(pFilterMode);
		EditorGUILayout.PropertyField(pFilterItems);

		serializedObject.ApplyModifiedProperties();
	}
}

