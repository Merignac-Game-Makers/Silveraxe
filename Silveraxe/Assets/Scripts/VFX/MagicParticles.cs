using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicParticles : MonoBehaviour
{
	//public Color inactiveColor = Color.white;       // couleur qd inactif
	//public Color activeColor = Color.white;			// couleur qd actif
	public Texture cookie;							// motif

	ParticleSystem particles;
	Light light;

	void Start() {
		particles = GetComponentInChildren<ParticleSystem>();
		light = GetComponentInChildren<Light>();
	}

	/// <summary>
	/// true  : allumer 
	/// false : éteindre 
	/// </summary>
	public virtual void Highlight(bool on) {
		if (particles) {
			if (on)
				particles.Play();
			else
				particles.Stop();
		}

		if (light)
			light.enabled = on;         // lumière
	}

	public void SetColor(Color color) {
		if (particles) {
			var psMain = particles.main;
			psMain.startColor = color;
		}
	}
}
