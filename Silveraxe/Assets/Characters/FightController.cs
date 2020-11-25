using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour
{
	public static string Fight = "Fight";
	public static int FightAttack = 1;
	public static int FightHit = 50;
	public static int FightBlock = 60;

	AnimatorController animatorController;
	StatSystem stats;
	Character _this;
	public Character other { get; set; }

	float rand;

	private void Start() {
		_this = GetComponentInParent<Character>();
		animatorController = _this.GetComponentInChildren<AnimatorController>();
		stats = GetComponentInParent<CharacterData>().Stats;

	}

	public void Fight_hit() {
		if (other) {
			other.animatorController?.anim?.SetInteger(Fight, FightHit);                // 80% de chances de toucher
		}
	}

	public void Fight_Attack() {
		rand = Random.value;
		if (rand < .6) {
			animatorController.anim.SetInteger(Fight, FightAttack);
		} else {
			animatorController.anim.SetInteger(Fight, FightAttack);
			other.animatorController.anim.SetInteger(Fight, FightBlock);
		}
	}

	public Vector3 FightPosition(Character other) {
		var dir = other.transform.position - transform.position;
		var dist = _this.navAgent.radius + other.navAgent.radius;
		Vector3 pos = other.transform.position - dir * (dist / dir.magnitude);
		return pos;
	}
}
