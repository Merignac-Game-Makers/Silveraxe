using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Objets statiques disponibles dans l'ensemble de l'application
/// </summary>
public static class App {

	public static CameraController cameraController;

	public static UIManager uiManager;
	public static InventoryManager inventoryManager;
	public static SFXManager sfxManager;
	public static PlayerManager playerManager;
	public static MessageManager messageManager;
	public static InteractableObjectsManager interactableObjectsManager;
	public static TargetsManager targetsManager;

	public static DialoguesUI dialogueUI;
	public static InventoryUI inventoryUI;




	//SAVE TEXTURE
	public static void SaveTextureToFile(Texture2D texture, string path, string filename) {
		File.WriteAllBytes(Application.dataPath + "/" + path + "/" + filename + ".png", texture.EncodeToPNG());
	}

	//LOAD TEXTURE
	public static Texture2D LoadTextureFromFile(string path, string filename) {
		byte[] bytes;
		bytes = File.ReadAllBytes(Application.dataPath + "/" + path + "/" + filename + ".png");
		var texture = new Texture2D(1, 1);
		texture.LoadImage(bytes);
		return texture;
	}

	// trouver un 'child' dans toute la hiérarchie
	public static Transform FindDeepChild(this Transform aParent, string aName) {
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0) {
			var c = queue.Dequeue();
			if (c.name.ToLower() == aName.ToLower())
				return c;
			foreach (Transform t in c)
				queue.Enqueue(t);
		}
		return null;
	}


	///Returns 'true' if we touched or hovering on Unity UI element.
	public static bool IsPointerOverUIElement() {
		return IsPointerOverUIElement(GetEventSystemRaycastResults());
	}
	///Returns 'true' if we touched or hovering on Unity UI element.
	public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
		for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
			RaycastResult curRaysastResult = eventSystemRaysastResults[index];
			if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
				return true;
		}
		return false;
	}
	///Gets all event systen raycast results of current mouse or touch position.
	static List<RaycastResult> GetEventSystemRaycastResults() {
		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position = Input.mousePosition;
		List<RaycastResult> raysastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, raysastResults);
		return raysastResults;
	}
}
