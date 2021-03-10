using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pupitre : UIBase {

	public PatrolPoint point;

	void Start() {
		Show(false);
	}

	private void OnTriggerEnter(Collider other) {
		if (other == App.playerManager.characterController) {
			Show(true);
			GoToPupitre();
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other == App.playerManager.characterController)
			Show(false);
	}

	void GoToPupitre() {
		if (App.playerManager.transform.position != point.transform.position) {
			App.playerManager.movementInput.canMove = false;
			App.playerManager.navAgent.SetDestination(point.transform.position, () => {
				App.playerManager.movementInput.canMove = true;
			});
		}
	}
}
