using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LichActions : MonoBehaviour
{
	public GameObject compass;
	public Transform hazelPoint;
	public Loot rainbowFlower;
	public Target table;

	NavMeshAgent navAgent;
	LichDispatcher dispatcher;

	private void Start() {
		navAgent = GetComponent<NavMeshAgent>();
		dispatcher = GetComponentInChildren<LichDispatcher>();
		rainbowFlower.gameObject.SetActive(false);
	}

	public void ShowCompass() {
		compass.SetActive(true);
	}

	public void GotoTable() {
		navAgent.SetDestination(hazelPoint.position);
	}

	public void FlowerRequested() {
		dispatcher.flowerRequested = true;
	}
	public void FlowerQuestDone() {
		dispatcher.flowerQuestDone = true;
		GiveCastlePass();
	}

	public void DropFlower() {
		table.Highlight(false);
		table.item = rainbowFlower;
		App.inventoryUI.selectedEntry?.Select(false);
		rainbowFlower.Drop(table);
		if (table.targetAction) {
			table.targetAction.Act(rainbowFlower);
		}
	}

	public void GiveCastlePass() {

	}
}
