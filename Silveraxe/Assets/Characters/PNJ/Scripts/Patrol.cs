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

	FightController fightController;

	RaycastHit[] hits = new RaycastHit[16];
	List<RaycastHit> hitsList;

	public enum GuardMode { idle, patrol, attack }

	void Start() {
		agent = GetComponentInParent<NavMeshAgent>();
		fightController = GetComponent<FightController>();

		// Disabling auto-braking allows for continuous movement
		// between points (ie, the agent doesn't slow down as it
		// approaches a destination point).
		agent.autoBraking = false;
		if (guardMode == GuardMode.patrol)
			GotoNextPoint();
	}


	void GotoNextPoint() {
		// Returns if no points have been set up
		if (points.Length == 0)
			return;

		// Set the agent to go to the currently selected destination.
		agent.destination = points[destPoint].position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % points.Length;
	}


	void Update() {
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (guardMode == GuardMode.patrol  && !agent.pathPending && agent.remainingDistance < agent.radius * 2)
			GotoNextPoint();

		if (playerDetection() && SceneModeManager.sceneMode != SceneMode.fight) {
			Attack();
		}
	}

	bool playerDetection() {
		// detection
		// Cast a sphere wrapping character controller 10 meters forward
		// to see if it is about to hit anything.
		int hitCount = Physics.SphereCastNonAlloc(head.transform.position, .5f, head.transform.forward, hits, detectionRange);
		if (hitCount > 1) {
			RaycastHit[] tmp = new RaycastHit[hitCount - 1];
			Array.Copy(hits, 1, tmp, 0, hitCount - 1);
			hitsList = new List<RaycastHit>(tmp);
			hitsList.Sort(SortByDistance);
			if (hitsList[0].collider.gameObject == App.playerManager.gameObject) {
				Vector3 dir = (hitsList[0].point - head.transform.position).normalized;
				return Vector3.Dot(head.transform.forward, dir) >= Mathf.Cos(detectionAngle * Mathf.Deg2Rad);
			}
			return false;
		}
		return false;
	}

	void Attack() {
		guardMode = GuardMode.attack;
		agent.speed = 5;
		agent.SetDestination(App.playerManager.ActPosition(GetComponentInParent<Character>()), () => {
			SceneModeManager.SetSceneMode(SceneMode.fight, true, GetComponentInParent<Character>());
		});

	}


	int SortByDistance(RaycastHit p1, RaycastHit p2) {
		return p1.distance.CompareTo(p2.distance);
	}
}
