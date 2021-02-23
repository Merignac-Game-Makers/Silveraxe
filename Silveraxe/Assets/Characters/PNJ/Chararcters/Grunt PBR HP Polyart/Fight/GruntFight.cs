using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntFight : FightController
{
	public DoorOrc door;

	[Header("Other")]
	public float attackRate;
	public LightDetector lightDetector;

	float timer;

	public GameObject polyartSkin;			// apparence basic = plyart
	public GameObject rockSkin;				// apparence medium = hand painted +> rock

	public AudioClip[] petrified;

	private void OnEnable() {
		if (lightDetector)
			lightDetector.OnTriggerOn += SunFlashed;
	}
	private void OnDisable() {
		if (lightDetector)
			lightDetector.OnTriggerOn -= SunFlashed;
	}

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

		if (isAlive && lightDetector && lightDetector.isOn) {
			stats.ChangeHealth(-stats.CurrentHealth);
			CheckLife();
		}
	}

	public override void Die() {
		if (lightDetector && lightDetector.isOn) {
			if (!door.isOpen)
				return;
			animatorController?.anim?.SetTrigger(DIE);                 // animation "mort"
			PlaySound(petrified);                                      // son "pétrifié"
			polyartSkin.SetActive(false);
			rockSkin.SetActive(true);
			_this.navAgent.isStopped = true;
		} else {
			base.Die();
		}
	}

	void SunFlashed() {
		stats.ChangeHealth(-stats.CurrentHealth);
		Die();
	}
}
