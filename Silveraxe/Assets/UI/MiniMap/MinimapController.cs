using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
	public Camera minimapCamera;

	public GameObject smallView;
	public int smallMapSize = 10;
	public GameObject smallArrow;

	public GameObject largeView;
	public GameObject largeArrow;

	public int largeMapSize = 200;
	public int sqr = 512;

	RenderTexture smallMap;
	RenderTexture largeMap;

	int mode = 0;               // 0 = pas de carte; 1 * mini carte//; 2 = grande carte
	float k = 1;
	float contentSize;

	Transform player;
	// Start is called before the first frame update
	void Start() {
		player = PlayerManager.Instance.transform;

		smallMap = new RenderTexture(256, 256, 24);
		smallMap.name = "smallMap";
		smallView.GetComponentInChildren<RawImage>().texture = smallMap;

		largeView.GetComponentInChildren<RawImage>().texture = LoadTextureFromFile("map");

		minimapCamera.targetTexture = smallMap;
		minimapCamera.orthographicSize = smallMapSize;
		minimapCamera.GetComponent<EdgeDetect>().debugMode = EdgeDetect.DebugMode.outlines;

		contentSize = largeView.GetComponentInChildren<ScrollRect>().GetComponent<RectTransform>().rect.width;
		contentSize -= largeView.GetComponentInChildren<ScrollRect>().viewport.GetComponent<RectTransform>().rect.xMin * 2;
		k = contentSize / 2 / largeMapSize;
	}

	Quaternion arrowRotation = new Quaternion();
	// Update is called once per frame
	void Update() {

		// bascule d'affichage de la minimap
		if (Input.GetKeyDown(KeyCode.M)) {
			mode = (mode + 1) % 3;
			minimapCamera.gameObject.SetActive(mode == 1);
			smallView.SetActive(mode == 1);
			largeView.SetActive(mode == 2);
			if (mode == 2) {
				largeView.GetComponentInChildren<ScrollRect>().normalizedPosition = new Vector2(
						Mathf.Clamp(((contentSize / 2) + (k * player.position.x) * 2) / (contentSize), 0, 1f),
						Mathf.Clamp(((contentSize / 2) + (k * player.position.z) * 2) / (contentSize), 0, 1f)
						);
	
			}
		}
		// orientation de la flèche
		if (mode == 1) {
			minimapCamera.transform.position = new Vector3(player.position.x, 100, player.position.z);
			arrowRotation = Quaternion.Euler(Vector3.Cross(player.rotation.eulerAngles, Vector3.right));
			smallArrow.transform.localRotation = arrowRotation;
		} else if (mode == 2) {
			arrowRotation = Quaternion.Euler(Vector3.Cross(player.rotation.eulerAngles, Vector3.right));
			largeArrow.transform.localPosition = new Vector3(k * player.position.x, k * player.position.z, 0);
			largeArrow.transform.localRotation = arrowRotation;
		}

		// capture de la grande carte
		if (Input.GetKeyDown(KeyCode.W)) {
			StartCoroutine(IMap());
		}


	}

	IEnumerator IMap() {
		largeMap = new RenderTexture(sqr, sqr, 24);
		minimapCamera.transform.position = new Vector3(0, 100, 0);
		minimapCamera.orthographicSize = largeMapSize;
		minimapCamera.targetTexture = largeMap;
		minimapCamera.GetComponent<EdgeDetect>().Init(sqr, sqr);
		minimapCamera.GetComponent<EdgeDetect>().debugMode = EdgeDetect.DebugMode.none;
		yield return new WaitForEndOfFrame();
		minimapCamera.Render();
		minimapCamera.targetTexture = smallMap;
		minimapCamera.orthographicSize = smallMapSize;
		minimapCamera.GetComponent<EdgeDetect>().Init(256, 256);
		minimapCamera.GetComponent<EdgeDetect>().debugMode = EdgeDetect.DebugMode.outlines;
		RenderTexture.active = largeMap;
		Texture2D virtualPhoto = new Texture2D(sqr, sqr, TextureFormat.RGB24, false);    // false, meaning no need for mipmaps
		virtualPhoto.ReadPixels(new Rect(0, 0, sqr, sqr), 0, 0);
		RenderTexture.active = null;    // "just in case" 
		Destroy(largeMap);                // "just in case" 
		SaveTextureToFile(virtualPhoto, "map");
	}


	//SAVE TEXTURE
	void SaveTextureToFile(Texture2D texture, string filename) {
		File.WriteAllBytes(Application.dataPath + "/Save/" + filename + ".png", texture.EncodeToPNG());
	}
	//LOAD TEXTURE
	Texture2D LoadTextureFromFile(string filename) {
		byte[] bytes;
		bytes = File.ReadAllBytes(Application.dataPath + "/Save/" + filename + ".png");
		var texture = new Texture2D(1, 1);
		texture.LoadImage(bytes);
		return texture;
	}
}
