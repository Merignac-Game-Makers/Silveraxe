using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntFight : FightController
{
	[Header("Other")]
	public float attackRate;

	float timer;

	protected override void Start() {
		base.Start();
		timer = attackRate;
	}

	private void FixedUpdate() {
		if (SceneModeManager.sceneMode == SceneMode.fight && isAlive) {
			timer -= Time.fixedDeltaTime;
			if (timer < 0) {
				timer = attackRate / 2f * (1 +  Random.value);		// la prochaine attaque dans un délai entre attackRate et attackRate/2
				Fight_Attack();
			}
		}
	}

}
