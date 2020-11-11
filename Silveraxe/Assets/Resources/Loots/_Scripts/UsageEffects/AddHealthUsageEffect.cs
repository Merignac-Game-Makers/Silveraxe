using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealthUsageEffect : UsageEffect
{
    public int Percentage = 20;
    
    public override bool Use(CharacterData user)
    {
		if (user.Stats.CurrentHealth == user.Stats.stats.health)
			return false;

		//VFXManager.PlayVFX(VFXType.Healing, user.transform.position);

		user.Stats.ChangeHealth(Mathf.FloorToInt(Percentage / 100.0f * user.Stats.stats.health));

		return true;
    }
}