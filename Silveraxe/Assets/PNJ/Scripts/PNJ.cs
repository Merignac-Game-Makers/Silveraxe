using System;
using UnityEngine;
using System.Collections.Generic;
using static InteractableObject.Action;
using System.Collections;
using UnityEngine.Animations;

using static App;

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
	public enum Alignment { friend, neutral, ennemy }    // comportements possibles

	public string PNJName;                          // nom du PNJ (pour les dialogues)
	public int onTheFlyRadius = 1;                  // rayon d'action du mode onTheFlyOnce
	public Alignment alignment = Alignment.neutral; // comportement

	[HideInInspector]
	public GameObject PNJcam;

	float initialRadius = 1.5f;
	CapsuleCollider cCollider;
	float timer = 0f;

	DialogueTrigger dialogueTrigger;
	DialogueDispatcher dialogueDispatcher;

	public override bool IsInteractable() => true;

	protected override void Start() {
		base.Start();
		PNJcam = GetComponentInChildren<Camera>(true).gameObject;       // récupérer la caméra pour les dialogues
		dialogueTrigger = GetComponentInChildren<DialogueTrigger>();     // si le PNJ a-t-il un dialogue trigger
		dialogueDispatcher = GetComponentInChildren<DialogueDispatcher>();

		switch (alignment) {
			case Alignment.friend:
				SetColor(Color.green);
				break;
			case Alignment.neutral:
				SetColor(Color.white);
				break;
			case Alignment.ennemy:
				SetColor(Color.red);
				break;
		}

		if (mode != Mode.onClick) {                                     // si le mode est onTheFlyOnce
			cCollider = gameObject.GetComponent<CapsuleCollider>();
			if (cCollider && onTheFlyRadius > cCollider.radius) {       // et qu'il y a un CapsuleCollider
				initialRadius = cCollider.radius;                       //	=> mettre en place le rayon élargi
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

	void OnMouseEnter() {
		uiManager.SetCursor( dialogueTrigger.dialogueCursor);
	}

	private void OnMouseExit() {
		uiManager.ResetCursor();
	}

	public override bool Highlight(bool on) {
		return base.Highlight(on);
	}

	// intéraction avec un PNJ
	public override void InteractWith(CharacterData character, HighlightableObject target = null, Action action = talk) {
		if (dialogueTrigger.HasDialogue()) {								// si le PNJ a un dialogue
			playerManager.StopAgent();										//	stopper la navigation
			GetComponentInChildren<DialogueTrigger>().Run();                //	démarrer le dialogue	

			if (mode == Mode.onTheFlyOnce) {											// si le PNJ est en mode 'onTheFlyOnce'
				//Collider dtc = dialogueTrigger.gameObject.GetComponent<Collider>();   // et qu'il existe un collider spécifique de ce mode
				ResetColliderRadius();													// restaurer le rayon initial du collider (pour éviter à l'avenir une intéraction sur un rayon élargi)
			}
		}

		base.InteractWith(character, target, action);                      // intéraction par défaut
	}

	Coroutine coroutine;
	public void FaceTo(GameObject other) {

		var delta =  other.transform.position - transform.position;
		delta.y = 0;
		var rotation = Quaternion.LookRotation(delta);

			if (coroutine != null)
				StopCoroutine(coroutine);
			coroutine = StartCoroutine(IFaceTo(rotation, 1));
		}
	
	IEnumerator IFaceTo(Quaternion rot, float s) {
		timer = 0;
		while (timer <= s) {
			timer += Time.deltaTime;
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, timer);
			yield return new WaitForEndOfFrame();
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PNJ))]
[CanEditMultipleObjects]
public class PNJEditor : Editor
{
	SerializedProperty p_mode;
	SerializedProperty p_alignment;
	SerializedProperty p_radius;
	SerializedProperty p_PNJName;

	PNJ m_PNJ;

	public void OnEnable() {
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
	void OnInspectorUpdate() {
		Repaint();
	}
}
#endif
