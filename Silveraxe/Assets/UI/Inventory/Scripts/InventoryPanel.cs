using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
	public Image topButton;
	public Sprite invUp;
	public Sprite invDown;

	public bool isOn => transform.position.y > 0;

	/// <summary>
	/// bascule d'affichage
	/// </summary>
	public void Toggle(GameObject combinePanel) {
		if (isOn) {
			Hide(combinePanel);
		} else {
			Show();
		}
	}
	public void Toggle() {
		if (isOn) {
			Hide();
		} else {
			Show();
		}
	}

	public void Hide(GameObject combinePanel) {
		if (isOn) {
			GetComponentInChildren<Animator>().SetTrigger("Down");
			topButton.sprite = invUp;
			foreach (ItemEntryUI entry in GetComponentsInChildren<ItemEntryUI>()) {
				entry.Select(false);
			}
			combinePanel.SetActive(false);
		}
	}
	public void Hide() {
		if (isOn) {
			GetComponentInChildren<Animator>().SetTrigger("Down");
			topButton.sprite = invUp;
			foreach (ItemEntryUI entry in GetComponentsInChildren<ItemEntryUI>()) {
				entry.Select(false);
			}
		}
	}

	public void Show() {
		if (!isOn) {
			GetComponentInChildren<Animator>().SetTrigger("Up");
			topButton.sprite = invDown;
		}
	}

}
