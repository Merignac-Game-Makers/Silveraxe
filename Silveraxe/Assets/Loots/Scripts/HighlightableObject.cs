using UnityEngine;

using static App;

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
	public bool useOutline = true;

	public bool isOn { get; set; } = false;               // flag allumé ?
	protected bool isMouseOver = false;
	public bool isInPlayerCollider { get; set; } = false;

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

	public virtual bool IsHighlightable() {
		return isHighlightable;
	}

	public virtual void OnMouseEnter() {
		isMouseOver = true;
		if (IsHighlightable()) {
			Highlight(true);
		}
	}

	public virtual void OnMouseExit() {
		isMouseOver = false;
		if (!isInPlayerCollider)
			Highlight(false);
	}

	/// <summary>
	/// true  : allumer le projecteur
	/// false : éteindre le projecteur
	/// </summary>
	public virtual bool Highlight(bool on) {
		bool found = false;

		if (IsHighlightable() || on==false) {
			// pour les projecteurs
			if (projector)
				found |= projector.Highlight(on, useLight);
			// pour les systèmes de particules
			if (particles)
				found |= particles.Highlight(on, useLight);
			// pour les tores
			if (ring)
				found |= ring.Highlight(on, useLight);
			// pour les highlighters
			if (highlighter)
				found |= highlighter.Highlight(on, useLight);

			if (useOutline && outline) {
				outline.enabled = on;
				found |= true;
			}

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

		outline.OutlineColor = color;
	}

}


#if UNITY_EDITOR
[CustomEditor(typeof(HighlightableObject))]
public class HighlightableEditor : Editor
{
	SerializedProperty pHighlightable;
	SerializedProperty pUseLight;
	SerializedProperty pUseOutline;

	public void Init(SerializedObject target) {
		pHighlightable = target.FindProperty(nameof(HighlightableObject.isHighlightable));
		pUseLight = target.FindProperty(nameof(HighlightableObject.useLight));
		pUseOutline = target.FindProperty(nameof(HighlightableObject.useOutline));
	}

	public void GUI(HighlightableObject item) {
		item.isHighlightable = pHighlightable.boolValue = EditorGUILayout.Toggle("Is Highlightable", pHighlightable.boolValue);
		if (item.isHighlightable) {
			item.useLight = pUseLight.boolValue = EditorGUILayout.Toggle("Use Light", pUseLight.boolValue);
			item.useOutline = pUseOutline.boolValue = EditorGUILayout.Toggle("Use Outline", pUseOutline.boolValue);
		}
	}
}
#endif