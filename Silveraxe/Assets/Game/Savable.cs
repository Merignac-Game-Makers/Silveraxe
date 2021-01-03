using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Savable : MonoBehaviour, ISave
{
	public Guid? guid => GetComponent<GuidComponent>()?.GetGuid() ?? Guid.Empty;//{ get; set; }

	protected virtual void Awake() {
	}

	protected virtual void Start() {
	}

	public virtual SSavable Serialize() {
		return new SSavable() {
			version = App.saveVersion,
			guid = guid != null ? ((Guid)guid).ToByteArray() : null,
			scene = gameObject.scene.name,
			position = transform.position.ToArray(),                 // position
			rotation = transform.rotation.ToArray(),                 // rotation
		};
	}

	/// <summary>
	/// Restaurer les valeurs précédement sérialisées
	/// </summary>
	/// <param name="serialized">les valeurs sérialisées</param>
	public virtual void Deserialize(object serialized) {
		if (serialized is SSavable) {
			SSavable s = serialized as SSavable;
			transform.position = s.position.ToVector();                     // position
			transform.rotation = s.rotation.ToQuaternion();                 // rotation
		}
	}
}


/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[Serializable]
public class SSavable
{
	public string version;      // version de sauvegarde
	public byte[] guid;         // identifiant unique
	public string scene;        // scène dans laquelle se trouve l'objet
	public float[] position;    // position
	public float[] rotation;    // rotation

}
