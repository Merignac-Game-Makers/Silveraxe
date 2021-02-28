using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowFlower : Loot {

	Object[] textureList;
	int frameCounter = 0;
	int total;
	Material mat;
	float timer = 0;

	public MeshRenderer rend;
	public float speed = 10;

	public HazelDialogueManager hazelDialogueManager;

	protected override void Start() {
		base.Start();
		textureList = Resources.LoadAll("Loots/RainbowFlower/RflowerTextures", typeof(Texture));
		total = textureList.Length;
		mat = rend.material;
		speed = 1 / speed;
	}

	protected override void Update() {
		base.Update();
		timer += Time.deltaTime;
		if (timer > speed) {
			frameCounter = (frameCounter + total - 1) % total;
			mat.SetTexture("_EmissionMap", (Texture)textureList[frameCounter]);
			timer = 0;
		}
	}

	protected override void OnTake() {
		base.OnTake();
	}

	protected override void OnDrop(Target target) {
		base.OnDrop(target);
		hazelDialogueManager.SetStartNode();
		isHighlightable = false;
	}
}
