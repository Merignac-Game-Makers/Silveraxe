using UnityEngine;
using static InventoryManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Base clase of all PNJ in the game. This is an abstract class and need to be inherited to specify behaviour.
/// The project offer ? type of items : neutralPNJ, Ennemy ...
/// </summary>
public abstract class PNJbase : ScriptableObject
{
	public string Name;
	public string Description;
	public GameObject WorldObjectPrefab;
	public string CameraTargetName;
	public bool animate = true;

	[HideInInspector]
	public GameObject CameraTarget;

	public virtual string GetDescription() {
		return Description;
	}

	public virtual GameObject getCameraTarget() {
		foreach (Transform t in WorldObjectPrefab.transform) {
			if (t.name == CameraTargetName)
				return t.gameObject;
		}
		return null;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PNJbase))]
public class PNJbaseEditor
{
	SerializedProperty pNameProperty;
	SerializedProperty pDescriptionProperty;
	SerializedProperty pWorldObjectPrefabProperty;
	SerializedProperty pCameraTargetProperty;
	SerializedProperty pAnimate;

	public void Init(SerializedObject target) {

		pNameProperty = target.FindProperty(nameof(PNJbase.Name));
		pDescriptionProperty = target.FindProperty(nameof(PNJbase.Description));
		pWorldObjectPrefabProperty = target.FindProperty(nameof(PNJbase.WorldObjectPrefab));
		pCameraTargetProperty = target.FindProperty(nameof(PNJbase.CameraTargetName));
		pAnimate = target.FindProperty(nameof(PNJbase.animate));
	}

	public void GUI(PNJbase pnj) {

		EditorGUILayout.PropertyField(pNameProperty);
		EditorGUILayout.PropertyField(pDescriptionProperty, GUILayout.MinHeight(128));
		EditorGUILayout.PropertyField(pWorldObjectPrefabProperty);
		EditorGUILayout.PropertyField(pCameraTargetProperty);
		EditorGUILayout.PropertyField(pAnimate);

	}
}
#endif