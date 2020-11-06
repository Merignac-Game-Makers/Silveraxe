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
			// supprimer l'avatar du PNJ précédent
			foreach (Transform c in dialoguesUI.NPC_PrefabHolder.transform)
				Destroy(c.gameObject);
			// mettre en place l'avatar du PNJ
			Instantiate(pnj, dialoguesUI.NPC_PrefabHolder.transform);
			var constraintSources = new List<UnityEngine.Animations.ConstraintSource>();
			var aim = new UnityEngine.Animations.ConstraintSource();
			aim.sourceTransform = pnj.CameraTarget.transform;
			constraintSources.Add(aim);
			dialoguesUI.NPC_Camera.SetSources(constraintSources);
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
