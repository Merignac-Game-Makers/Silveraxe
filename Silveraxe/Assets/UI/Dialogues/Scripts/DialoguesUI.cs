using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VIDE_Data;
using TMPro;
using static UIManager.State;
using UnityEngine.Animations;

public class DialoguesUI : UIBase
{

	public static DialoguesUI Instance;

	//public GameObject questButton;
	//public GameObject diaryButton;
	public GameObject inventory;

	public GameObject container_NPC;
	public Image NPC_Sprite;
	public Text NPC_label;
	public TMP_Text text_NPC;

	public GameObject container_PLAYER;
	public Text PLAYER_Label;
	public TMP_Text text_PLAYER;
	public GameObject Buttons;
	public TMP_Text[] choices;

	[HideInInspector]
	public GameObject currentPNJcam;

	UIManager uiManager;

	private void Awake() {
				Instance = this;
	}

	public override void Init(UIManager uiManager) {
		this.uiManager = uiManager;

		gameObject.SetActive(true);
		panel.SetActive(false);

	}

	// Start is called before the first frame update
	void Start() {
		container_NPC.SetActive(false);
		container_PLAYER.SetActive(false);
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (VD.isActive)
				VD.Next();
		}
	}

	/// <summary>
	/// Commencer le dialogue
	/// </summary>
	/// <param name="dialog"></param>
	public void Begin(VIDE_Assign dialog) {
		PlayerManager.Instance.StopAgent();
		//panel.SetActive(true);
		currentPNJcam = dialog.GetComponentInParent<PNJ>().PNJcam;
		if (currentPNJcam!=null)
			currentPNJcam.SetActive(true);
		if (string.IsNullOrEmpty(dialog.alias))
			dialog.alias = dialog.GetComponentInParent<PNJ>().PNJName;
		Show();
		VD.OnNodeChange += UpdateUI;
		VD.OnEnd += End;
		if (VD.isActive)
			VD.Next(); 
		else {
			VD.BeginDialogue(dialog);
		}
	}

	public override void Toggle() {
		panel.SetActive(!isOn);
		//if (App.isMagicEquiped) {
		//	questButton.SetActive(!isOn);
		//	diaryButton.SetActive(!isOn);
		//}
		inventory.SetActive(!isOn);
	}
	public void Show() {
		panel.SetActive(true);
		uiManager.ManageButtons(dialog);
		//InventoryUI.Instance.SaveAndHide();
		//if (App.isMagicEquiped) {
		//	questButton.SetActive(false);
		//	diaryButton.SetActive(!isOn);
		//}
		//inventory.SetActive(false);
	}
	public void Hide() {
		panel.SetActive(false);
		uiManager.RestoreButtonsPreviousState();
		//InventoryUI.Instance.Restore();
		//if (App.isMagicEquiped) {
		//	questButton.SetActive(true);
		//	diaryButton.SetActive(!isOn);
		//}
		//inventory.SetActive(true);
	}


	void UpdateUI(VD.NodeData data) {
		container_NPC.SetActive(false);
		container_PLAYER.SetActive(false);


		if (data.isPlayer) {
			container_PLAYER.SetActive(true);
			// set sprite
			// TODO: revoir la mise en place de sprite spécifique

			//if (data.creferences[data.commentIndex].sprites != null)
			//	PLAYER_Sprite.sprite = data.creferences[data.commentIndex].sprites;    // specific for comment i exists
			//else if (data.sprite != null)
			//	PLAYER_Sprite.sprite = data.sprite;
			//else if (VD.assigned.defaultPlayerSprite != null)
			//	PLAYER_Sprite.sprite = VD.assigned.defaultPlayerSprite;

			// set name
			//If it has a tag, show it, otherwise let's use the alias we set in the VIDE Assign
			if (data.tag.Length > 0)
				PLAYER_Label.text = data.tag;
			else
				PLAYER_Label.text = "Player";

			// set choices
			for (int i = 0; i < choices.Length; i++) {
				if (i < data.comments.Length) {
					choices[i].transform.parent.gameObject.SetActive(true);
					choices[i].text = data.comments[i];
				} else {
					choices[i].transform.parent.gameObject.SetActive(false);
				}
			}
		} else {
			container_NPC.SetActive(true);

			// set sprite
			if (data.creferences[data.commentIndex].sprites != null)				
				NPC_Sprite.sprite = data.creferences[data.commentIndex].sprites;	// specific for comment i exists
			else if (data.sprite != null)		
				NPC_Sprite.sprite = data.sprite;									// specific for node if exists
			else if (VD.assigned.defaultNPCSprite != null)
				NPC_Sprite.sprite = VD.assigned.defaultNPCSprite;					// for dialog

			// set name
			// If it has a tag, show it, otherwise let's use the alias we set in the VIDE Assign
			if (data.tag.Length > 0)
				NPC_label.text = data.tag;
			else
				NPC_label.text = VD.assigned.alias;

			// set text
			text_NPC.text = data.comments[data.commentIndex];
		}
	}

	
	/// <summary>
	/// Terminer le dialogue
	/// </summary>
	/// <param name="data"></param>
	public void End(VD.NodeData data) {
		Hide();
		if (currentPNJcam!=null)
			currentPNJcam.SetActive(false);
		container_NPC.SetActive(false);
		container_PLAYER.SetActive(false);
		VD.OnNodeChange -= UpdateUI;
		VD.OnEnd -= End;
		VD.EndDialogue();
	}
	private void OnDisable() {
		if (container_NPC != null)
			End(null);
	}

	public void GetChoice(int choice) {
		VD.nodeData.commentIndex = choice;
		if (Input.GetMouseButtonUp(0))
			VD.Next();
	}

	public void nextNPC() {
		if (container_NPC.activeInHierarchy)
			VD.Next();
	}

	public void Next() {
		VD.Next();
	}

	public enum QuestStatus { None, Accepted, Refused, Done}
	[HideInInspector]
	public QuestStatus questStatus = QuestStatus.None;
	public void RefuseQuest() {
		questStatus = QuestStatus.Refused;
	}
	public void AcceptQuest() {
		questStatus = QuestStatus.Accepted;
	}
	public void QuestDone() {
		questStatus = QuestStatus.Done;
	}
}
