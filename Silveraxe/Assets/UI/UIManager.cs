using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using static App;
using System;

/// <summary>
/// Gestionnaire général des interfaces (Dialogues, Inventaire, Magie ou QUêtes)
/// </summary>
public class UIManager : MonoBehaviour
{

	public enum State { noMagic, openBook, closedBook, dialog, end, quit }  // les états possibles de l'UI
	public State state { get; private set; }    // l'état actuel de l'UI
	private State prevState;                    // l'état précédent de l'UI

	public MenuUI menuUI;                               // interface Menu principal	

	public Texture2D dialogueIcon;
	public Texture2D takeIcon;
	public Texture2D dropIcon;
	public Texture2D fightIcon;

	public int defaultCursorSize = 64;

	public bool isClicOnUI => IsPointerOverUIElement();//{ get; set; }                            // le clic en cours a-t-il débuté sur un élément d'interface ?


	Texture2D cursor;

	void Awake() {
		uiManager = this;
	}

	private void Start() {
		menuUI.Show(true);
	}

	private void Update() {
		DefineCursor();

		// quitter le jeu par la touche escape
		if (Input.GetKeyDown(KeyCode.Escape)) {
			ShowQuitUi();
		}
	}

	public void ShowQuitUi() {
		ManageButtons(State.quit);
		menuUI.Toggle();
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
				//inventoryUI.SaveAndHide();
				//exitButton.SaveAndHide();
				break;
			case State.quit:
				//inventoryUI.SaveAndHide();
				//exitButton.SaveAndHide();
				break;
			default:
				//inventoryUI.Restore();
				//exitButton.Restore();
				break;
		}
	}

	public void RestoreButtonsPreviousState() {
		ManageButtons(prevState);
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



	public void DefineCursor() {
		if (IsMouseInActiveArea()) {
			if (cursor != playerManager.movementInput.cursor) {
				cursor = playerManager.movementInput.cursor;
				Cursor.SetCursor(cursor, new Vector2(defaultCursorSize / 3, defaultCursorSize / 3), CursorMode.ForceSoftware);
			}
		} else {
			if (cursor) {
				cursor = null;
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
		}
	}

	bool IsMouseInActiveArea() {
		return InScreen() && !App.IsPointerOverUIElement();
	}

	bool InScreen() {
		return Input.mousePosition.x > 0 &&
			Input.mousePosition.x < Screen.width &&
			Input.mousePosition.y > 0 &&
			Input.mousePosition.y < Screen.height;
	}
}
