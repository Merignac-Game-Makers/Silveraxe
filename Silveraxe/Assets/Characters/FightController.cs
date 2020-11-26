using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FightController : MonoBehaviour
{
	public static int FightExit = -1;
	public static int FightIdle = 0;
	public static int FightAttack = 1;
	public static int FightHit = 50;
	public static int FightBlock = 60;

	protected NavAnimController animatorController;
	protected StatSystem stats;
	protected Character _this;
	public Character other { get; set; }

	protected float rand;
	protected string Fight => SceneModeManager.Fight;


	protected virtual void Start() {
		_this = GetComponentInParent<Character>();
		animatorController = _this.GetComponentInChildren<NavAnimController>();
		stats = GetComponentInParent<CharacterData>()?.Stats;

	}

	public virtual void Fight_hit() {
		if (other && other.animatorController?.anim?.GetInteger(Fight)!=FightBlock) {
			other.animatorController?.anim?.SetInteger(Fight, FightHit);
		}
	}

	public virtual void Fight_Attack() {
		rand = Random.value;
		if (rand < .6) {													// 60% de chances de toucher
			animatorController.anim.SetInteger(Fight, FightAttack);
		} else {
			animatorController.anim.SetInteger(Fight, FightAttack);
			other.animatorController.anim.SetInteger(Fight, FightBlock);
		}
	}
}
