using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpoScene : MonoBehaviour
{

	public GameObject LowPoly;
	public GameObject HandPainted;
	public GameObject PBR;

	public TMP_Text resolutionLabel;
	string[] labels = { "Low poly", "Hand painted", "PBR"};

	int select = 0;

	// Start is called before the first frame update
	void Start() {
		LowPoly.SetActive(select == 0);
		HandPainted.SetActive(select == 1);
		PBR.SetActive(select == 2);

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			select = (select + 1) % 3;
			LowPoly.SetActive(select == 0);
			HandPainted.SetActive(select == 1);
			PBR.SetActive(select == 2);
			resolutionLabel.text = labels[select];
		}

	}
}
