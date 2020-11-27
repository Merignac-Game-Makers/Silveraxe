using UnityEngine;
using UnityEngine.UI;
using VIDE_Data;
using TMPro;

using static App;

using static UIManager.State;
using Cinemachine;
using System.Collections;

public class DialoguesUI : UIBase
{

	//public GameObject questButton;
	//public GameObject diaryButton;
	public GameObject inventory;

	public GameObject container_NPC;
	public TMP_Text NPC_label;
	public TMP_Text text_NPC;

	public GameObject container_PLAYER;
	public TMP_Text PLAYER_Label;
	public TMP_Text text_PLAYER;
	public GameObject Buttons;
	public TMP_Text[] choices;

	private PNJ pnj;

	private void Awake() {
		dialogueUI = this;
	}


	// Start is called before the first frame update
	void Start() {
		container_NPC.SetActive(false);
		container_PLAYER.SetActive(false);
		gameObject.SetActive(true);
		panel.SetActive(false);

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
		// personnages
		pnj = dialog.GetComponentInParent<PNJ>();                               // retrouver le PNJ																				
																				// interface dialogues
		pnj.PNJcam?.SetActive(true);                                            // activer la caméra 'portait' du PNJ
		if (string.IsNullOrEmpty(dialog.alias))
			dialog.alias = pnj.PNJName;                                         // indiquer le nom du PNJ
																				//Show();																	// afficher l'interface 'dialogues'
		VD.OnNodeChange += UpdateUI;                                            // callback changement de 'noeud'
		VD.OnEnd += End;                                                        // callback 'fin de dialogue'
																				// activer le dialogue
		if (VD.isActive)
			VD.Next();
		else {
			VD.BeginDialogue(dialog);
		}
	}

	public override void Toggle() {
		panel.SetActive(!isOn);
		inventory.SetActive(!isOn);
	}
	public void Show() {
		StartCoroutine(IShow());
		container_NPC.transform.position = Camera.main.WorldToScreenPoint(pnj.transform.position);
		container_PLAYER.transform.position = Camera.main.WorldToScreenPoint(playerManager.transform.position);
	}
	IEnumerator IShow() {
		var cm = Camera.main.GetComponent<CinemachineBrain>();
		while (cm.ActiveBlend != null)
			yield return new WaitForEndOfFrame();
		panel.SetActive(true);
		uiManager.ManageButtons(dialog);

	}
	public void Hide() {
		panel.SetActive(false);
		uiManager.RestoreButtonsPreviousState();
	}


	void UpdateUI(VD.NodeData data) {
		container_NPC.SetActive(false);
		container_PLAYER.SetActive(false);


		if (data.isPlayer) {
			container_PLAYER.SetActive(true);

			// set name
			//If it has a tag, show it, otherwise let's use the alias we set in the VIDE Assign
			if (data.tag.Length > 0)
				PLAYER_Label.text = data.tag;
			else
				PLAYER_Label.text = playerManager.characterData.CharacterName;

			if (data.comments.Length == 1) {
				text_PLAYER.text = data.comments[data.commentIndex];
				text_PLAYER.gameObject.SetActive(true);
				Buttons.gameObject.SetActive(false);
			} else {
				// set choices
				for (int i = 0; i < choices.Length; i++) {
					if (i < data.comments.Length) {
						choices[i].transform.parent.gameObject.SetActive(true);
						choices[i].text = data.comments[i];
					} else {
						choices[i].transform.parent.gameObject.SetActive(false);
					}
				}
				text_PLAYER.gameObject.SetActive(false);
				Buttons.gameObject.SetActive(true);
			}
		} else {
			container_NPC.SetActive(true);
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
		// interface
		Hide();                                             // masquer l'interface dialogue
		if (pnj) {
			pnj.PNJcam?.SetActive(false);                       // désactiver la caméra 'portrait' du PNJ
			SceneModeManager.SetSceneMode(SceneMode.dialogue, false, pnj);      //container_NPC.SetActive(false);
		}

		VD.OnNodeChange -= UpdateUI;                        // supprimer callback 'changement de noeud'
		VD.OnEnd -= End;                                    // supprimer callback 'fin de dialogue'
		VD.EndDialogue();
	}
	//private void OnDisable() {
	//	if (container_NPC != null)
	//		End(null);
	//}

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
		if (VD.nodeData.comments.Length == 1)
			VD.Next();
	}

	public enum QuestStatus { None, Accepted, Refused, Done }
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
