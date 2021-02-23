using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableAction : TargetAction
{
	public override void Act(Loot loot) {                   // loot est l'objet déposé sur la cible... ici la fleur arc en ciel
		var actionScript = aim.GetComponent<LichActions>();
		if (actionScript) {                                 // si la cible de l'action est bien Hazel
			actionScript.FlowerQuestDone();					// le dialogue est modifié 
		}
		loot.enabled = false;								// la fleur arce en ciel n'est plus intéractible
	}
}
