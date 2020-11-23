using System;
using UnityEngine;
using System.Collections;

using static InteractableObject.Action;
using static App;

/// <summary>
/// Classe générique pour les PNJ
/// => Intéraction par défaut = interrompre le déplacement + lancer le dialogue
/// </summary>
/// 
[Serializable]
public class PNJ : Character
{
	public enum Alignment { friend, neutral, ennemy }    // comportements possibles (ami / neutre / ennemi)

	public string PNJName;                          // nom du PNJ (pour les dialogues)
	public int onTheFlyRadius = 1;                  // rayon d'action du mode onTheFlyOnce
	public Alignment alignment = Alignment.neutral; // comportement

	public GameObject PNJcam { get; private set; }

	float initialRadius = 1.5f;
	CapsuleCollider cCollider;

	DialogueTrigger dialogueTrigger;


	public override bool IsInteractable() => isInPlayerCollider;

	protected override void Start() {
		base.Start();

		PNJcam = GetComponentInChildren<Camera>(true).gameObject;       // récupérer la caméra pour les dialogues
		dialogueTrigger = GetComponentInChildren<DialogueTrigger>();     // si le PNJ a-t-il un dialogue trigger

		SetAlignmentActionSprite();

		if (mode != Mode.onClick) {                                     // si le mode est onTheFlyOnce
			cCollider = gameObject.GetComponent<CapsuleCollider>();
			if (cCollider && onTheFlyRadius > cCollider.radius) {       // et qu'il y a un CapsuleCollider
				initialRadius = cCollider.radius;                       //	=> mettre en place le rayon élargi
				SetColliderRadius(onTheFlyRadius);
			}
		}
	}

	void Update() {
		// bouton d'action
		if (Input.GetButtonDown("Fire1")) {
			if (!interactableObjectsManager.MultipleSelection()) {
				Act();
			}
		}
	}

	private void OnMouseUp() {
		//if (isMouseOver)
		Act();
	}

	private void Act() {
		if (IsInteractable() && !uiManager.isClicOnUI) {
			switch (alignment) {
				case Alignment.friend:
					Talk();
					break;
				case Alignment.neutral:
					Talk();
					break;
				case Alignment.ennemy:
					if (!isInFightMode)
						EnterFightMode(true);
					else {
						playerManager.Fight(1);
					}
					break;
			}
		}
	}

	void SetAlignmentActionSprite() {
		switch (alignment) {
			case Alignment.friend:
				SetColor(Color.green);
				SetActionSprite(interactableObjectsManager.dialogueIcon);
				break;
			case Alignment.neutral:
				SetColor(Color.white);
				SetActionSprite(interactableObjectsManager.dialogueIcon);
				break;
			case Alignment.ennemy:
				SetColor(Color.red);
				SetActionSprite(interactableObjectsManager.fightIcon);
				break;
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


	private void Talk() {
		if (dialogueTrigger.HasDialogue()) {                                // si le PNJ a un dialogue
			playerManager.StopAgent();                                      //	stopper la navigation
			playerManager.FaceTo(gameObject);
			GetComponentInChildren<DialogueTrigger>().Run();                //	démarrer le dialogue	
		}
	}


	// intéraction avec un PNJ
	//public override void InteractWith(CharacterData character, Action action = talk, HighlightableObject target = null) {
	//	if (dialogueTrigger.HasDialogue()) {                                // si le PNJ a un dialogue
	//		playerManager.StopAgent();                                      //	stopper la navigation
	//		GetComponentInChildren<DialogueTrigger>().Run();                //	démarrer le dialogue	

	//		if (mode == Mode.onTheFlyOnce) {                                            // si le PNJ est en mode 'onTheFlyOnce'
	//																					//Collider dtc = dialogueTrigger.gameObject.GetComponent<Collider>();   // et qu'il existe un collider spécifique de ce mode
	//			ResetColliderRadius();                                                  // restaurer le rayon initial du collider (pour éviter à l'avenir une intéraction sur un rayon élargi)
	//		}
	//	}

	//	base.InteractWith(character, target, action);                      // intéraction par défaut
	//}
}