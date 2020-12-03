using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
	public Color color = new Color(.2f, .2f, .1f);
    MeshRenderer m_renderer;
	new Light light;

    void Start()
    {
        m_renderer = transform.parent.GetComponentInChildren<MeshRenderer>();
		//if (m_renderer) {
		//	m_renderer.material = new Material(m_renderer.material);  // duplication du material pour ne pas changer tous les objets simultanément
		//}

		light = GetComponentInChildren<Light>();

		SetColor(color);
		Highlight(false, true);
	}

	/// <summary>
	/// true  : allumer 
	/// false : éteindre 
	/// </summary>
	public virtual bool Highlight(bool on, bool useLight) {

		if (m_renderer) {
			if (on)
				m_renderer.material.EnableKeyword("_EMISSION");
			else 
				m_renderer.material.DisableKeyword("_EMISSION");
		}

		if (light && useLight) 
			light.gameObject.SetActive(on);
		
		return m_renderer || (light && useLight);
	}

	public void SetColor(Color color) {
		if (m_renderer) {
			m_renderer.material.SetColor("_EmissionColor", color);
		}
	}

}
