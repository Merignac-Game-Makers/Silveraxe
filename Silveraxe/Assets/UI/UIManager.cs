using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;


/// <summary>
/// Gestionnaire général des interfaces (Dialogues, Inventaire, Magie ou QUêtes)
/// </summary>
public class UIManager : App
{

	public enum State { noMagic, openBook, closedBook, dialog, end, quit }  // les états possibles de l'UI
	public State state { get; private set; }    // l'état actuel de l'UI
	private State prevState;                    // l'état précédent de l'UI

	//public static UIManager Instance;

	public DialoguesUI dialoguesUI;             // interface Dialogues
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

	public int defaultCursorSize = 64;

	Coroutine coroutine;

	Texture2D cursor;
	Stack<Texture2D> cursorStack;


	void Awake() {
		uiManager = this;
	}

	private void Start() {
		cursorStack = new Stack<Texture2D>();
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
		playerManager.isClicOnUI = false;
	}

	/// <summary>
	/// Redimensionner une texture
	/// </summary>
	RenderTexture rt;
	Texture2D tempTexture;
	public Texture2D Resize(Texture2D tex, int size) {
		rt = new RenderTexture(size, size, 32);
		RenderTexture.active = rt;
		Graphics.Blit(tex, rt);
		tempTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
		tempTexture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
		tempTexture.alphaIsTransparency = true;
		tempTexture.Apply();
		return tempTexture;
	}


	public void SetCursor(Texture2D tex) {
		SetCursor(tex, defaultCursorSize);
	}

	public void SetCursor(Texture2D tex, int size) {
		cursor = Resize(tex, size);
		cursorStack.Push(cursor);

		Cursor.SetCursor(cursor, new Vector2(size / 2, size / 2), CursorMode.ForceSoftware);
	}

	public void ResetCursor() {
		if (cursorStack.Count > 0) {
			cursorStack.Pop();
			if (cursorStack.Count > 0) {
				cursor = cursorStack.Peek();
				Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
			} else {
				SetBaseCursor(playerManager.GetComponent<MovementInput>().shouldMove);
			}
		}
	}

	public void SetBaseCursor(bool shouldMove) {
		if (shouldMove) {
			Cursor.SetCursor(Resize(playerManager.GetComponent<MovementInput>().footSteps, defaultCursorSize), new Vector2(defaultCursorSize/2, defaultCursorSize / 2), CursorMode.ForceSoftware);
		} else {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

}
