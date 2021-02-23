using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyIntro : MonoBehaviour {
	public float alt { get; set; }
	public Vector3 position;
	public Vector3 rotation;

	public Animator TyAnimator;
	public GameObject pivot;

	bool isOnFloor = false;
	Vector2 labelPosition = new Vector2(Screen.width / 2f, Screen.height - 50);

	void Start() {
		TyAnimator.SetTrigger("StartIntro");
	}

	public void StartFalling() {
		App.playerManager.GetComponent<Animator>().SetTrigger("Fall");
	}


	public void EndFalling() {
		TyAnimator.SetTrigger("StandUp");
	}

	public void EndOfIntro() {
		this.enabled = false;                                           // désactiver ce script
		App.playerManager.GetComponent<Animator>().enabled = false;     // désactiver l'animateur de l'intro
		App.playerManager.navAgent.enabled = false;                     // réinitialiser le navAgent sur le navMesh
		App.playerManager.navAgent.enabled = true;
		App.instructionsUI.Toggle(true);

	}

	private void OnTriggerEnter(Collider other) {
		if (!isOnFloor && other.tag == "Floor") {
			TyAnimator.SetTrigger("StandUp");
			isOnFloor = true;
		}
	}

	public void ShowMessage(string text) {
		App.messageManager.ShowLabel(text, labelPosition, 2.5f);
	}
}
