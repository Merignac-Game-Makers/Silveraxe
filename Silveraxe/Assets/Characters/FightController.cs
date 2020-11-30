using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FightController : MonoBehaviour
{
	[Header("Audio")]
	public SFXManager.Use UseType;

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
	public bool isAlive => stats.CurrentHealth > 0;

	bool blocked = false;
	public bool isInFightMode => animatorController?.anim?.GetBool(SceneModeManager.Fight) ?? false;


	// INITIALISATION
	protected virtual void Start() {
		_this = GetComponentInParent<Character>();
		animatorController = _this.GetComponentInChildren<NavAnimController>();
		stats = GetComponentInParent<CharacterData>()?.Stats;

	}

	public void SetOther(Character o) {
		other = o;
		otherStats = other?.GetComponent<CharacterData>()?.Stats;
	}


	// EVENEMENTS DE COMBAT
	/// Attaquer
	public virtual void Fight_Attack() {
		if (other.fightController.isAlive) {
			blocked = Random.value > .6;                    // 60% de chances de toucher
			animatorController.anim.SetTrigger(Attack);     // animation "attaqu"
			PlaySound(attack);                              // son "attaque"
		}
	}

	/// Notre attaque est parée
	public virtual void Fight_Blocked() {
		if (blocked) other.fightController.Fight_Block();
	}
	/// Parer une attaque
	public virtual void Fight_Block() {
		animatorController.anim.SetTrigger(Block);
		PlaySound(block);
	}

	/// Notre attaque touche l'ennemi
	public virtual void Fight_hit() {
		if (!blocked) other.fightController.Fight_GetHit();
	}
	/// Encaisser un coup
	public virtual void Fight_GetHit() {
			stats.ChangeHealth(-1);								//	on perd de la vie
			if (stats.CurrentHealth <= 0) {						//		si plus de vie
				Die();											//			on est mort		
			} else {											//		sinon
				animatorController?.anim?.SetTrigger(Hit);		//			animation "prendre un coup"
				PlaySound(hit);									//			son "prendre un coup"
			}
	}

	public void Fight_End() {                                       // sortir du mode combat 
		SceneModeManager.SetSceneMode(SceneMode.fight, false, other);
	}


	public void CheckLife() {
		if (stats.CurrentHealth <= 0) {                             // si la vie restante est 0
			Die();                                                  // on est mort !
		}
	}

	void Die() {                                                    // en cas de décès
		animatorController?.anim?.SetTrigger(Dead);                 // animation "mort"
		PlaySound(die);                                             // son "mort"
	}

	public void OtherWin() {
		other.animatorController?.anim?.SetTrigger(Victory);
		other.fightController.PlaySound(other.fightController.victory);
	}



	// AUDIO

	public void PlaySound(string sound) {
		switch (sound) {
			case Attack:
				PlaySound(attack);
				break;
			case Block:
				if (blocked)
					other.fightController.PlaySound(block);
				break;
			case Hit:
				PlaySound(hit);
				break;
			case Dead:
				PlaySound(die);
				break;
			case Victory:
				PlaySound(victory);
				break;
		}
	}

	public void PlaySound(AudioClip[] clips) {
		if (clips.Length == 0)
			return;

		SFXManager.PlaySound(UseType, new SFXManager.PlayData() {
			Clip = clips[Random.Range(0, clips.Length)],
			Position = transform.position
		});
	}

}
