using UnityEngine;


/// <summary>
/// Handle all the UI code related to the inventory (drag'n'drop of object, using objects, equipping object etc.)
/// </summary>
public abstract class UIBase : MonoBehaviour
{

	public GameObject panel;
	public virtual bool isOn => (panel==null && gameObject.activeInHierarchy) || panel.activeInHierarchy;


	public virtual void Toggle() {
		panel.SetActive(!panel.activeInHierarchy);
	}

	public virtual void Show(bool on) {
		panel.SetActive(on);
	}
}
