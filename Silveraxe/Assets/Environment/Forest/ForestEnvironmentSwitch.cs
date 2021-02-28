using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestEnvironmentSwitch : EnvironmentSwitch {

	public Material lanternMaterial1;
	public Material lanternMaterial2;
	public Light sun;

	List<Light> lights;
	List<Behaviour> halos = new List<Behaviour>();
	List<LensFlare> lensFlares;

	bool playerIsHere { get; set; } = false;
	bool night => sun.transform.position.y < 0;
	bool status;

	protected override void Start() {
		heavy.gameObject.SetActive(true);
		lights = new List<Light>(heavy.GetComponentsInChildren<Light>(true));
		lights.ForEach(l => halos.Add((Behaviour)(l.GetComponent("Halo"))));
		lensFlares = new List<LensFlare>(heavy.GetComponentsInChildren<LensFlare>(true));
		ToggleLights(false);
	}

	private void OnTriggerStay(Collider other) {
		if (other.gameObject == App.playerManager.gameObject) {
			if (night != status) {
				ToggleLights(night);
			}
		}
	}

	public void ToggleLights(bool on) {
		foreach(Light light in lights) {
			light.enabled = on;
		}
		foreach(Behaviour halo in halos) {
			halo.enabled = on;
		}
		foreach(LensFlare lensFlare in lensFlares) {
			lensFlare.enabled = on;
		}
		if (on) {
			lanternMaterial1.EnableKeyword("_EMISSION");
			lanternMaterial2.EnableKeyword("_EMISSION");
		} else {
			lanternMaterial1.DisableKeyword("_EMISSION");
			lanternMaterial2.DisableKeyword("_EMISSION");
		}
		status = on;
	}
}
