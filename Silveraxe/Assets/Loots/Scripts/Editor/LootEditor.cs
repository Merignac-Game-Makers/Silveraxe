using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Loot))]
public class LootEditor : Editor
{
	Loot m_Target;

	HighlightableEditor m_HighlightableEditor;

	SerializedProperty pName;
	SerializedProperty pIcon;
	SerializedProperty pDescription;
	SerializedProperty pPrefab;
	SerializedProperty pLootCategory;
	SerializedProperty pInteractionMode;
	SerializedProperty pCursor;
	SerializedProperty pAnimate;
	SerializedProperty pDropable;
	SerializedProperty pUsable;
	SerializedProperty pUsageEffectList;

	List<string> m_AvailableUsageType;

	void OnEnable() {
		m_Target = target as Loot;

		//m_Target.usable = false;
		//m_Target.UsageEffects.Clear();
		//serializedObject.Update();

		m_HighlightableEditor = CreateInstance<HighlightableEditor>();
		m_HighlightableEditor.Init(serializedObject);

		pName = serializedObject.FindProperty(nameof(Loot.ItemName));
		pIcon = serializedObject.FindProperty(nameof(Loot.ItemSprite));
		pDescription = serializedObject.FindProperty(nameof(Loot.Description));
		pPrefab = serializedObject.FindProperty(nameof(Loot.prefab));
		pLootCategory = serializedObject.FindProperty(nameof(Loot.lootCategory));
		pInteractionMode = serializedObject.FindProperty(nameof(Loot.mode));
		pCursor = serializedObject.FindProperty(nameof(Loot.cursor));
		pAnimate = serializedObject.FindProperty(nameof(Loot.animate));
		pDropable = serializedObject.FindProperty(nameof(Loot.dropable));
		pUsable = serializedObject.FindProperty(nameof(Loot.usable));
		pUsageEffectList = serializedObject.FindProperty(nameof(Loot.UsageEffects));

		var lookup = typeof(UsageEffect);
		m_AvailableUsageType = System.AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
			.Select(type => type.Name)
			.ToList();

	}

	public override void OnInspectorGUI() {
		//serializedObject.Update();

		EditorGUILayout.PropertyField(pName);

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

		EditorGUILayout.PropertyField(pIcon);
		EditorGUILayout.PropertyField(pDescription, GUILayout.MinHeight(64));
		EditorGUILayout.PropertyField(pLootCategory);
		EditorGUILayout.PropertyField(pInteractionMode);
		EditorGUILayout.PropertyField(pCursor);

		m_HighlightableEditor.GUI(target as Loot);

		EditorGUILayout.PropertyField(pAnimate);
		m_Target.dropable = EditorGUILayout.Toggle("Dropable", pDropable.boolValue);
		m_Target.usable = EditorGUILayout.Toggle("Usable", pUsable.boolValue);

		if (m_Target.usable) {
			int choice = EditorGUILayout.Popup("Add new Effect", -1, m_AvailableUsageType.ToArray());

			if (choice != -1) {
				var newInstance = ScriptableObject.CreateInstance(m_AvailableUsageType[choice]);

				pUsageEffectList.InsertArrayElementAtIndex(pUsageEffectList.arraySize);
				pUsageEffectList.GetArrayElementAtIndex(pUsageEffectList.arraySize - 1).objectReferenceValue = newInstance;
				serializedObject.ApplyModifiedProperties();

				return;
			}

			Editor ed = null;
			int toDelete = -1;
			for (int i = 0; i < pUsageEffectList.arraySize; ++i) {
				EditorGUILayout.BeginHorizontal();

				var item = pUsageEffectList.GetArrayElementAtIndex(i);
				if (item.objectReferenceValue) {
					EditorGUILayout.BeginVertical();
					CreateCachedEditor(item.objectReferenceValue, null, ref ed);
					ed.OnInspectorGUI();
					EditorGUILayout.EndVertical();

					if (GUILayout.Button("-", GUILayout.Width(32))) {
						toDelete = i;
					}

				}
				EditorGUILayout.EndHorizontal();
			}

			if (toDelete != -1) {
				var item = pUsageEffectList.GetArrayElementAtIndex(toDelete).objectReferenceValue;
				DestroyImmediate(item, true);

				//need to do it twice, first time just nullify the entry, second actually remove it.
				pUsageEffectList.DeleteArrayElementAtIndex(toDelete);
				pUsageEffectList.DeleteArrayElementAtIndex(toDelete);
			}

		}


		serializedObject.ApplyModifiedProperties();
	}
}