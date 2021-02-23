using UnityEditor;

[CustomEditor(typeof(PNJ))]
[CanEditMultipleObjects]
public class PNJEditor : Editor
{
	SerializedProperty p_mode;
	SerializedProperty p_alignment;
	SerializedProperty p_radius;
	SerializedProperty p_PNJName;

	PNJ m_PNJ;

	public virtual void OnEnable() {
		m_PNJ = (PNJ)target;
		p_mode = serializedObject.FindProperty(nameof(m_PNJ.mode));
		p_alignment = serializedObject.FindProperty(nameof(m_PNJ.alignment));
		p_PNJName = serializedObject.FindProperty(nameof(m_PNJ.PNJName));
		p_radius = serializedObject.FindProperty(nameof(m_PNJ.onTheFlyRadius));
	}


	public override void OnInspectorGUI() {
		EditorStyles.textField.wordWrap = true;
		serializedObject.Update();

		EditorGUILayout.PropertyField(p_PNJName);
		EditorGUILayout.PropertyField(p_alignment);
		EditorGUILayout.PropertyField(p_mode);
		if (m_PNJ.mode == InteractableObject.Mode.onTheFlyOnce) {
			EditorGUILayout.PropertyField(p_radius);
		}
		EditorGUILayout.PropertyField(p_radius);

		serializedObject.ApplyModifiedProperties();
	}
	//void OnInspectorUpdate() {
	//	Repaint();
	//}
}