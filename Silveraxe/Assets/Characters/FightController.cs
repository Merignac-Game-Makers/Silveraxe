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
	public static int FightDead = 90;

	protected NavAnimController animatorController;
	protected StatSystem stats;
	protected StatSystem otherStats;
	protected Character _this;
	public Character other { get; private set; }

	protected float rand;
	protected string Fight => SceneModeManager.Fight;
	public bool canFight => stats.CurrentHealth > 0;


	protected virtual void Start() {
		_this = GetComponentInParent<Character>();
		animatorController = _this.GetComponentInChildren<NavAnimController>();
		stats = GetComponentInParent<CharacterData>()?.Stats;

	}

	public void SetOther(Character o) {
		other = o;
		otherStats = other?.GetComponent<CharacterData>()?.Stats;
	}

	public virtual void Fight_hit() {
		if (other && other.animatorController?.anim?.GetInteger(Fight)!=FightBlock) {
			otherStats.ChangeHealth(-1);
			if (otherStats.CurrentHealth <= 0) {
				other.animatorController?.anim?.SetInteger(Fight, FightDead);
			} else {
				other.animatorController?.anim?.SetInteger(Fight, FightHit);
			}
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

	public void Fight_End() {
		SceneModeManager.SetSceneMode(SceneMode.fight, false);
	}

	public void RestetFight() {
		animatorController.anim.SetInteger(Fight, FightIdle);
	}

	public void CheckLife() {
		if (stats.CurrentHealth <= 0) {
			animatorController?.anim?.SetInteger(Fight, FightDead);
		}
	}
}
