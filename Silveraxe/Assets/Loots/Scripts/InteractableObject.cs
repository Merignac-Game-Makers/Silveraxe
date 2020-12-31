using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static InteractableObject.Action;
using static App;
using UnityEngine.UI;
using UnityEngine.Animations;
using System.Reflection;

/// <summary>
/// Base class for interactable object, inherit from this class and override InteractWith to handle what happen when
/// the player interact with the object.
/// </summary>
public abstract class InteractableObject : HighlightableObject
{

	public Guid? guid { get; set; }

	public enum Action { take, drop, talk }

	public enum Mode { onClick, onTheFly, onTheFlyOnce }    // modes d'intéraction possibles

	public Mode mode;                                       // le mode d'intéraction de cet objet

	public virtual bool IsInteractable() {                  // l'objet est-il actif pour l'intéraction ?
		return IsHighlightable() && isInPlayerCollider && isClosest;
	}

	public bool isClosest => this == App.itemsManager.closest;

	public Image actionSprite { get; protected set; }
	public bool selectionMuted { get; set; } = false;


	protected override void Start() {
		base.Start();

		guid = GetComponent<GuidComponent>()?.GetGuid();

		// ajouter des MeshColliders si nécessaire
		var meshes = GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter m in meshes) {
			if (m.GetComponent<MeshCollider>() == null)
				m.gameObject.AddComponent<MeshCollider>();

		}

		// lier le sprite
		actionSprite = GetComponentInChildren<Image>();
		if (actionSprite) {
			actionSprite.enabled = false;
			var lookAt = actionSprite.GetComponent<LookAtConstraint>();
			var aim = new ConstraintSource() { sourceTransform = Camera.main.transform, weight = 1 };
			if (lookAt.sourceCount == 0) {
				lookAt.AddSource(aim);
			} else {
				lookAt.SetSource(0, aim);
			}
		}

	}

	public override bool Highlight(bool on) {
		if (actionSprite)
			actionSprite.enabled = on && IsInteractable() && SceneModeManager.sceneMode == SceneMode.normal;
		return base.Highlight(on);
	}

	public bool IsSelected() {
		return actionSprite != null && actionSprite.enabled == true;
	}

	protected void SetActionSprite(Texture2D tex) {
		if (actionSprite) {
			actionSprite.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
		}
	}


	public virtual SInteractable Serialize() {
		return new SInteractable() {
			version = App.saveVersion,
			guid = guid!=null ? ((Guid)guid).ToByteArray() : null,
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
		if (serialized is SInteractable) {
			SInteractable s = serialized as SInteractable;
			transform.position = s.position.ToVector();                     // position
			transform.rotation = s.rotation.ToQuaternion();                 // rotation
		}
	}

}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[Serializable]
public class SInteractable
{
	public string version;      // version de sauvegarde
	public byte[] guid;         // identifiant unique
	public string scene;        // scène dans laquelle se trouve l'objet
	public float[] position;    // position
	public float[] rotation;    // rotation

}
