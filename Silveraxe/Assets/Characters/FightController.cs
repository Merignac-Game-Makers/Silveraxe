using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FightController : MonoBehaviour
{
	[Header("Audio")]
	public AudioClip[] attack;
	public AudioClip[] block;
	public AudioClip[] hit;
	public AudioClip[] die;
	public AudioClip[] victory;



	const string Attack = "Attack";
	const string Block = "Block";
	const string Hit = "Hit";
	const string Dead = "Die";
	const string Victory = "Victory";

	protected NavAnimController animatorController;
	protected StatSystem stats;
	protected StatSystem otherStats;
	protected Character _this;
	public Character other { get; private set; }

	protected string Fight => SceneModeManager.Fight;
	public bool canFight => stats.CurrentHealth > 0;

	bool blocked = false;
	public bool isInFightMode => animatorController?.anim?.GetBool(SceneModeManager.Fight) ?? false;

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
		if (!blocked) {
			otherStats.ChangeHealth(-1);
			if (otherStats.CurrentHealth <= 0) {
				Die();
			} else {
				other.animatorController?.anim?.SetTrigger(Hit);
			}
		}
	}

	public virtual void Fight_Attack() {
		if (other.fightController.canFight) {
			blocked = Random.value > .6;					// 60% de chances de toucher
			animatorController.anim.SetTrigger(Attack);
		}
	}

	public virtual void Fight_Block() {
		if (blocked)
			other.animatorController.anim.SetTrigger(Block);
	}

	public void Fight_End() {
		SceneModeManager.SetSceneMode(SceneMode.fight, false, other);
	}


	public void CheckLife() {
		if (stats.CurrentHealth <= 0) {
			Die();
		}
	}

	void Die() {
		other.animatorController?.anim?.SetTrigger(Dead);
	}

	public void OtherWin() {
		other.animatorController?.anim?.SetTrigger(Victory);
	}
}
