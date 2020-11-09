using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventoryManager;

/// <summary>
/// Handle all the UI code related to the inventory (drag'n'drop of object, using objects, equipping object etc.)
/// </summary>
public class InventoryUI : UIBase
{
	public class DragData
	{
		public EntryUI draggedEntry;
		public RectTransform originalParent;
	}

	//public CombineUI combineUI;
	//public GameObject bookPanel;
	public GameObject content;

	public ItemEntryUI itemEntryPrefab;
	public RectTransform slotPrefab;

	public Canvas dragCanvas;

	public static InventoryUI Instance;

	// Raycast
	readonly RaycastHit[] m_RaycastHitCache = new RaycastHit[16];
	int m_TargetLayer;


	//public GameObject combinePanel { get; private set; }
	public DragData currentlyDragged { get; set; }
	public CanvasScaler DragCanvasScaler { get; private set; }

	public List<ItemEntryUI> entries { get; private set; } = new List<ItemEntryUI>();

	public EntryUI selectedEntry { get; set; }
	EntryUI hoveredItem;
	InventoryPanel iPanel;
	//HighlightableObject item;
	UIManager uiManager;

	bool? prevStatus = false;

	private void Awake() {
		Instance = this;

	}

	public override void Init(UIManager uiManager) {
		this.uiManager = uiManager;

		//combinePanel = combineUI.gameObject;
		//combinePanel.SetActive(false);

		iPanel = panel.GetComponent<InventoryPanel>();
		gameObject.SetActive(true);

		currentlyDragged = null;

		DragCanvasScaler = dragCanvas.GetComponentInParent<CanvasScaler>();

		m_TargetLayer = 1 << LayerMask.NameToLayer("Interactable");
	}

	void OnEnable() {
		hoveredItem = null;
	}

	public ItemEntryUI AddItemEntry(int idx, InventoryEntry inventoryEntry) {
		RectTransform slot = Instantiate(slotPrefab, content.transform);        // créer un nouvel emplacement
		ItemEntryUI itemEntry = Instantiate(itemEntryPrefab, slot);             // créer une nouvelle entrée d'inventaire dans cet emplacement																				//itemEntry.gameObject.SetActive(true);
		itemEntry.Init(inventoryEntry);
		if (entries.Count == 0)                                                 // si c'est le 1er objet
			Show();                                                             // montrer l'inventaire
		entries.Add(itemEntry);
		return itemEntry;
	}

	/// <summary>
	/// détruire une entrée
	/// </summary>
	/// <param name="entryUi"></param>
	public void RemoveEntry(ItemEntryUI entryUi) {
		Destroy(entryUi.transform.parent.gameObject);       // détruire le slot qui contient l'entrée
		entries.Remove(entryUi);
		if (entries.Count == 0)                                                 // si l'inventaire est vide
			Hide();                                                             // cacher l'inventaire
	}

	/// <summary>
	/// bascule d'affichage
	/// </summary>
	public override void Toggle() {
		//iPanel.Toggle(combinePanel);
		iPanel.Toggle();
	}
	public void Hide() {
		//iPanel.Hide(combinePanel);
		iPanel.Hide();
	}
	public void Show() {
		iPanel.Show();
	}

	/// <summary>
	/// Actualiser l'affichage de toutes les entrés d'iventaire
	/// </summary>
	public void UpdateEntries() {
		//this.item = item;
		for (int i = entries.Count - 1; i > 0; i--) {
			if ((entries[i].entry as InventoryEntry).count <= 0) {
				Destroy(entries[i].gameObject);
				entries.RemoveAt(i);
			} else {
				entries[i].UpdateEntry();
			}
		}
	}

	/// <summary>
	/// utiliser un objet (ex: boire une potion...)
	/// (inutilisé pour l'instant)
	/// </summary>
	/// <param name="usedItem"></param>
	public void ObjectDoubleClicked(Entry usedItem) {
		InventoryManager.Instance.UseItem(usedItem);
		ObjectHoverExited(hoveredItem);
	}

	/// <summary>
	/// Survol par le pointeur de souris
	/// TODO: pour l'instant sans effet... à conserver ???
	/// </summary>
	/// <param name="hovered">L'entrée sous la souris</param>
	public void ObjectHoveredEnter(EntryUI hovered) {       // début de survol
		hoveredItem = hovered;
	}
	public void ObjectHoverExited(EntryUI exited) {         // fin de survol
		if (hoveredItem == exited) {
			hoveredItem = null;
		}
	}

	public void HandledDroppedEntry(Vector3 position) {
		// check for drop on ItemSlots
		for (int i = 0; i < content.transform.childCount; ++i) {                                // pour chaque slot
			var slot = content.transform.GetChild(i).GetComponent<RectTransform>();
			if (RectTransformUtility.RectangleContainsScreenPoint(slot, position)) {            // si on lache sur ce slot
				var entryUi = slot.GetComponentInChildren<ItemEntryUI>();                       // récuperer l'entrée contenue dans ce slot

				if (entryUi != null) {                                                          // s'il y a déjà une entrée => déplacer l'entrée
					var prevParent = entryUi.transform.parent;
					entryUi.transform.SetParent(currentlyDragged.originalParent, false);        // vers le slot vide
					currentlyDragged.originalParent = prevParent as RectTransform;
					currentlyDragged.draggedEntry.UpdateEntry();                                // mettre l'entrée déposée à jour
					return;
				}
			}
		}
		// check for drop on 3D target
		//DropOn3D(PlayerManager.Instance.m_InvItemDragging.draggedEntry.entry);
		DropOn3D(currentlyDragged.draggedEntry.entry);
	}

