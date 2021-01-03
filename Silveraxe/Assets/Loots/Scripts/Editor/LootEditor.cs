using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Loot))]
public class LootEditor : Editor
{
	Loot m_Target;

	HighlightableEditor m_HighlightableEditor;

	SerializedProperty pItemBase;
	SerializedProperty pInteractionMode;
	//SerializedProperty pTarget;

	List<string> m_AvailableUsageType;

	void OnEnable() {
		m_Target = target as Loot;

		//m_Target.usable = false;
		//m_Target.UsageEffects.Clear();
		//serializedObject.Update();

		m_HighlightableEditor = CreateInstance<HighlightableEditor>();
		m_HighlightableEditor.Init(serializedObject);

		pItemBase = serializedObject.FindProperty(nameof(Loot.itemBase));
		pInteractionMode = serializedObject.FindProperty(nameof(Loot.mode));
		//pTarget = serializedObject.FindProperty(nameof(Loot.target));

		//var lookup = typeof(UsageEffect);
		//m_AvailableUsageType = System.AppDomain.CurrentDomain.GetAssemblies()
		//	.SelectMany(assembly => assembly.GetTypes())
		//	.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
		//	.Select(type => type.Name)
		//	.ToList();

	}

	public override void OnInspectorGUI() {
		//serializedObject.Update();

		EditorGUILayout.PropertyField(pItemBase);
		EditorGUILayout.PropertyField(pInteractionMode);
		//EditorGUILayout.PropertyField(pTarget);


		m_HighlightableEditor.GUI(target as Loot);


		serializedObject.ApplyModifiedProperties();
	}
}