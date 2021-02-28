using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using static InteractableObject.Action;
using static App;
using UnityEngine.UI;
using System.Reflection;

/// <summary>
/// Describes an InteractableObject that can be read up and grants a specific item when interacted with.
/// </summary>
public class Readable : InteractableObject
{
	public string text;
	public bool read { get; set; } = false;												// lu ?

	public override bool IsInteractable() {                                         // l'objet est intéractif si
		return base.IsInteractable();                                               //  
	}

	protected override void Start() {
		base.Start();
	}

	void Update() {

		// bouton d'action
		if (Input.GetButtonDown("Fire1") && !uiManager.isClicOnUI) {
			Act();
		}
	}

	void Act() {
		if (IsInteractable())
			Read();
	}


	protected virtual void Read() {
		// on lit l'objet
		App.readableUI.ShowMessage(text);
		read = true;
	}



	#region sauvegarde
	/// <summary>
	/// Ajouter la sérialisation des infos à sauvegarder pour cet objet à la sauvegarde générale 'sav'
	/// </summary>
	public override SSavable Serialize() {
		var result = new SReadable().Copy(base.Serialize());
		result.read = read;
		return result;
	}

	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {
		base.Deserialize(serialized);
		SReadable sReadable = (SReadable)serialized;
		read = sReadable.read;
	}

	#endregion
}

/// <summary>
/// Classe pour la sauvegarde
/// 
/// Pour Loot : l'id de la 'target' sur laquelle il est posé
/// </summary>
[System.Serializable]
public class SReadable : SSavable
{
	public bool read;
}