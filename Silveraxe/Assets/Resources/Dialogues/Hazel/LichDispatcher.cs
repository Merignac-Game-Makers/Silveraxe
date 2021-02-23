using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichDispatcher : DialogueDispatcher
{
	public Loot rainbowFlower;
	public bool flowerRequested { get; set; } = false;
	public int flowerRequestedNode;
	public int gotFlowerNode;
	public bool flowerQuestDone { get; set; } = false;
	public int flowerQuestDoneNode;

	public override void SetStartNode() {
		base.SetStartNode();
		if (flowerRequested) {
			if (rainbowFlower.entry == null) {
				dialogue.overrideStartNode = flowerRequestedNode;
			} else {
				dialogue.overrideStartNode = gotFlowerNode;
			}
		}
		if (flowerQuestDone) {
			dialogue.overrideStartNode = flowerQuestDoneNode;
		}
	}

}
