using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HazelPNJ : PNJ {
	public GameObject compass;
	public Transform hazelPoint;
	public RainbowFlower rainbowFlower;
	public Target table;

	HazelDialogueManager hazelDialogueManager;

	protected override void Start() {
		base.Start();
		hazelDialogueManager = GetComponentInChildren<HazelDialogueManager>();
		rainbowFlower.gameObject.SetActive(false);
	}

	public void FlowerRequested() {
		hazelDialogueManager.flowerRequested = true;
		navAgent.SetDestination(hazelPoint.position);
		compass.SetActive(true);
	}

	public void FlowerQuestDone() {
		hazelDialogueManager.flowerQuestDone = true;
		GiveCastlePass();
	}

	public void DropFlower() {
		table.Highlight(false);
		table.item = rainbowFlower;
		App.inventoryUI.selectedEntry?.Select(false);
		rainbowFlower.Drop(table);
	}

	public void GiveCastlePass() {

	}
}
