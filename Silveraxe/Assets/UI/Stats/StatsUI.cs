using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static App;

public class StatsUI : UIBase
{
	public GameObject player_Stats;
	public GameObject PNJ_Stats;

	public Slider playerSlider;
	public Slider pnjSlider;

	StatSystem player;
	StatSystem pnj;

	private void Awake() {
		statsUI = this;
	}

	void Start() {
		panel.GetComponent<RectTransform>().sizeDelta = new Vector2(-Screen.width / 2, 0);
		Hide();
	}

	void Update() {
		if (SceneModeManager.sceneMode == SceneMode.fight) {
			playerSlider.value = (float)(player.CurrentHealth) / player.baseStats.health;
			pnjSlider.value = (float)(pnj.CurrentHealth) / pnj.baseStats.health;
		}
	}

	public override void Toggle() {
		panel.SetActive(!isOn);
		//PNJ_Stats.transform.position = Camera.main.WorldToScreenPoint(playerManager.fightController.other.transform.position);
		player_Stats.transform.position = Camera.main.WorldToScreenPoint(playerManager.transform.position);
	}
	public void Show() {
		panel.SetActive(true);

		pnj = playerManager.fightController.other.characterData.Stats;
		player = playerManager.characterData.Stats;

		//PNJ_Stats.transform.position = Camera.main.WorldToScreenPoint(playerManager.fightController.other.transform.position);
		player_Stats.transform.position = Camera.main.WorldToScreenPoint(playerManager.transform.position);
	}
	public void Hide() {
		panel.SetActive(false);
	}


}
