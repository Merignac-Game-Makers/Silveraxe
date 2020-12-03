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
	public bool critical { get; set; } = false;
	public bool isInFightMode => animatorController?.anim?.GetBool(SceneModeManager.Fight) ?? false;

	int dice;

	// INITIALISATION
	protected virtual void Start() {
		_this = GetComponentInParent<Character>();
		animatorController = _this.GetComponentInChildren<NavAnimController>();
		stats = gameObject.GetComponentInParent<CharacterData>()?.stats;

	}

	public void SetOther(Character o) {
		other = o;
		otherStats = other?.GetComponent<CharacterData>()?.stats;
	}


	// EVENEMENTS DE COMBAT
	/// Attaquer
	public virtual void Fight_Attack() {
		if (other.fightController.isAlive) {
			blocked = !CalculAttack();						// Le coup touche-t-il ? est-il critique ?
			animatorController.anim.SetTrigger(Attack);     // animation "attaque"
			PlaySound(attack);                              // son "attaque"
		}
	}

	/// Notre attaque est parée
	public virtual void Fight_Blocked() {
		if (blocked) other.fightController.Fight_Block();
	}
	/// Parer une attaque
	public virtual void Fight_Block() {
		//animatorController.SendAnims((Animator anim) => { anim.SetTrigger(Block); });
		animatorController.anim.SetTrigger(Block);
		PlaySound(block);
	}

	/// Notre attaque touche l'ennemi
	public virtual void Fight_hit() {
		if (enabled && !blocked)
			other.fightController.Fight_GetHit();
	}
	/// Encaisser un coup
	public virtual void Fight_GetHit() {
		stats.ChangeHealth(-CalculDamage());				//	on perd de la vie
		if (stats.CurrentHealth <= 0) {                     //		si plus de vie
			Die();                                          //			on est mort		
		} else {                                            //		sinon
			//animatorController.SendAnims((Animator anim) => { anim.SetTrigger(Hit); });
			animatorController?.anim?.SetTrigger(Hit);		//			animation "prendre un coup"
			PlaySound(hit);                                 //			son "prendre un coup"
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
		//animatorController.SendAnims((Animator anim) => { anim.SetTrigger(Dead); });
		animatorController?.anim?.SetTrigger(Dead);                 // animation "mort"
		PlaySound(die);                                             // son "mort"
	}

	public void OtherWin() {
		//other.animatorController.SendAnims((Animator anim) => { anim.SetTrigger(Victory); });
		other.animatorController?.anim?.SetTrigger(Victory);
		other.fightController.PlaySound(other.fightController.victory);
	}


	public bool CalculAttack() {
		int agility = stats.baseStats.agility;                      // notre agilité
		int strength = stats.baseStats.strength;                    // notre force
		int otherDefense = otherStats.baseStats.defense;            // la défense de l'aversaire
		int otherAgility = otherStats.baseStats.agility;            // l'agilité de l'aversaire

		int dice = Random.Range(0, 20);								// 1 dé 20
		int bonus = agility + strength / 4;							// bonus lié à l'agilité et à la force

		other.fightController.critical =  (dice + bonus) > (otherDefense + otherAgility) / 2;	// le coup est-il critique ? => on transmet l'information à l'autre combattant pour le calcul des dégâts

		return (dice + bonus) > (otherDefense + otherAgility) / 4;								// le coup porte-t-il ?
	}

	public int CalculDamage() {
		int weapon = 3;										// la puissance de l'arme
		int strength = otherStats.baseStats.strength;		// la force de celui qui nous attaque
		int defense = stats.baseStats.defense;				// notre défense

		dice = Random.Range(0, weapon);
		int damages = (strength / 2) + dice;
		int absorption = defense/2;
		int k = critical ? 2 : 1;

		int result = k * Mathf.Max(0, damages - absorption);
		return result;
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
