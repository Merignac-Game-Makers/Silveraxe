using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSceneItemsManager : MonoBehaviour
{
	static List<Transform> children;

	private void Awake() {
		children = new List<Transform>();
		for (int i=0; i<transform.childCount; i++) {
			children.Add(transform.GetChild(i));
		}
		//children.Remove(transform);
	}

	public static void SetActiveScene(string scene) {
		foreach (Transform child in children) {
			child.gameObject.SetActive(child.name == scene);
		}
	}
}
