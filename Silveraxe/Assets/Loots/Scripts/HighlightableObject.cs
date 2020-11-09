using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// All object that can be highlighted (enemies, interactable object etc.) derive from this class, which takes care
/// of setting the material parameters for it when it gets highlighted.
/// If the object use another material, it will just ignore all the changes.
/// </summary>
public class HighlightableObject : MonoBehaviour
{

	public bool isOn { get; set; } = false;               // flag allumé ?

	// pour les projecteurs
	ProjectorDriver projector;
	// pour les systèmes de particules
	MagicParticles particles;
	// pour les tores
	SelectionRing ring;

	protected virtual void Start() {
		// pour les projecteurs
		projector = GetComponentInChildren<ProjectorDriver>();
		// pour les systèmes de particules
		particles = GetComponentInChildren<MagicParticles>();
		// pour les tores
		ring = GetComponentInChildren<SelectionRing>();

		Highlight(false);
	}

	/// <summary>
	/// true  : allumer le projecteur
	/// false : éteindre le projecteur
	/// </summary>
	public virtual void Highlight(bool on) {
		// pour les projecteurs
		if (projector)
			projector.Highlight(on);
		// pour les systèmes de particules
		if (particles)
			particles.Highlight(on);
		// pour les tores
		if (ring)
			ring.Highlight(on);

		isOn = on;
	}

	public void SetColor(Color color) {
		// pour les projecteurs
		if (projector)
			projector.SetColor(color);
		// pour les systèmes de particules
		if (particles)
			particles.SetColor(color);
		// pour les tores
		if (ring)
			ring.SetColor(color);
	}

}
