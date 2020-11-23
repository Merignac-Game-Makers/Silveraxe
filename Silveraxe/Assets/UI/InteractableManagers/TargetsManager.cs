using System.Collections.Generic;
using UnityEngine;

using static App;

public class TargetsManager : MonoBehaviour
{

	Target[] targets;

	private void Awake() {
		targetsManager = this;
	}

	void Start() {
		targets = FindObjectsOfType<Target>();
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
