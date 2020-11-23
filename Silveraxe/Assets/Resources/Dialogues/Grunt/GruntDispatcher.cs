using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntDispatcher : DialogueDispatcher
{

	public override bool HasDialogue() {
		return false;
	}

	public override void SetStartNode() {
		base.SetStartNode();
	}
}
