using System;
using UnityEngine;
using System.Collections;

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

	float initialRadius = 1.5f;
	CapsuleCollider cCollider;

	DialogueManager dialogueManager;

	PlayerManager player => App.playerManager;


	public override bool IsInteractable() {
		if (!enabled) return false;
		if (!player.isAlive) return false;
		if (!isClosest) return false;
		switch (SceneModeManager.sceneMode) {
			case SceneMode.normal:
				if (alignment == Alignment.ennemy)
					return isAlive;
				return true;
			case SceneMode.dialogue:
				return false;
			case SceneMode.fight:
				return alignment == Alignment.ennemy && isAlive;
		}
		return false;
	}

	protected override void Start() {
		base.Start();

		dialogueManager = GetComponentInChildren<DialogueManager>();     // si le PNJ a-t-il un dialogue trigger

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
		if (Input.GetButtonDown("Fire1") && !App.uiManager.isClicOnUI) {
			Act();
		}
	}

	public override void Act() {
		if (isOn && IsInteractable()) {
			switch (alignment) {
				case Alignment.friend:
					Talk();
					break;
				case Alignment.neutral:
					Talk();
					break;
				case Alignment.ennemy:
					if (!isInFightMode && player.isAlive) {
						SceneModeManager.SetSceneMode(SceneMode.fight, true, this);
					}  else {
						player.fightController.Fight_Attack();
					}
					break;
			}
		}
	}

	void SetAlignmentActionSprite() {
		switch (alignment) {
			case Alignment.friend:
				SetColor(Color.green);
				SetActionSprite(App.uiManager.dialogueIcon);
				break;
			case Alignment.neutral:
				SetColor(Color.white);
				SetActionSprite(App.uiManager.dialogueIcon);
				break;
			case Alignment.ennemy:
				SetColor(Color.red);
				SetActionSprite(App.uiManager.fightIcon);
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
		if (dialogueManager) {                                                        // si le PNJ a un dialogue
			SceneModeManager.SetSceneMode(SceneMode.dialogue, true, this);            // joueur en mode 'dialogue'
		}
	}
}