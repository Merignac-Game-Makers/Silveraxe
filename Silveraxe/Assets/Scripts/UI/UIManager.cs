using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;

/// <summary>
/// Gestionnaire général des interfaces (Dialogues, Inventaire, Magie ou QUêtes)
/// </summary>
public class UIManager : MonoBehaviour
{

	public enum State { noMagic, openBook, closedBook, dialog, end, quit }  // les états possibles de l'UI
	public State state { get; private set; }    // l'état actuel de l'UI
	private State prevState;                    // l'état précédent de l'UI

	public static UIManager Instance;

	public DialoguesUI dialoguesUI;             // interface Dialogues
	public InventoryUI inventoryUI;             // interface Inventaire
	//public DiaryBookContent diaryBookContent;   // pages du journal
	//public MagicUI magicUI;                     // interface Magie
	public QuitUI quitUi;                       // interface Quit

	//public GameObject magicButton;              // bouton du grimoire		
	//public Button artifactButton;               // bouton des artefacts		
	//public Exit exitButton;                     // bouton exit
	//public Button questButton;                  // bouton des quêtes		
	//public Button diaryButton;                  // bouton du journal		

	public GameObject messageLabel;
	public GameObject forbidden;

	Coroutine coroutine;

	void Awake() {
		Instance = this;
	}

	private void Start() {
		dialoguesUI.Init(this);                 // initialisation du gestionnaire de dialogues
		inventoryUI.Init(this);                 // initialisation du gestionnaire d'inventaire
		//diaryBookContent.Init();                // initialisation des pages du journal
		//magicUI.Init(this);                     // intialisation du gestionnaire de magie
		//questButton.gameObject.SetActive(false);            // masquer le bouton des quêtes
		//diaryButton.gameObject.SetActive(false);            // masquer le bouton du journal
	}

	public void ShowQuitUi() {
		ManageButtons(State.quit);
		quitUi.Show(true);
	}

	/// <summary>
	/// Gérer la coordination d'affichage des boutons
	/// (masquer le bouton grimoire quand on affiche l'inventaire ou les quêtes, ...)
	/// </summary>
	public void ManageButtons(State state) {
		prevState = this.state;                 // mémoriser l'état précédent de l'UI
		this.state = state;                     // mémoriser le nouvel état de l'UI
			//magicButton.SetActive(false);
			//questButton.gameObject.SetActive(false);
			//diaryButton.gameObject.SetActive(false);
			//artifactButton.gameObject.SetActive(false);
			switch (state) {
				case State.dialog:
					inventoryUI.SaveAndHide();
					//exitButton.SaveAndHide();
					break;
				case State.quit:
					inventoryUI.SaveAndHide();
					//exitButton.SaveAndHide();
					break;
				default:
					inventoryUI.Restore();
					//exitButton.Restore();
					break;
			}
	}

	public void RestoreButtonsPreviousState() {
		ManageButtons(prevState);
	}

	/// <summary>
	/// afficher un message
	/// </summary>
	/// <param name="text">le message</param>
	/// <param name="position">la position d'affichage</param>
	public void ShowLabel(string text, Vector2 position) {
		messageLabel.GetComponentInChildren<TMP_Text>().text = text;
		messageLabel.transform.position = position;
		if (coroutine != null)
			StopCoroutine(coroutine);
		coroutine = StartCoroutine(IShow(messageLabel, 2));
	}

	/// <summary>
	/// afficher un message
	/// </summary>
	/// <param name="text">le message</param>
	/// <param name="position">la position d'affichage</param>
	public void Forbidden(Vector2 position, int delay) {
		forbidden.transform.position = position;
		if (coroutine != null)
			StopCoroutine(coroutine);
		coroutine = StartCoroutine(IShow(forbidden, delay));
	}

	IEnumerator IShow(GameObject obj, float s) {
		obj.SetActive(true);
		yield return new WaitForSeconds(s);
		obj.SetActive(false);
		PlayerManager.Instance.isClicOnUI = false;
	}

}
