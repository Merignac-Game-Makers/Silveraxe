using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeTemple : MonoBehaviour
{

	public FairyPNJ Fairy;
	public GameObject lightRay;
	public bool isFairyAvailable = true;
	public bool isFairyVisible = false;

	private void OnTriggerEnter(Collider other) {
		if (other == App.playerManager.characterController && isFairyAvailable && !isFairyVisible) {
			ShowFairy(true);
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other == App.playerManager.characterController && isFairyAvailable && isFairyVisible) {
			ShowFairy(false);
		}
	}

	public void ShowFairy(bool on) {
		if (on) {
			Fairy.EnterDialogue();
		} else {
			Fairy.ExitDialogue();
		}
		isFairyVisible = on;
		lightRay.SetActive(isFairyAvailable && !isFairyVisible);
	}

	public void EnableFairy(bool on) {
		isFairyAvailable = on;
		lightRay.SetActive(isFairyAvailable && !isFairyVisible);
	}
}
