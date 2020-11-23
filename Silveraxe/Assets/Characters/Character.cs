using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

using static App;

public abstract class Character : InteractableObject
{
	// pour les dialogues
	Camera portraitCamera;
	public Transform portraitCameraTarget;
	string[] targetNames = new string[] { "head" };

	// pour les combats
	protected bool isInFightMode = false;


	protected override void Start() {
		base.Start();

		// pour les dialogues
		portraitCamera = GetComponentInChildren<Camera>(true);
		GetTarget();
		if (portraitCameraTarget) {
			var lookAt = portraitCamera.GetComponent<LookAtConstraint>();
			var aim = new ConstraintSource() { sourceTransform = portraitCameraTarget, weight = 1 };
			if (lookAt.sourceCount == 0) {
				lookAt.AddSource(aim);
			} else {
				lookAt.SetSource(0, aim);
			}
		}

	}

	public override bool Highlight(bool on) {
		if (!on)
			EnterFightMode(false); ;
		return base.Highlight(on);
	}

	void GetTarget() {
		foreach (string name in targetNames) {
			portraitCameraTarget = transform.FindDeepChild(name);
			if (portraitCameraTarget)
				break;
		}
	}

	/// <summary>
	/// en mode combat, tous les colliders sont désactivés sauf le fightCollider... et vice-versa
	/// </summary>
	/// <param name="on"></param>
	protected void EnterFightMode(bool on) {
		isInFightMode = on;
		FaceTo(playerManager.gameObject);
		playerManager.FaceTo(gameObject);
		interactableObjectsManager.showActionSprites(on);
	}


	public void Fight(int attack) {
		animatorController.anim.SetInteger("Fight", attack);
	}
}
