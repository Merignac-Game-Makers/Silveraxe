using System.Collections;
using TMPro;
using UnityEngine;

using static App;
public class MessageManager : MonoBehaviour
{

	TMP_Text messageLabel;
	Coroutine coroutine;

	private void Awake() {
		messageManager = this;
	}

	private void Start() {
		messageLabel = GetComponentInChildren<TMP_Text>();
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
		messageLabel.GetComponentInChildren<TMP_Text>().text = text;
		messageLabel.transform.position = position;
		gameObject.SetActive(true);
		yield return new WaitForSeconds(delay);
		gameObject.SetActive(false);
		//uiManager.isClicOnUI = false;
	}
}
