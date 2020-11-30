using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using static App;

public abstract class EntryUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
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

	public abstract void Select(bool on);
}
