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

	public QuitUI quitUi;                       // interface Quit	

	public int defaultCursorSize = 64;

	public bool isClicOnUI => IsPointerOverUIElement();//{ get; set; }                            // le clic en cours a-t-il débuté sur un élément d'interface ?


	Texture2D cursor;

	void Awake() {
		uiManager = this;
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
		//if (inventoryUI.selectedEntry) {
		//	cursor = Resize(inventoryUI.selectedEntry.item.ItemSprite.texture, defaultCursorSize);
		//	Cursor.SetCursor(cursor, new Vector2(defaultCursorSize / 3, defaultCursorSize / 3), CursorMode.ForceSoftware);
		//} else 
		if (IsMouseInActiveArea()) {
			cursor = playerManager.movementInput.cursor;
			Cursor.SetCursor(cursor, new Vector2(defaultCursorSize / 3, defaultCursorSize / 3), CursorMode.ForceSoftware);
		} else {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	Vector3 playerFeet => Camera.main.WorldToScreenPoint(playerManager.transform.position);
	bool IsMouseInActiveArea() {
		return Input.mousePosition.x > 0 &&
			Input.mousePosition.x < Screen.width &&
			Input.mousePosition.y > Screen.height * playerFeet.y / Screen.height &&
			Input.mousePosition.y < Screen.height &&
			(Input.mousePosition - playerFeet).magnitude > 50;
	}

}
