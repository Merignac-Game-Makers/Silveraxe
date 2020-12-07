using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
	public Transform[] points;
	private int destPoint = 0;
	private NavMeshAgent agent;

	public GuardMode guardMode = GuardMode.patrol;
	public Transform head;
	public float detectionRange = 5;
	public float detectionAngle = 60;

	RaycastHit[] hits = new RaycastHit[16];
	List<RaycastHit> hitsList;

	Character sentinel;
	GuardMode defMode;
	float defSpeed;


	public enum GuardMode { idle, patrol, attack }

	void Start() {
		sentinel = GetComponentInParent<Character>();
		agent = GetComponentInParent<NavMeshAgent>();

		// Disabling auto-braking allows for continuous movement
		// between points (ie, the agent doesn't slow down as it
		// approaches a destination point).
		agent.autoBraking = false;

		defMode = guardMode;                    // mémoriser le mode initial
		defSpeed = agent.speed;                 // mémoriser la vitesse initiale
												//if (guardMode == GuardMode.patrol)
												//	GotoNextPoint();
	}


	void GotoNextPoint() {

		if (points.Length == 0)                             // Returns if no points have been set up
			return;

		agent.destination = points[destPoint].position;     // Set the agent to go to the currently selected destination.
		destPoint = (destPoint + 1) % points.Length;        // Choose the next point in the array as the destination, cycling to the start if necessary.
	}


	void Update() {

		switch (guardMode) {
			case GuardMode.idle:                                                        // si la sentinelle est en poste fixe
				if (playerDetection())                                                      // si la sentinelle voit le joueur
					Attack();                                                               // attaquer
				break;
			case GuardMode.patrol:                                                      // si la sentinelle patrouille
				if (playerDetection())                                                      // si la sentinelle voit le joueur
					Attack();                                                               // attaquer
				else if (!agent.pathPending && agent.remainingDistance < agent.radius * 2)  // sinon, à l'approche d'un point de passage
					GotoNextPoint();                                                        // aller au point de passage suivant
				break;
			case GuardMode.attack:                                                      // si la sentinelle est en combat
				break;
		}

	}

	Vector3 playerVector;
	Vector3 dir;
	float sqrDistance;
	Vector3[] corners = new Vector3[8];
	Vector3 playerPos;
	bool playerDetection() {

		if (SceneModeManager.sceneMode != SceneMode.normal)         // détection seulement en mode 'normal'
			return false;

		playerPos = App.playerManager.transform.position;
		playerVector = playerPos - head.transform.position;         // vecteur jusqu'au joueur
		dir = playerVector.normalized;                                                          // direction du joueur
		sqrDistance = playerVector.sqrMagnitude;                                                // distance jusqu'au joueur (au carré)

		if (sqrDistance > detectionRange * detectionRange) {                                            // si le joueur est trop loin
			return false;                                                                               //		=> pas vu
		} else if (Vector3.Dot(transform.forward, dir) < Mathf.Cos(detectionAngle * Mathf.Deg2Rad)) {   // si le joueur est hors de l'angle de vision							
			return false;                                                                               //		=> pas vu
		} else {
			corners = App.playerManager.GetBounds();	// les coins de la 'bounding box' du joueur
			foreach (Vector3 corner in corners) {       // pour chaque coin
				var o = Physics.Raycast(head.transform.position, (corner - head.transform.position).normalized, playerVector.magnitude);
				if (!o) {			// s'il n'y a pas d'obstacle
					return true;	// on voit le joueur
				}
			}
			return false;
		}

	}

	void Attack() {
		agent.speed = 5;
		agent.autoBraking = true;

		guardMode = GuardMode.attack;
		agent.SetDestination(App.playerManager.ActPosition(sentinel, SceneMode.fight));
		StopCoroutine(IAttack());		// stopper le callback précédent (ne pas passer en mode combat à la position initiale du joueur s'il s'est déplacé)
		StartCoroutine(IAttack());		// engager le mode combat quand on a rattrapé le joueur

		IEnumerator IAttack() {
			while (playerDetection() && (agent.pathPending || agent.remainingDistance > agent.radius))
				yield return new WaitForEndOfFrame();

			if (sentinel.isInPlayerCollider) {																// si on est proche du joueur
				SceneModeManager.SetSceneMode(SceneMode.fight, true, GetComponentInParent<Character>());	//		engager le combat
			} else {																						// si le joueur s'est enfui
				RestoreInitialMode();																		//		reprendre la patrouille
			}
		}
	}

	public void RestoreInitialMode() {
		agent.autoBraking = false;
		agent.speed = defSpeed;
		guardMode = defMode;
		agent.destination = points[destPoint].position;
	}

}
