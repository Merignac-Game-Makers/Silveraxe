using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRing : MonoBehaviour
{
    Renderer renderer;
    Light light;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        light = GetComponentInChildren<Light>();
    }

	/// <summary>
	/// true  : allumer 
	/// false : éteindre 
	/// </summary>
	public virtual void Highlight(bool on) {
		if (renderer)
			renderer.enabled = on;
		if (light)
			light.gameObject.SetActive(on);
	}

	public void SetColor(Color color) {
		if (renderer)
			renderer.material.SetColor("_EmissionColor", color);
	}
}
