using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

using static App;

public abstract class Character : InteractableObject
{

	// CharacterData
	public CharacterData characterData { get; private set; }                          // caractéristiques du personnage (santé, force..., inventaire, ...)

	// navigation
	public NavMeshAgent navAgent { get; private set; }                                  // agent de navigation
	protected bool MoveAcrossNavMeshesStarted = false;              // flag : est-on sur un nav mesh link ? (pour gérer la vitesse)

	// animation
	public NavAnimController animatorController { get; private set; }
	LookAtConstraint lookAt;

	// pour les intéractions (combats / dialogues)
	public float interactionDistance = 1.5f;

	// pour les combats
	public bool isInFightMode => fightController?.isInFightMode ?? false;
	public FightController fightController { get; private set; }
	public bool isAlive => fightController ?  fightController.isAlive: true;

	public Patrol patrol { get; private set; }


	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		base.Start();

		// pour les déplacements
		navAgent = GetComponentInChildren<NavMeshAgent>();                     // préparation de la navigation

		// pour les animations
		animatorController = GetComponentInChildren<NavAnimController>();
		lookAt = GetComponent<LookAtConstraint>();
		if (!lookAt)
			lookAt = gameObject.AddComponent<LookAtConstraint>();
		if (lookAt.sourceCount == 0) {
			lookAt.AddSource(new ConstraintSource());
		}

		// pour les combats
		fightController = GetComponentInChildren<FightController>();
		characterData = GetComponent<CharacterData>();              // caractéristiques du joueur
		characterData.Init();                                       // ... initialisation

		// pour les patrouilleurs
		patrol = GetComponentInChildren<Patrol>();

	}

	public abstract void Act();

	public void FaceTo(bool on, GameObject other = null) {
		lookAt.constraintActive = on;
		if (on && other != null) {
			lookAt.SetSource(0, new ConstraintSource() { sourceTransform = other.transform, weight = 1 });
		}
	}

	// à quelle distance doit se placer un autre personnage pour intéragir avec moi ?
	public Vector3 ActPosition(Character other, SceneMode mode) {
		var dir = other.transform.position - transform.position;
		var dist = (interactionDistance + other.interactionDistance);
		float k = 1f;
		switch (mode) {
			case SceneMode.normal:
				k = 2f;
				break;
			case SceneMode.dialogue:
				k = 1.5f;
				break;
			case SceneMode.fight:
				k = 1f;
				break;
		}
		Vector3 pos = transform.position + k * dir.normalized * dist;
		return pos;
	}

	// coordonnées des 8 sommets de la 'boundingBox' 
	public Vector3[] GetBounds() {
		CharacterController cc = GetComponent<CharacterController>();
		Vector3[] corners = new Vector3[8];
		Vector3 min = cc.bounds.min;
		Vector3 max = cc.bounds.max;
		corners[0] = new Vector3(min.x, min.y, min.z);
		corners[1] = new Vector3(min.x, min.y, max.z);
		corners[2] = new Vector3(max.x, min.y, min.z);
		corners[3] = new Vector3(max.x, min.y, max.z);
		corners[4] = new Vector3(min.x, max.y, min.z);
		corners[5] = new Vector3(min.x, max.y, max.z);
		corners[6] = new Vector3(max.x, max.y, min.z);
		corners[7] = new Vector3(max.x, max.y, max.z);
		return corners;
	}



	#region sauvegarde
	/// <summary>
	/// Sérialiser les infos à sauvegarder pour cet objet
	/// </summary>
	public override SSavable Serialize() {
		var result = new SCharacter().Copy(base.Serialize());
		result.stats0 = characterData.stats.baseStats; ;									// statistiques de base
		result.stats1 = characterData.stats.stats;                                          // statistiques de base
		result.currentHealth = characterData.stats.CurrentHealth;                           // points de vie courants						
		result.inventory = new InventoryData(characterData.inventory);						// inventaire
		return result;
	}

	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized) {

		if (navAgent!=null && navAgent.enabled) {
			navAgent.ResetPath();                    // annulation de la navigation en cours
			navAgent.velocity = Vector3.zero;        // vitesse nulle
		}

		base.Deserialize(serialized);
		if (serialized is SCharacter) {
			SCharacter s = serialized as SCharacter;
			characterData.stats.baseStats.Copy(s.stats0);                       // statistiques de base
			characterData.stats.stats.Copy(s.stats1);                           // statistiques courantes
			characterData.stats.LoadCurrentHealth(s.currentHealth);				// points de vie courants
			s.inventory.CopyTo(characterData.inventory);                        // inventaire

			if (!isAlive)
				animatorController.anim.SetBool("IsDead", true);
		}

	}
	#endregion
}


/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class SCharacter : SSavable
{
	public StatSystem.Stats stats0;             // statistiques de base
	public StatSystem.Stats stats1;             // statistiques courantes
	public int currentHealth;					// points de vie courants
	public InventoryData inventory;				// inventaire
	public float[] navAgentDestination;			// destination
}