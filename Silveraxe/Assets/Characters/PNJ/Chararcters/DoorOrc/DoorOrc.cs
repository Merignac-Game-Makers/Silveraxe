using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorOrc : MonoBehaviour
{
	public Animator doorAnim;
	public Character doorOrc;
	public DoorSensor sensor;

	public Loot loot;

	public Transform safePoint;
	public Transform actionPoint;

	public bool freezeRotation = false;

	NavMeshAgent orcAgent;
	Quaternion initialRotation;


	private void Start() {
		orcAgent = doorOrc.GetComponent<NavMeshAgent>();
		initialRotation = orcAgent.transform.rotation;
		orcAgent.updateRotation = !freezeRotation;

		safePoint.gameObject.SetActive(false);
		actionPoint.gameObject.SetActive(false);
	}

	public void GotoActionPoint() {
		if (doorOrc.isAlive)
			doorOrc.navAgent.SetDestination(actionPoint.position);
	}

	public void GotoSafePoint() {
		if (doorOrc.isAlive)
			doorOrc.navAgent.SetDestination(safePoint.position);
	}

	public void OpenDoor() {
		if (doorOrc.isAlive)
			doorAnim.SetTrigger("Open");
	}

	public void CloseDoor() {
		if (doorOrc.isAlive) {
			doorOrc.navAgent.SetDestination(safePoint.position);
			StartCoroutine(Iin());
		}
	}
	IEnumerator Iin() {
		while (orcAgent.pathPending || orcAgent.remainingDistance > orcAgent.radius) {
			yield return null;
		}
		doorAnim.SetTrigger("Close");
	}

	public void ReorientOrc() {
		if (!freezeRotation)
			doorOrc.transform.rotation = initialRotation;
	}

	public void ToggleDetectorThreshold() {
		sensor.ToggleThreshold();
	}

	public void LootEnabled(int on){
		loot.gameObject.SetActive(on==1);
		doorOrc.enabled = on==1;
	}
}

