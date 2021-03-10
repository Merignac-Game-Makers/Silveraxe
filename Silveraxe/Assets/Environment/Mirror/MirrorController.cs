using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour {

	public Pupitre pupitre;
	public GameObject socle;
	public GameObject miroir;
	public float hStep = 10;
	public float vStep = 10;

	public RectTransform timeHolder;
	RectTransform rectTransform;

	float t = 0;
	Quaternion start;
	Quaternion end;

	private void Start() {
		rectTransform = GetComponent<RectTransform>();
		rectTransform.position = new Vector3(timeHolder.position.x, timeHolder.position.y - timeHolder.rect.height * 1.1f, 0);
		rectTransform.sizeDelta = new Vector2(timeHolder.rect.width * 2, timeHolder.rect.width * 2 * 9 / 16);
	}

	public void Left() {
		StartCoroutine(Ihorizontal(-hStep));
	}
	public void Right() {
		StartCoroutine(Ihorizontal(hStep));
	}
	IEnumerator Ihorizontal(float value) {
		EnterCode();
		t = 0;
		start = socle.transform.localRotation;
		end = Quaternion.Euler(start.eulerAngles + new Vector3(0, 0, value));
		while (t < 1) {
			t += Time.deltaTime;
			socle.transform.localRotation = Quaternion.Lerp(start, end, t);
			yield return null;
		}
		socle.transform.localRotation = end;
	}

	public void Down() {
		StartCoroutine(Ivertical(-vStep));
	}
	public void Up() {
		StartCoroutine(Ivertical(vStep));
	}
	IEnumerator Ivertical(float value) {
		EnterCode();
		t = 0;
		start = miroir.transform.localRotation;
		end = Quaternion.Euler(start.eulerAngles + new Vector3(value, 0, 0));
		while (t < 1) {
			t += Time.deltaTime;
			miroir.transform.localRotation = Quaternion.Lerp(start, end, t);
			yield return null;
		}
	}

	void EnterCode() {
		if (App.playerManager.transform.position != pupitre.point.transform.position) {
			App.playerManager.movementInput.canMove = false;
			App.playerManager.navAgent.SetDestination(pupitre.point.transform.position, () => {
				App.playerManager.movementInput.canMove = true;
				App.playerManager.animatorController.anim.SetTrigger("Code");
			});
		} else {
			App.playerManager.animatorController.anim.SetTrigger("Code");
		}
	}

}
