using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntFight : FightController
{

	public float attackRate;

	float timer;

	protected override void Start() {
		base.Start();
		timer = attackRate;
	}

	private void FixedUpdate() {
		if (SceneModeManager.sceneMode == SceneMode.fight) {
			timer -= Time.fixedDeltaTime;
			if (timer < 0) {
				timer = attackRate;
				Fight_Attack();
			}
		}
	}

}
