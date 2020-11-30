using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using static App;

public abstract class EntryUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public TMP_Text lowerText;
	public TMP_Text label;
	public Entry entry;

	public int Index { get; set; }
	public Loot item { get; set; }

	protected bool selected = false;


	public abstract void Init(Entry entry);

	/// <summary>
	/// double clic pour 'consommer' un objet
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.clickCount % 2 == 0) {
			if (entry != null)
				inventoryUI.ObjectDoubleClicked(entry);
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

	public virtual void Toggle() {
		Select(!selected);
	}

	public virtual void Select(bool on) {
		if (on) {
			transform.localPosition = new Vector2(0, 20);
			transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			inventoryUI.selectedEntry = this;
			targetsManager.OnItemSelect();
		} else {
			transform.localPosition = new Vector2(0, 0);
			transform.localScale = new Vector3(.9f, .9f, .9f);
			inventoryUI.selectedEntry = null;
		}
		label.enabled = on;
		selected = on;
	}
}
