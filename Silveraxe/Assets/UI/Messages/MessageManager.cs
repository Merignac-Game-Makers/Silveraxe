using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
	public GameObject messageLabel;
	public TMP_Text textHolder;
	public TMP_Text text;
	Coroutine coroutine;

	private void Awake() {
		App.messageManager = this;
	}


	/// <summary>
	/// afficher un message
	/// </summary>
	/// <param name="text">le message</param>
	/// <param name="position">la position d'affichage</param>
	public void ShowLabel(string text, Vector2 position, float delay) {
		if (coroutine != null)
			StopCoroutine(coroutine);
		coroutine = StartCoroutine(IShow(text, position, delay));
	}


	IEnumerator IShow(string text, Vector2 position, float delay) {
		this.textHolder.text = text;
		this.text.text = text;
		messageLabel.transform.position = position;
		messageLabel.SetActive(true);
		yield return new WaitForSeconds(delay);
		messageLabel.SetActive(false);
		//uiManager.isClicOnUI = false;
	}
}
