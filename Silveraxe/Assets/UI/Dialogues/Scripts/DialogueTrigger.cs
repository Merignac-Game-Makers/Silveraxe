﻿using UnityEngine;
using VIDE_Data;

using static App;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(VIDE_Assign))]
public class DialogueTrigger : MonoBehaviour
{
	public Texture2D dialogueCursor;

	VIDE_Assign dialogue;
	DialogueDispatcher dispatcher;
	PNJ pnj;

	void Start() {
		VD.isActive = false;

		dialogue = gameObject.GetComponent<VIDE_Assign>();                      // le dialogue
		dispatcher = gameObject.GetComponentInChildren<DialogueDispatcher>();   // le script de validation de dialogue (points d'entrée en fonction du statut de la quête [si elle existe])
		pnj = gameObject.GetComponentInParent<PNJ>();                           // le PNJ
	}

	public void Run() {
		if (!VD.isActive) {
			if (dispatcher != null) {
				dispatcher.SetStartNode();
			}
			dialogueUI.Begin(dialogue);                         // commencer le dialogue
		}
	}

	public bool HasDialogue() {
		return dispatcher ? dispatcher.HasDialogue() : false;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueTrigger))]
public class DialogTriggerEditor : Editor
{

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		GUILayout.Label(
@"Attention !
Pour personnaliser le dialogue : 
     - affecter un dialoque dans le composant VIDE_Assign
     - ajouter un DialogValidation spécifique du dialogue");

	}
}
#endif
