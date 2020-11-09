using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public abstract class EntryUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
	IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public TMP_Text lowerText;
	public TMP_Text label;
	public Entry entry;

	public InventoryUI inventoryUI { get; set; }
	public int Index { get; set; }

	protected bool selected = false;

	private void Start() {
		inventoryUI = InventoryUI.Instance;
	}

	public abstract void Init(Entry entry);

	/// <summary>
	/// double clic pour 'consommer' un objet
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.clickCount % 2 == 0) {
			if (entry != null)
				InventoryUI.Instance.ObjectDoubleClicked(entry);
		}
	}

	/// <summary>
	/// début de survol
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerEnter(PointerEventData eventData) {
		inventoryUI.ObjectHoveredEnter(this);
	}

	/// <summary>
	/// fin de survol
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerExit(PointerEventData eventData) {
		inventoryUI.ObjectHoverExited(this);
	}

	/// <summary>
	/// mise à jour
	/// </summary>
	public abstract void UpdateEntry();

	/// <summary>
	/// début de glisser-déposer
	/// </summary>
	/// <param name="eventData"></param>
	public void OnBeginDrag(PointerEventData eventData) {
		inventoryUI.currentlyDragged = new InventoryUI.DragData();                                  // créer un 'dragData'
		inventoryUI.currentlyDragged.draggedEntry = this;                                           // qui contient cette entrée
		inventoryUI.currentlyDragged.originalParent = (RectTransform)transform.parent;              // dont on mémorise le parent actuel
		transform.SetParent(inventoryUI.dragCanvas.transform, true);                                // puis qu'on rattachera au canvas 'DragCanvas'
		inventoryUI.selectedEntry = this;
	}

	/// <summary>
	/// pendant le glisser-déposer
	/// </summary>
	/// <param name="eventData"></param>
	public void OnDrag(PointerEventData eventData) {
		transform.localPosition = transform.localPosition + UnscaleEventDelta(eventData.delta);     // tenir compte de l'échelle du DragCanvas
	}

	/// <summary>
	/// tenir compte de l'échelle du DragCanvas
	/// </summary>
	/// <param name="vec"></param>
	/// <returns></returns>
	Vector3 UnscaleEventDelta(Vector3 vec) {
		Vector2 referenceResolution = inventoryUI.DragCanvasScaler.referenceResolution;
		Vector2 currentResolution = new Vector2(Screen.width, Screen.height);
		float heightRatio = currentResolution.y / referenceResolution.y;
		return vec / heightRatio;
	}

	/// <summary>
	/// fin de glisser-déposer
	/// </summary>
	/// <param name="eventData"></param>
	public void OnEndDrag(PointerEventData eventData) {
		inventoryUI.HandledDroppedEntry(eventData.position);                            // gérer le 'drop'
		RectTransform t = transform as RectTransform;
		transform.SetParent(inventoryUI.currentlyDragged.originalParent, true);         // rattacher au parent original
		inventoryUI.currentlyDragged = null;                                            // supprimer le 'dragData'
		t.offsetMax = Vector2.zero;//-Vector2.one * 4;
		t.offsetMin = Vector2.zero;//Vector2.one * 4;
	}

	public virtual void Toggle() {
		Select(!selected);
	}

	public virtual void Select(bool on) {
		if (on) {
			transform.localPosition = new Vector2(0, 20);
			transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			inventoryUI.selectedEntry = this;
		} else {
			transform.localPosition = new Vector2(0, 0);
			transform.localScale = new Vector3(.9f, .9f, .9f);
			inventoryUI.selectedEntry = null;
		}
		label.enabled = on;
		selected = on;
	}
}
