using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

using static App;

public abstract class Character : InteractableObject
{

	// CharacterData
	[HideInInspector]
	public CharacterData characterData;                           // caractéristiques du personnage (santé, force..., inventaire, ...)

	// navigation
	[HideInInspector]
	public NavMeshAgent navAgent;                                   // agent de navigation
	protected bool MoveAcrossNavMeshesStarted = false;              // flag : est-on sur un nav mesh link ? (pour gérer la vitesse)

	// animation
	public NavAnimController animatorController { get; set; }
	LookAtConstraint lookAt;

	// pour les dialogues
	Camera portraitCamera;
	public Transform portraitCameraTarget { get; set; }
	string[] targetNames = new string[] { "head" };

	// pour les combats
	public bool isInFightMode => fightController?.isInFightMode ?? false;
	public FightController fightController { get; set; }
	public bool isAlive => fightController.isAlive;

	protected override void Start() {
		base.Start();

		// pour les déplacements
		navAgent = GetComponentInChildren<NavMeshAgent>();                     // préparation de la navigation

		// pour les dialogues
		//portraitCamera = GetComponentInChildren<Camera>(true);
		//GetPortraitTarget();
		//if (portraitCameraTarget) {
		//	var pLookAt = portraitCamera.GetComponent<LookAtConstraint>();
		//	var aim = new ConstraintSource() { sourceTransform = portraitCameraTarget, weight = 1 };
		//	if (pLookAt.sourceCount == 0) {
		//		pLookAt.AddSource(aim);
		//	} else {
		//		pLookAt.SetSource(0, aim);
		//	}
		//}

		// pour les animations
		animatorController = GetComponentInChildren<NavAnimController>();
		lookAt = GetComponent<LookAtConstraint>();
		if (!lookAt)
			lookAt = gameObject.AddComponent<LookAtConstraint>();
		if (lookAt.sourceCount == 0) {
			lookAt.AddSource(new ConstraintSource());
		}

		// pour les combats
		fightController = GetComponentInChildren<FightController>();
		characterData = GetComponent<CharacterData>();              // caractéristiques du joueur
		characterData.Init();                                       // ... initialisation

	}

	void GetPortraitTarget() {
		foreach (string name in targetNames) {
			portraitCameraTarget = transform.FindDeepChild(name);
			if (portraitCameraTarget)
				break;
		}
	}

	public abstract void Act();

	public void FaceTo(bool on, GameObject other = null) {
		lookAt.constraintActive = on;
		if (on && other != null) {
			lookAt.SetSource(0, new ConstraintSource() { sourceTransform = other.transform, weight = 1 });
		}
	}

	public Vector3 ActPosition(Character other) {
		var dir = other.transform.position - transform.position;
		var dist = (navAgent.radius + other.navAgent.radius) * 1.5f;
		Vector3 pos = other.transform.position - dir * (dist / dir.magnitude);
		return pos;
	}
}
