using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Handle all the UI code related to the inventory (drag'n'drop of object, using objects, equipping object etc.)
/// </summary>
public abstract class UIBase : App
{

	public GameObject panel;
	public virtual bool isOn => (panel==null && gameObject.activeInHierarchy) || panel.activeInHierarchy;


	public abstract void Toggle();

	public void Show(bool on) {
		panel.SetActive(on);
	}
}
