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

	public void Save(bool withPlayer) {
		sav = new List<object>();

		GameObject.Find("World").GetComponent<OutdoorSceneSaver>().Serialize(sav);

		GuidComponent[] items = Object.FindObjectsOfType<GuidComponent>(); // tous les GuidComponents
		foreach (GuidComponent item in items) {
			if (ISave<Loot>(item, sav)) continue;							// si l'objet est un Loot => sauver et passer à l'objet suivant
			if (ISave<Target>(item, sav)) continue;							// si l'objet est un Target => sauver et passer à l'objet suivant
			if (withPlayer)													// si on a choisi de sauver le joueur
				if (ISave<PlayerManager>(item, sav)) continue;              //	- si l'objet est un PlayerManager => sauver et passer à l'objet suivant
			if (ISave<Character>(item, sav)) continue;                      // si l'objet est un Character => sauver et passer à l'objet suivant
		}

		SaveLoad.Save();	// enregistrer le fichier
	}

	public void Load(bool withPlayer) {
		allGuidComponents = GetAllGuidComponents();

		GameObject.Find("World").GetComponent<OutdoorSceneSaver>().Deserialize(sav[0]);
		sav.RemoveAt(0);

		foreach (SInteractable item in sav) {
			ILoad(item, withPlayer);
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

	/// <summary>
	/// Sauvegarder un objet du type T
	/// </summary>
	/// <typeparam name="T">le type d'objet à sauvegarder</typeparam>
	/// <param name="item">l'identifiant unique</param>
	/// <param name="sav">la sauvegarde à incrémenter</param>
	/// <returns></returns>
	bool ISave<T>(GuidComponent item, List<object> sav) where T : ISave {
		T iItem = item.GetComponent<T>();       // rechercher un composant de type T sur ce gameObject
		if (iItem != null && iItem is T) {      // si cet item est bien de type T
			iItem.Serialize(sav);               //		sauvegarder ses infos
			return true;                        //		retourner 'vrai'
		} else {                                // sinon
			return false;                       //		retrouner 'faux'
		}
	}
	bool IsType<T>(GuidComponent item) {
		T iItem = item.GetComponent<T>();       // rechercher un composant de type T sur ce gameObject
		return (iItem != null && iItem is T);
	}

	void ILoad(SInteractable serialized, bool withPlayer) {
		var guid = new System.Guid(serialized.guid);
		InteractableObject iItem = current.allGuidComponents[guid].GetComponent<InteractableObject>();      // retrouver l'objet dans la scène (même guid)

		if (withPlayer) {																					// si on a décidé de restaurer le joueur
			iItem.Deserialize(serialized);																	//		restaurer les valeurs sauvegardées sans vérification
		} else {																							// si on a décidé de NE PAS restaurer le joueur
			PlayerManager player = current.allGuidComponents[guid].GetComponent<PlayerManager>();           //	vérifier si l'objet sérialisé est le joueur
			if (player != null)                                                                             //	si c'est le joueur
				return;                                                                                     //		passer à l'objet suivnant
			else                                                                                            //	si ce n'est pas le joueur
				iItem.Deserialize(serialized);                                                              //		restaurer les valeurs sauvegardées
		}
	}
}
