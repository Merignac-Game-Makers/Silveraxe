using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazelDialogueManager : DialogueManager {
	public Loot rainbowFlower;
	public bool flowerRequested { get; set; } = false;
	public int flowerRequestedNode;
	public int gotFlowerNode;
	public bool flowerQuestDone { get; set; } = false;
	public int flowerQuestDoneNode;

	public override void SetStartNode() {
		base.SetStartNode();
		if (flowerRequested) {										// si la requête "fleur" a été donnée
			if (rainbowFlower.entry == null) {						//	si la fleur n'est pas dans l'inventaire
				dialogue.overrideStartNode = flowerRequestedNode;	//		fleur non trouvée
			} else {												//	si la fleur est dans l'inventaire
				dialogue.overrideStartNode = gotFlowerNode;			//		fleur trouvée
			}
		}
		if (flowerQuestDone) {										// si la quête "fleur" est terminée (fleur donnée)
			dialogue.overrideStartNode = flowerQuestDoneNode;		//		suite
		}
	}
}
