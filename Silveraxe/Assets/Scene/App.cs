using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

/// <summary>
/// Objets statiques disponibles dans l'ensemble de l'application
/// </summary>
public static class App {

	public static CameraController cameraController;

	public static UIManager uiManager;
	//public static InventoryManager inventoryManager;
	public static SFXManager sfxManager;
	public static PlayerManager playerManager;
	public static MessageManager messageManager;
	public static InteractableObjectsManager interactableObjectsManager;
	public static TargetsManager targetsManager;

	public static DialoguesUI dialogueUI;
	public static InventoryUI inventoryUI;
	public static EquipmentUI equipmentUI;
	public static StatsUI statsUI;


	//--------------------
	// strings
	public const string DIALOGUE = "Dialogue";
	public const string FIGHT = "Fight";


	//--------------------
	// Texture <=> fichier
	/// SAVE TEXTURE
	public static void SaveTextureToFile(Texture2D texture, string path, string filename) {
		File.WriteAllBytes(Application.dataPath + "/" + path + "/" + filename + ".png", texture.EncodeToPNG());
	}
	/// LOAD TEXTURE
	public static Texture2D LoadTextureFromFile(string path, string filename) {
		byte[] bytes;
		bytes = File.ReadAllBytes(Application.dataPath + "/" + path + "/" + filename + ".png");
		var texture = new Texture2D(1, 1);
		texture.LoadImage(bytes);
		return texture;
	}

	//--------------------
	/// <summary>
	/// trouver un 'transform' par son nom parmi les descendants dans toute la hiérarchie
	/// </summary>
	/// <param name="aParent"></param>
	/// <param name="aName"></param>
	/// <returns></returns>
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


	//-----------------------------------------------------------
	// Vérifier si le dernier clic a eu lieu sur un objet de l'UI
	/// 
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

	//----------------------------------------------------------------------------------------------
	// Extension de NavMesh Agent 'set destination' pour ajouter un callback à la fin du déplacement
	public static NavMeshAgent SetDestination(this NavMeshAgent navAgent, Vector3 pos, Action callback = null) {
		playerManager.StopCoroutine("Igoto");
		playerManager.StartCoroutine(Igoto(navAgent, pos, callback));
		return navAgent;
	}
	static IEnumerator Igoto(NavMeshAgent navAgent, Vector3 pos, Action callback) {
		navAgent.updateRotation = true;
		navAgent.SetDestination(pos);
		if (callback != null) {
			while (navAgent.remainingDistance > navAgent.radius)
				yield return new WaitForEndOfFrame();
			callback();
		}
	}

	// Extension pour permettre la sérialisation / désérialisation de Vector3
	public static float[] toArray(this Vector3 vector) {
		return new float[] { vector.x, vector.y, vector.z };
	}
	public static Vector3 toVector(this float[] array) {
		return new Vector3(array[0], array[1], array[2]);
	}

	// Extension pour permettre la sérialisation / désérialisation de Quaternion
	public static float[] toArray(this Quaternion quaternion) {
		return new float[] { quaternion.x, quaternion.y, quaternion.z, quaternion.w };
	}
	public static Quaternion toQuaternion(this float[] array) {
		return new Quaternion(array[0], array[1], array[2], array[3]);
	}
}
