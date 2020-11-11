using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;
using static InteractableObject.Action;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Describes an InteractableObject that can be picked up and grants a specific item when interacted with.
///
/// It will also play a small animation (object going in an arc from spawn point to a random point around) when the
/// object is actually "spawned", and the object becomes interactable only when that animation is finished.
///
/// Finally it will notify the LootUI that a new loot is available in the world so the UI displays the name.
/// </summary>
public class Loot : InteractableObject
{
	static float AnimationTime = 0.1f;

	public string ItemName;
	public GameObject prefab;
	public Sprite ItemSprite;
	public string Description;
	public bool animate = true;
	public bool dropable = true;
	public LootCategory lootCategory;
	public bool usable = false;
	public List<UsageEffect> UsageEffects;


	public static InventoryUI inventoryUI;
	public InventoryEntry entry { get; set; } = null;                             // L'entrée d'inventaire lorsque l'objet a été ramassé

	public override bool IsInteractable() {                         // l'objet est intéractif si
		return !animate || m_AnimationTimer >= AnimationTime;       // l'animation de mise en place est terminée ou désactivée
	}

	Vector3 m_OriginalPosition;
	Vector3 m_TargetPoint;
	float m_AnimationTimer = 0.0f;

	public Loot(Loot item) {
		ItemName = item.ItemName;
		prefab = item.prefab;
		ItemSprite = item.ItemSprite;
		Description = item.Description;
		animate = item.animate;
		dropable = item.dropable;
		lootCategory = item.lootCategory;
		usable = item.usable;
		UsageEffects = item.UsageEffects;
	}

	void Awake() {
		m_OriginalPosition = transform.position;                    // préparation
		m_TargetPoint = transform.position;                         // de l'animation
		m_AnimationTimer = AnimationTime - 0.1f;                    // de mise en place
	}

	protected override void Start() {
		base.Start();
		inventoryUI = InventoryUI.Instance;
	}


	void Update() {
		// animation de mise en place
		if (animate && m_AnimationTimer < AnimationTime) {
			m_AnimationTimer += Time.deltaTime;
			float ratio = Mathf.Clamp01(m_AnimationTimer / AnimationTime);
			Vector3 currentPos = Vector3.Lerp(m_OriginalPosition, m_TargetPoint, ratio);
			currentPos.y = currentPos.y + Mathf.Sin(ratio * Mathf.PI) * 2.0f;
			transform.position = currentPos;
		}
	}

	public void StartAnimation() {
		m_OriginalPosition = transform.position;                    // préparation
		m_TargetPoint = transform.position;                         // de l'animation
		m_AnimationTimer = AnimationTime - 0.1f;                    // de mise en place
	}

	/// <summary>
	/// Ramasser / déposer un objet
	/// </summary>
	/// <param name="character">le personnage (joueur, PNJ, ...)</param>
	/// <param name="target">le lieu (lorsqu'on pose un objet)</param>
	/// <param name="action">l'action : prendre ou poser</param>
	public override void InteractWith(CharacterData character, HighlightableObject target = null, Action action = take) {
		base.InteractWith(character, target, action);

		if (action == take) {
			// si on ramasse l'objet
			SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });
			InventoryManager.Instance.AddItem(this);
		} else {
			// si on dépose l'objet sur une cible
			if (action == drop && target is Target) {
				if ((target as Target).isAvailable(this)) {                // et que cet emplacement est disponible pour cet objet
					Drop(target as Target);
					//inventoryUI.DropItem(target as Target, entry);         // déposer l'objet d'inventaire
				}
			}
		}
	}

	/// <summary>
	/// Déposer un objet d'inventaire
	/// </summary>
	/// <param name="target">le lieu</param>
	/// <param name="entry">l'entrée d'inventaire </param>
	public void Drop(Target target) {
		animate = true;
		transform.position = target.targetPos;
		StartAnimation();
		InventoryManager.Instance.RemoveItem(entry);       // retirer l'objet déposé de l'inventaire
	}


}





#if UNITY_EDITOR
[CustomEditor(typeof(Loot))]
public class LootEditor : Editor
{
	Loot m_Target;

	HighlightableEditor m_HighlightableEditor;

	SerializedProperty pName;
	SerializedProperty pIcon;
	SerializedProperty pDescription;
	SerializedProperty pPrefab;
	SerializedProperty pAnimate;
	SerializedProperty pDropable;
	SerializedProperty pLootCategory;
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
		pPrefab = serializedObject.FindProperty(nameof(Loot.prefab));
		pIcon = serializedObject.FindProperty(nameof(Loot.ItemSprite));
		pDescription = serializedObject.FindProperty(nameof(Loot.Description));
		pAnimate = serializedObject.FindProperty(nameof(Loot.animate));
		pDropable = serializedObject.FindProperty(nameof(Loot.dropable));
		pLootCategory = serializedObject.FindProperty(nameof(Loot.lootCategory));
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

		m_HighlightableEditor.GUI(target as Loot);

		EditorGUILayout.PropertyField(pIcon);
		EditorGUILayout.PropertyField(pName);
		EditorGUILayout.PropertyField(pDescription, GUILayout.MinHeight(128));

		var oldPrefab = serializedObject.FindProperty(nameof(Loot.prefab)).objectReferenceValue;
		EditorGUILayout.PropertyField(pPrefab);
		var newPrefab = serializedObject.FindProperty(nameof(Loot.prefab)).objectReferenceValue;
		if (newPrefab != null && (oldPrefab == null || newPrefab.name != oldPrefab.name)) {
			Debug.Log("change prefab");
			var holder = m_Target.transform.Find("PrefabHolder");
			foreach (Transform t in holder) {
				DestroyImmediate(t.gameObject);
			}
			var obj = Instantiate(serializedObject.FindProperty(nameof(Loot.prefab)).objectReferenceValue, holder) as GameObject;
			obj.layer = holder.gameObject.layer;
		}

		EditorGUILayout.PropertyField(pAnimate);
		m_Target.dropable = EditorGUILayout.Toggle("Dropable", pDropable.boolValue);
		EditorGUILayout.PropertyField(pLootCategory);
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
#endif