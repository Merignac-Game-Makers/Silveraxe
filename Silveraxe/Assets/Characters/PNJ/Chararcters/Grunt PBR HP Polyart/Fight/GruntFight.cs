using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntFight : FightController
{
	[Header("Other")]
	public float attackRate;
	public LightDetector lightDetector;

	float timer;

	public GameObject polyartSkin;      // apparence basic = plyart
	public GameObject rockSkin;           // apparence medium = hand painted +> rock

	public AudioClip[] petrified;

	protected override void Start() {
		base.Start();
		timer = attackRate;
	}

	private void FixedUpdate() {
		if (SceneModeManager.sceneMode == SceneMode.fight && isAlive && App.playerManager.fightController.other == _this) {
			timer -= Time.fixedDeltaTime;
			if (timer < 0) {
				timer = attackRate / 2f * (1 + Random.value);       // la prochaine attaque dans un délai entre attackRate et attackRate/2
				Fight_Attack();
			}
		}

		if (isAlive && lightDetector && lightDetector.trigger) {
			stats.ChangeHealth(-stats.CurrentHealth);
			CheckLife();
		}
	}

	public override void Die() {
		if (lightDetector.trigger) {
			animatorController?.anim?.SetTrigger(DIE);                 // animation "mort"
			PlaySound(petrified);                                      // son "pétrifié"
			polyartSkin.SetActive(false);
			rockSkin.SetActive(true);
		} else {
			base.Die();
		}

	}
}
