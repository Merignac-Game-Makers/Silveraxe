using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorOrc : MonoBehaviour
{
	public Animator orcAnim;
	public Animator doorAnim;
	public Character doorOrc;
	public DoorSensor sensor;

	NavMeshAgent orcAgent;


	private void Start() {
		orcAgent = doorOrc.GetComponent<NavMeshAgent>();
		orcAgent.updateRotation = false;
	}

	public void DoorOrcMove(float amount) {
		if (doorOrc.isAlive)
			doorOrc.navAgent.SetDestination(doorOrc.transform.position + doorOrc.transform.forward * amount);
	}

	public void OpenDoor() {
		if (doorOrc.isAlive)
			doorAnim.SetTrigger("Open");
	}

	public void CloseDoor() {
		if (doorOrc.isAlive) {
			doorOrc.navAgent.SetDestination(doorOrc.transform.position + doorOrc.transform.forward * -3);
			StartCoroutine(Iin());
		}
	}
	IEnumerator Iin() {
		while (orcAgent.pathPending || orcAgent.remainingDistance > orcAgent.radius) {
			yield return null;
		}
		doorAnim.SetTrigger("Close");
	}

	public void ToggleDetectorThreshold() {
		sensor.ToggleThreshold();
	}
}