	/// <summary>
	/// Gérer la dépose sur un objet 3D du décor
	/// l'entry peut être :
	///		- un objet d'inventaire
	///		- un orbe de magie
	///	le lieu de dépôt doit être dans le layer "Target"
	/// </summary>
	/// <param name="entry">l'entrée d'inventaire contenant l'item à déposer</param>
	public void DropOn3D(Entry entry) {
		Ray screenRay = CameraController.Instance.GameplayCamera.ScreenPointToRay(Input.mousePosition);         // lancer de rayon
		int count = Physics.SphereCastNonAlloc(screenRay, 1.0f, m_RaycastHitCache, 1000.0f, m_TargetLayer);     // combien d'objets du layer 'interactable'sous le pointeur ?
		if (count > 0) {                                                            // s'il y a des objets sous le pointeur
			foreach (RaycastHit rh in m_RaycastHitCache) {                          // pour chacun d'eux
				if (rh.collider != null) {                                          // si l'objet a un collider
					if (entry is InventoryEntry) {                                                      // si on dépose un objet d'inventaire
						//bool combinable = (entry as InventoryEntry).item.combinable;

						Target data = rh.collider.GetComponentInParent<Target>();                       // si l'objet est une 'target' (lieu de dépôt d'objet d'inventaire autorisé)
						if (data != null && data.isAvailable((entry as InventoryEntry).item)) {         // et que cet emplacement est libre et que l'objet actuel n'est pas combinable
							PlayerManager.Instance.RequestInteraction(data);                            // aller jusqu'à la cible puis déposer l'objet d'inventaire
							break;
						} else {
							uiManager.ShowLabel("Impossible d'utiliser cet objet ici", Input.mousePosition);
						}
					} 
					//else if (entry is OrbEntry) {                                                     // si on dépose un orbe de magie
					//	MagicEffectBase data = rh.collider.GetComponentInChildren<MagicEffectBase>();   // si l'objet est une ' magic target' (lieu de dépôt d'orbe autorisé)
					//	DropItem(data, entry as OrbEntry);                                              // déposer l'objet d'inventaire
					//	break;
					//}
				}
			}
		} else {
			uiManager.ShowLabel("Impossible d'utiliser cet objet ici", Input.mousePosition);
		}
	}

	/// <summary>
	/// Déposer un objet d'inventaire
	/// </summary>
	/// <param name="target">le lieu</param>
	/// <param name="entry">l'entrée d'inventaire </param>
	public void DropItem(Target target, Entry entry) {
		if (entry is InventoryEntry) {
			Loot item = (entry as InventoryEntry).item;
			item.animate = true;
			CreateWorldRepresentation(item, target);													// créer l'objet 3D
			if (currentlyDragged != null)                                                               // si on est dans un 'drag & drop'
				currentlyDragged.draggedEntry.transform.SetParent(currentlyDragged.originalParent);     // rattacher le 'drag & drop' à son parent original
			InventoryManager.Instance.RemoveItem(item.entry);											// retirer l'objet déposé de l'inventaire
		}
	}

	///// <summary>
	///// Déposer un orbe de magie 
	///// </summary>
	///// <param name="target">le lieu</param>
	///// <param name="entry">l'entrée orbe</param>
	//private void DropItem(MagicEffectBase target, OrbEntry entry) {
	//	if (target != null && target.isFree) {          // si on lâche l'orbe sur une cible de magie
	//		target.MakeMagicalStuff(entry.orb);         // déclencher la magie
	//		entry.ui.Select(false);
	//	} else {
	//		uiManager.ShowLabel("Impossible ici", Input.mousePosition);
	//	}

	//}

	void CreateWorldRepresentation(Loot item, Target target) {
		var pos = target.gameObject.transform.position + Vector3.up * item.prefab.gameObject.transform.localScale.y / 2;
		// if the item have a world object prefab set use that...
		if (item.prefab != null) {
			var obj = Instantiate(item.prefab, pos, new Quaternion());
			obj.transform.parent = target.gameObject.transform;
			obj.layer = LayerMask.NameToLayer("Interactable");
		}
	}

	void CreatePrefab() {
		//var v = System.AppDomain.CurrentDomain.GetAssemblies()
			//.SelectMany(assembly => assembly.GetTypes())
			//.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
			//.Select(type => type.Name)
			//.ToList();
	}

	public void SaveAndHide() {
		Save();
		Hide();
	}
	public void Save() {
		prevStatus = iPanel.isOn;
	}

	public void Restore() {
		if (iPanel.isOn != prevStatus)
			Toggle();
		prevStatus = null;
	}
}
