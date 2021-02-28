using UnityEngine;
using VIDE_Data;

using static App;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(VIDE_Assign))]
public class DialogueManager : MonoBehaviour {
	protected VIDE_Assign dialogue;
	PNJ pnj;

	protected virtual void Start() {
		VD.isActive = false;

		dialogue = GetComponent<VIDE_Assign>();									// le dialogue
		pnj = gameObject.GetComponentInParent<PNJ>();                           // le PNJ
	}

	public void Run() {
		if (!VD.isActive) {
			SetStartNode();														// définir le point d'entrée
			dialogueUI.Begin(pnj, dialogue);                                    // commencer le dialogue
		}
	}

	public virtual void SetStartNode() {
		dialogue.overrideStartNode = 0;
	}


}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor {

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
