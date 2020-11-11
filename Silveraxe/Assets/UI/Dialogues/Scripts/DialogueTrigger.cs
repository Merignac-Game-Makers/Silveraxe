using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using VIDE_Data;

public class DialogueTrigger : MonoBehaviour
{

	DialoguesUI dialoguesUI;
	VIDE_Assign dialogue;
	DialogueDispatcher dispatcher;
	PNJ pnj;

	void Start() {
		dialoguesUI = DialoguesUI.Instance;                                     // le gestionnaire d'interface de dialogues
		dialogue = gameObject.GetComponent<VIDE_Assign>();                      // le dialogue
		dispatcher = gameObject.GetComponent<DialogueDispatcher>();             // le script de validation de dialogue (points d'entrée en fonction du statut de la quête [si elle existe])
		pnj = gameObject.GetComponentInParent<PNJ>();							// le PNJ
	}

	//// Update is called once per frame
	//void Update() {
	//	if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
	//		dialoguesUI.End(null);
	//	}
	//}

	public void Run() {
		if (!VD.isActive) {
			if (dispatcher != null) {
				dispatcher.SetStartNode();
			}
			//// activer la caméra du PNJ
			//pnj.PNJcam.SetActive(true);

			pnj.FaceTo(PlayerManager.Instance.gameObject);
			dialoguesUI.Begin(dialogue);
		} 
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueTrigger))]
public class DialogTriggerEditor : Editor
{

	public override void OnInspectorGUI() {

		GUILayout.Label(
@"Attention !
Pour personnaliser le dialogue : 
     - affecter un dialoque dans le composant VIDE_Assign
     - ajouter un DialogValidation spécifique du dialogue");

	}
}
#endif
