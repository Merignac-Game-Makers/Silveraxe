using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsageEffect : ScriptableObject
{
	public string Description;
	public abstract bool Use(CharacterData user);       //return true if could be used, false otherwise.

}