using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using VIDE_Data;

[RequireComponent(typeof(VIDE_Assign))]
public class DialogueTrigger : MonoBehaviour
{
	public Texture2D dialogueCursor;

	DialoguesUI dialoguesUI;
	VIDE_Assign dialogue;
	DialogueDispatcher dispatcher;
	PNJ pnj;

	void Start() {
		dialoguesUI = DialoguesUI.Instance;                                     // le gestionnaire d'interface de dialogues
		dialogue = gameObject.GetComponent<VIDE_Assign>();                      // le dialogue
		dispatcher = gameObject.GetComponentInChildren<DialogueDispatcher>();   // le script de validation de dialogue (points d'entrée en fonction du statut de la quête [si elle existe])
		pnj = gameObject.GetComponentInParent<PNJ>();							// le PNJ
	}

	public void Run() {
		if (!VD.isActive) {
			if (dispatcher != null) {
				dispatcher.SetStartNode();
			}
			pnj.FaceTo(PlayerManager.Instance.gameObject);		// orienter le PNJ vers le joueur
			dialoguesUI.Begin(dialogue);						// commencer le dialogue
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
