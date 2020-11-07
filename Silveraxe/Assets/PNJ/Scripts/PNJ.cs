using System;
using UnityEngine;
using System.Collections.Generic;
using static InteractableObject.Action;

#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Classe générique pour les PNJ
/// => Intéraction par défaut = interrompre le déplacement + lancer le dialogue
/// </summary>
/// 
[Serializable]
public class PNJ : InteractableObject
{
	public string PNJName;          // nom du PNJ (pour les dialogues)
	public int onTheFlyRadius;  // rayon d'action du mode onTheFlyOnce

	[HideInInspector]
	public GameObject PNJcam;

	float initialRadius = 1.5f;
	CapsuleCollider cCollider;

	public override bool IsInteractable() => true;

	protected override void Start() {
		base.Start();
		//var dialogue = gameObject.GetComponentInChildren<VIDE_Assign>();
		//if (dialogue != null) {									// s'il y a un dialogue
		//if (image != null) {								//		et un image pour le PNJ
		//	dialogue.defaultNPCSprite = image;				//		=> affecter l'image du PNJ au dialogue
		//}
		//if (!string.IsNullOrEmpty(PNJName)) {				//		et un nom
		//	dialogue.alias = PNJName;						//		=> affecter le nom du PNJ au dialogue
		//}
		//}
		PNJcam = GetComponentInChildren<Camera>(true).gameObject;			// récupérer la caméra pour les dialogues
		if (mode != Mode.onClick) {										// si le mode est onTheFlyOnce
			cCollider = gameObject.GetComponent<CapsuleCollider>();		
			if (cCollider && onTheFlyRadius> cCollider.radius) {		// et qu'il y a un CapsuleCollider
				initialRadius = cCollider.radius;						//	=> mettre en place le rayon élargi
				SetColliderRadius(onTheFlyRadius);
			}
		}
	}

	void SetColliderRadius(float radius) {
		cCollider.radius = radius;
	}

	public void ResetColliderRadius() {
		SetColliderRadius(initialRadius);
	}

	public void resetToOnClickMode() {
		ResetColliderRadius();
		mode = Mode.onClick;
	}

	public void DisableCollider() {
		cCollider.enabled = false;
		cCollider.isTrigger = false;
	}

	// intéraction avec un PNJ
	public override void InteractWith(CharacterData character, HighlightableObject target = null, Action action = talk) {
		DialogueTrigger dt = GetComponentInChildren<DialogueTrigger>();     // si le PNJ a un dialogue
		if (dt) {
			PlayerManager.Instance.StopAgent();                             //	stopper la navigation
			GetComponentInChildren<DialogueTrigger>().Run();                //	démarrer le dialogue	

			if (mode == Mode.onTheFlyOnce) {                                // si le PNJ est en mode 'onTheFlyOnce'
				Collider dtc = dt.gameObject.GetComponent<Collider>();      // et qu'il existe un collider spécifique de ce mode
				ResetColliderRadius();										// restaurer le rayon initial du collider (pour éviter à l'avenir une intéraction sur un rayon élargi)
			}
		}

		base.InteractWith(character, target, action);                      // intéraction par défaut
	}

}

#if UNITY_EDITOR
[CustomEditor(typeof(PNJ))]
[CanEditMultipleObjects]
public class PNJEditor : Editor
{
	SerializedProperty p_mode;
	SerializedProperty p_radius;
	SerializedProperty p_PNJName;

	PNJ m_PNJ;

	public void OnEnable() {
		m_PNJ = (PNJ)target;
		p_mode = serializedObject.FindProperty(nameof(m_PNJ.mode));
		p_PNJName = serializedObject.FindProperty(nameof(m_PNJ.PNJName));
		p_radius = serializedObject.FindProperty(nameof(m_PNJ.onTheFlyRadius));
	}


	public override void OnInspectorGUI() {
		EditorStyles.textField.wordWrap = true;
		serializedObject.Update();

		EditorGUILayout.PropertyField(p_mode);
		if (m_PNJ.mode != InteractableObject.Mode.onClick) {
			EditorGUILayout.PropertyField(p_radius);
		}
		EditorGUILayout.PropertyField(p_PNJName);
		EditorGUILayout.PropertyField(p_radius);

		serializedObject.ApplyModifiedProperties();
	}
	void OnInspectorUpdate() {
		Repaint();
	}
}
#endif
