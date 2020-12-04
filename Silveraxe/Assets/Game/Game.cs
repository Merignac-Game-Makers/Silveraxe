using UnityEngine;
using System.Collections;

using static App;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class Game
{

	public static Game current;

	List<object> sav = new List<object>();

	[System.NonSerialized]
	Dictionary<System.Guid, Character> allCharacters;

	[System.NonSerialized]
	public Dictionary<System.Guid, GameObject> allGuidComponents;

	[System.NonSerialized]
	public Dictionary<System.Guid, InteractableObject> allInteractables;


	public Game() {
		allGuidComponents = GetAllGuidComponents();
	}

	public void Save() {
		sav = new List<object>();

		GuidComponent[] items = Object.FindObjectsOfType<GuidComponent>(); // tous les GuidComponents
		foreach (GuidComponent item in items) {

			if (ISave<Loot>(item, sav)) continue;
			if (ISave<Target>(item, sav)) continue;
			if (ISave<Character>(item, sav)) continue;

		}

		SaveLoad.Save();
	}

	public void Load() {
		allGuidComponents = GetAllGuidComponents();

		foreach (SInteractable item in sav) {
			ILoad(item);
		}
	}


	Dictionary<System.Guid, GameObject> GetAllGuidComponents() {
		Dictionary<System.Guid, GameObject> allGuidComponents = new Dictionary<System.Guid, GameObject>();
		GuidComponent[] items = Object.FindObjectsOfType<GuidComponent>(); // tous les GuidComponents
		foreach (GuidComponent item in items) {
			allGuidComponents.Add(item.GetGuid(), item.gameObject);
		}
		return allGuidComponents;
	}

	bool ISave<T>(GuidComponent item, List<object> sav) where T : ISave {
		T iItem = item.GetComponent<T>();
		if (iItem != null && iItem is T) {
			iItem.Serialize(sav);
			return true;
		} else {
			return false;
		}
	}

	void ILoad(SInteractable serialized) {
		var guid = new System.Guid(serialized.guid);
		InteractableObject iItem = current.allGuidComponents[guid].GetComponent<InteractableObject>();     // retrouver l'objet dans la scène (même guid)
		iItem.Deserialize(serialized);
	}
}
