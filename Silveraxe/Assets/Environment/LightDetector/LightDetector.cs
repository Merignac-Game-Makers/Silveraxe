using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightDetector : MonoBehaviour
{

	public float threshold;
	Camera cam;
	RenderTexture rt;
	RenderTexture tmp;
	RenderTexture prev;

	Texture2D T2D;
	Color[] colors;

	float lightLevel;

	public bool trigger => Trigger();

	private void Start() {
		cam = GetComponentInChildren<Camera>();
		rt = cam.targetTexture;
		T2D = new Texture2D(rt.width, rt.height);
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
		foreach(Color c in colors) {
			lightLevel += (.2126f * c.r) + (.7152f * c.g) + (.0722f * c.b);
		}
		lightLevel /= colors.Length;
	}

	bool Trigger() {
		return lightLevel > threshold;
	}
}
