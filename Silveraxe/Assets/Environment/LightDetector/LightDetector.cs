using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using System;

public class LightDetector : MonoBehaviour
{

	public float threshold;
	public bool debug;
	public TMP_Text debugText;

	Camera cam;
	RenderTexture rt;
	RenderTexture tmp;
	RenderTexture prev;

	Texture2D T2D;
	Color[] colors;

	float lightLevel;
	public Action OnTriggerOn;
	public Action OnTriggerOff;

	bool prevState;

	public bool isOn => lightLevel > threshold;


	public bool trigger => lightLevel > threshold;
	public float value => lightLevel;

	bool first = true;

	private void Start() {
		cam = GetComponentInChildren<Camera>();
		rt = new RenderTexture(16, 16, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
		cam.targetTexture = rt;
		T2D = new Texture2D(rt.width, rt.height);

		//if (isOn) {
		//	OnTriggerOn();      // jour
		//} else {
		//	OnTriggerOff();     // nuit
		//}

	}
	private void Update() {
		tmp = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
		Graphics.Blit(rt, tmp);
		prev = RenderTexture.active;
		RenderTexture.active = tmp;
		T2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		T2D.Apply();
		RenderTexture.active = prev;
		RenderTexture.ReleaseTemporary(tmp);

		colors = T2D.GetPixels();
		lightLevel = 0;
		foreach (Color c in colors) {
			//lightLevel += (.2126f * c.r) + (.7152f * c.g) + (.0722f * c.b);
			lightLevel += c.grayscale;
		}
		lightLevel /= colors.Length;

		// attendre l'initialisation de la scène
		if (first) { 
			prevState = isOn; 
			first = false; 
		}
		if (lightLevel == 0) first = true;

		// changement d'état
		if (OnTriggerOn != null && OnTriggerOff != null) {
			if (isOn != prevState) {
				if (isOn) {
					OnTriggerOn();		// jour
				} else {
					OnTriggerOff();		// nuit
				}
				prevState = isOn;
			}
		}


		// infos de débuggage
		if (debug) {
			debugText.text = (lightLevel*100).ToString("0");
		} else {
			debugText.text = "";
		}
	}
}
