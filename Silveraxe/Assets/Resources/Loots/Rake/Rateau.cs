using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rateau : MonoBehaviour {

	Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
	}

	void Update() {

	}

	private void OnTriggerEnter(Collider other) {
		if (other == App.playerManager.characterController) {
			App.playerManager.StopAgent(true);
			anim.SetTrigger("slam");
		}
	}

	public void Release() {
		StartCoroutine(IRestart());
		IEnumerator IRestart() {
			yield return new WaitForSeconds(1f);
			anim.SetTrigger("release");
		}
	}

	public void RestartPlayer() {
		App.playerManager.StopAgent(false);
	}

	public void GetFaceHit() {
		App.playerManager.animatorController.anim.SetTrigger("FaceImpact");
	}
}
