using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour
{
	public static string Fight = "Fight";
	public static int FightHit = 50;

	public Character other { get; set; }

	public void Fight_hit() {
		if (other) {
			other.animatorController?.anim?.SetInteger(Fight, FightHit);
		}
	}
}
