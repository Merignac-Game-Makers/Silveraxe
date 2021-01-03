using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe vraisemblablement devenue inutile
/// </summary>
public static class ResourcesManager 
{
	public static Object[] allItemBases;
	public static Object[] allSprites;

	public static void Init() {
		allItemBases = Resources.LoadAll("", typeof(ItemBase)) ;	// lister les Loot et Equipment
		allSprites = Resources.LoadAll("", typeof(Sprite));			// lister les Sprite
	}

	public static Sprite GetSprite(string name) {
		foreach (Sprite sprite in allSprites) {
			if (sprite.name == name)
				return sprite as Sprite;
		}
		return null;
	}
	public static ItemBase GetItemBase(string name) {
		foreach (ItemBase itemBase in allItemBases) {
			if (itemBase.name == name)
				return itemBase as ItemBase;
		}
		return null;
	}
}
