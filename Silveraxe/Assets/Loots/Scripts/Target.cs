using System.Collections.Generic;
using UnityEngine;

using static App;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Objet intéractible sur lequel on peut déposer un objet d'inventaire (loot)
/// </summary>
public class Target : InteractableObject
{
	public enum FilterMode { allow, refuse }

	public GameObject prefab;

	public FilterMode filterMode = FilterMode.allow;
	public List<LootCategory> filterItems;

	Transform prefabHolder;
	Transform target;                 

	public override bool IsInteractable() => isFree 
		&& (playerManager.m_InvItemDragging!=null || inventoryUI.selectedEntry!=null);         

	public bool isFree => !GetComponentInChildren<Loot>();      // ne peut contenir qu'un seul objet d'inventaire

	public Vector3 targetPos => target.position;


	public bool isAvailable(Loot item) {
		if (!isOn) return false;
		if (!isFree) return false;
		if (!item.dropable) return false;
		if (filterMode == FilterMode.allow && !filterItems.Contains(item.lootCategory)) return false;
		if (filterMode == FilterMode.refuse && filterItems.Contains(item.lootCategory)) return false;
		return true;
	}

	protected override void Start() {
		base.Start();
		// récupération des objets clés
		target = transform.Find("Target");
		prefabHolder = transform.Find("PrefabHolder");
		// masquage de la cible
		target.gameObject.SetActive(false);
		// ajout du navmesh obstacle
		if (prefabHolder) {
			var obj = prefabHolder.GetComponentInChildren<MeshFilter>();
			if (obj != null)
				obj.gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();
		}
	}

	private void Update() {
		if (Input.GetButtonDown("Fire1")) {
			if (inventoryUI.selectedEntry != null) {
				if (isAvailable(inventoryUI.selectedEntry.item)) {
					inventoryUI.selectedEntry.item.Drop(this);
					Highlight(false);
				}
			}
		}
	}


	//public override void OnMouseUp() {
	//	base.OnMouseUp();
	//	if (InventoryUI.Instance.selectedEntry != null) {
	//		if (isAvailable(InventoryUI.Instance.selectedEntry.item)) {
	//			InventoryUI.Instance.selectedEntry.item.Drop(this);
	//			Highlight(false);
	//		}
	//	}
	//}

}



#if UNITY_EDITOR
[CustomEditor(typeof(Target))]
public class TargetEditor : Editor
{
	Target m_Target;

	SerializedProperty pPrefab;
	SerializedProperty pFilterMode;
	SerializedProperty pFilterItems;

	void OnEnable() {
		m_Target = target as Target;

		//m_Target.usable = false;
		//m_Target.UsageEffects.Clear();
		//serializedObject.Update();

		pPrefab = serializedObject.FindProperty(nameof(Target.prefab));
		pFilterMode = serializedObject.FindProperty(nameof(Target.filterMode));
		pFilterItems = serializedObject.FindProperty(nameof(Target.filterItems));

	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		var oldPrefab = serializedObject.FindProperty(nameof(Target.prefab)).objectReferenceValue;
		EditorGUILayout.PropertyField(pPrefab);
		var newPrefab = serializedObject.FindProperty(nameof(Target.prefab)).objectReferenceValue;
		if (newPrefab != null && (oldPrefab == null || newPrefab.name != oldPrefab.name)) {
			Debug.Log("change prefab");
			var holder = m_Target.transform.Find("PrefabHolder");
			foreach (Transform t in holder) {
				DestroyImmediate(t.gameObject);
			}
			var obj = Instantiate(serializedObject.FindProperty(nameof(Loot.prefab)).objectReferenceValue, holder) as GameObject;
			obj.layer = holder.gameObject.layer;
		}
		EditorGUILayout.PropertyField(pFilterMode);
		EditorGUILayout.PropertyField(pFilterItems);

		serializedObject.ApplyModifiedProperties();
	}
}
#endif