using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Describe a PNJ.
/// </summary>
[CreateAssetMenu(fileName = "PNJinstance", menuName = "Custom/PNJ", order = -999)]
public class PNJinstance : PNJbase
{
	public override string GetDescription() {
		string description = base.GetDescription();

		if (!string.IsNullOrWhiteSpace(description))
			description += "\n";
		else
			description = "";

		return description;
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(PNJinstance))]
public class PNJinstanceEditor : Editor
{
	PNJinstance m_Target;

	PNJbaseEditor m_PNJEditor;

	List<string> m_AvailableUsageType;
	SerializedProperty m_UsageEffectListProperty;

	void OnEnable() {
		m_Target = target as PNJinstance;
		//m_UsageEffectListProperty = serializedObject.FindProperty(nameof(UsableItem.UsageEffects));

		m_PNJEditor = new PNJbaseEditor();
		m_PNJEditor.Init(serializedObject);

		//var lookup = typeof(UsableItem.UsageEffect);
		//m_AvailableUsageType = System.AppDomain.CurrentDomain.GetAssemblies()
		//	.SelectMany(assembly => assembly.GetTypes())
		//	.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
		//	.Select(type => type.Name)
		//	.ToList();
	}

	public override void OnInspectorGUI() {
		m_PNJEditor.GUI(m_Target);
		serializedObject.ApplyModifiedProperties();
	}
}
#endif
