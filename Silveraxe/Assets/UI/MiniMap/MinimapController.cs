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

	RenderTexture largeMap;
	EdgeDetect edgeDetect;

	int mode = 0;               // 0 = pas de carte; 1 * mini carte//; 2 = grande carte
	GameObject smallContent;
	float smallContentSize;
	float kSmall = 1;
	float largeContentSize;
	float kLarge = 1;

	Transform player;
	// Start is called before the first frame update
	void Start() {
		edgeDetect = minimapCamera.GetComponent<EdgeDetect>();
		player = PlayerManager.Instance.transform;

		smallContent = smallView.GetComponentInChildren<RawImage>().gameObject;
		smallView.GetComponentInChildren<RawImage>().texture = LoadTextureFromFile("map-outlines");
		smallContentSize = smallContent.GetComponent<RectTransform>().rect.width;
		kSmall =  - smallContentSize / smallView.GetComponent<RectTransform>().rect.width / 2;

		largeView.GetComponentInChildren<RawImage>().texture = LoadTextureFromFile("map");
		largeContentSize = largeView.GetComponentInChildren<ScrollRect>().GetComponent<RectTransform>().rect.width;
		largeContentSize -= largeView.GetComponentInChildren<ScrollRect>().viewport.GetComponent<RectTransform>().rect.xMin * 2;
		kLarge = largeContentSize / 2 / largeMapSize;
	}

	Quaternion arrowRotation = new Quaternion();
	// Update is called once per frame
	void Update() {

		// bascule d'affichage de la minimap
		if (Input.GetKeyDown(KeyCode.M)) {
			mode = (mode + 1) % 3;
			smallView.SetActive(mode == 1);
			largeView.SetActive(mode == 2);
			if (mode == 1) {
				smallView.GetComponentInChildren<RawImage>().texture = LoadTextureFromFile("map-outlines");
			} else if (mode == 2) {
				largeView.GetComponentInChildren<RawImage>().texture = LoadTextureFromFile("map");
				largeView.GetComponentInChildren<ScrollRect>().normalizedPosition = new Vector2(
						Mathf.Clamp(((largeContentSize / 2) + (kLarge * player.position.x) * 2) / (largeContentSize), 0, 1f),
						Mathf.Clamp(((largeContentSize / 2) + (kLarge * player.position.z) * 2) / (largeContentSize), 0, 1f)
						);
	
			}
		}
		// orientation de la flèche
		if (mode == 1) {
			//minimapCamera.transform.position = new Vector3(player.position.x, 100, player.position.z);
			smallContent.transform.localPosition = new Vector2(
					((kSmall * player.position.x)),
					((kSmall * player.position.z))
					);

			arrowRotation = Quaternion.Euler(Vector3.Cross(player.rotation.eulerAngles, Vector3.right));
			smallArrow.transform.localRotation = arrowRotation;
		} else if (mode == 2) {
			arrowRotation = Quaternion.Euler(Vector3.Cross(player.rotation.eulerAngles, Vector3.right));
			largeArrow.transform.localPosition = new Vector3(kLarge * player.position.x, kLarge * player.position.z, 0);
			largeArrow.transform.localRotation = arrowRotation;
		}

		// capture de la grande carte
		if (Input.GetKeyDown(KeyCode.W)) {
			StartCoroutine(IMap());
		}


	}

	Texture2D virtualPhoto;
	IEnumerator IMap() {
		var ds = edgeDetect.depthSensitivity;
		var ds2 = edgeDetect.depthSensitivity2;
		var ns = edgeDetect.normalsSensitivity;

		largeMap = new RenderTexture(sqr, sqr, 24);
		minimapCamera.transform.position = new Vector3(0, 100, 0);
		minimapCamera.orthographicSize = largeMapSize;
		minimapCamera.targetTexture = largeMap;
		minimapCamera.gameObject.SetActive(true);
		edgeDetect.Init(sqr, sqr);

		// map "satellite"
		minimapCamera.GetComponent<EdgeDetect>().debugMode = EdgeDetect.DebugMode.none;
		edgeDetect.depthSensitivity = 0;
		edgeDetect.depthSensitivity2 = 0;
		edgeDetect.normalsSensitivity = 0;
		yield return new WaitForEndOfFrame();
		minimapCamera.Render();
		//-- save
		RenderTexture.active = largeMap;
		virtualPhoto = new Texture2D(sqr, sqr, TextureFormat.RGB24, false);    // false, meaning no need for mipmaps
		virtualPhoto.ReadPixels(new Rect(0, 0, sqr, sqr), 0, 0);
		RenderTexture.active = null;    // "just in case" 
		SaveTextureToFile(virtualPhoto, "map");

		// map "outlines"
		edgeDetect.debugMode = EdgeDetect.DebugMode.outlines;
		edgeDetect.depthSensitivity = ds;
		edgeDetect.depthSensitivity2 = ds2;
		edgeDetect.normalsSensitivity = ns; 
		yield return new WaitForEndOfFrame();
		minimapCamera.Render();
		//-- save
		RenderTexture.active = largeMap;
		virtualPhoto = new Texture2D(sqr, sqr, TextureFormat.RGBA32, false);    // false, meaning no need for mipmaps
		virtualPhoto.ReadPixels(new Rect(0, 0, sqr, sqr), 0, 0);
		RenderTexture.active = null;    // "just in case" 
		SaveTextureToFile(virtualPhoto, "map-outlines");
		//Destroy(largeMap);                // "just in case" 
		minimapCamera.gameObject.SetActive(false);
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
