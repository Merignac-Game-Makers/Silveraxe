using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// All object that can be highlighted (enemies, interactable object etc.) derive from this class, which takes care
/// of setting the material parameters for it when it gets highlighted.
/// If the object use another material, it will just ignore all the changes.
/// </summary>
public class HighlightableObject : MonoBehaviour
{

	public bool isHighlightable = true;
	public bool useLight = true;

	public bool isOn { get; set; } = false;               // flag allumé ?

	// pour les projecteurs
	ProjectorDriver projector;
	// pour les systèmes de particules
	MagicParticles particles;
	// pour les tores
	SelectionRing ring;
	// pour les highlighters
	Highlighter highlighter;

	Outline outline;

	protected virtual void Start() {
		// pour les projecteurs
		projector = GetComponentInChildren<ProjectorDriver>();
		// pour les systèmes de particules
		particles = GetComponentInChildren<MagicParticles>();
		// pour les tores
		ring = GetComponentInChildren<SelectionRing>();
		// pour les highlighters
		highlighter = GetComponentInChildren<Highlighter>();

		Highlight(false);

		outline = gameObject.AddComponent<Outline>();

		outline.OutlineMode = Outline.Mode.OutlineVisible;
		outline.OutlineColor = Color.green;
		outline.OutlineWidth = 5f;
		outline.enabled = false;
	}

	/// <summary>
	/// true  : allumer le projecteur
	/// false : éteindre le projecteur
	/// </summary>
	public virtual bool Highlight(bool on) {
		bool found = false;

		if (isHighlightable) {
			// pour les projecteurs
			if (projector)
				found = projector.Highlight(on, useLight);
			// pour les systèmes de particules
			if (particles)
				found = particles.Highlight(on, useLight);
			// pour les tores
			if (ring)
				found = ring.Highlight(on, useLight);
			// pour les highlighters
			if (highlighter)
				found = highlighter.Highlight(on, useLight);

			isOn = on;
		}

		return found;
	}

	public virtual void ToggleOutline(bool on) {
		outline.enabled = on;
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
		// pour les highlighters
		if (highlighter)
			highlighter.SetColor(color);

	}

}


#if UNITY_EDITOR
[CustomEditor(typeof(HighlightableObject))]
public class HighlightableEditor : Editor
{
	SerializedProperty pHighlightable;
	SerializedProperty pUseLight;

	public void Init(SerializedObject target) {
		pHighlightable = target.FindProperty(nameof(HighlightableObject.isHighlightable));
		pUseLight = target.FindProperty(nameof(HighlightableObject.useLight));
	}

	public void GUI(HighlightableObject item) {
		item.isHighlightable = pHighlightable.boolValue = EditorGUILayout.Toggle("Is Highlightable", pHighlightable.boolValue);
		if (item.isHighlightable) {
			item.useLight = pUseLight.boolValue = EditorGUILayout.Toggle("Use Light", pUseLight.boolValue);
		}
	}
}
#endif