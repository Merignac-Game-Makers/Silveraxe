using System.Collections.Generic;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{

	List<Target> targets = new List<Target>();

	private void Awake() {
		App.targetsManager = this;
	}

	public void GetSceneTargets() {
		targets = new List<Target>(FindObjectsOfType<Target>());
		for (int i = targets.Count - 1; i > -1; i--) {
			if (targets[i].gameObject.scene.name != App.currentSceneName && targets[i].gameObject.scene.name != App.dontDestroyScene) // 
				targets.Remove(targets[i]);
		}
	}

	public void OnItemSelect() {
		foreach (Target target in targets) {
			if (target.isInPlayerCollider && !target.isOn && target.IsInteractable()) {
				target.Highlight(true);
			}
		}
	}

	public void OnTake() {
		foreach (Target target in targets) {
			if (target.isInPlayerCollider) {
				target.Highlight(false);
			}
		}
	}
}
