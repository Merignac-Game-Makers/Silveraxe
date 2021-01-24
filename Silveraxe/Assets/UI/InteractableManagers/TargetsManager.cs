using System.Collections.Generic;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{

	List<Target> targets = new List<Target>();

	private void Awake() {
		App.targetsManager = this;
	}

	private void Start() {
		GetSceneTargets();
	}

	public void GetSceneTargets() {
		targets = new List<Target>(FindObjectsOfType<Target>());
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
