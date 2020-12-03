using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealthUsageEffect : UsageEffect
{
    public int Percentage = 20;
    
    public override bool Use(CharacterData user)
    {
		if (user.stats.CurrentHealth == user.stats.stats.health)
			return false;

		//VFXManager.PlayVFX(VFXType.Healing, user.transform.position);

		user.stats.ChangeHealth(Mathf.FloorToInt(Percentage / 100.0f * user.stats.stats.health));

		return true;
    }
}