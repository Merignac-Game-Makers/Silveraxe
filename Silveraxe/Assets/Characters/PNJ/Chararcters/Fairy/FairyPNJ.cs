using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FairyPNJ : PNJ {
	public PNJ hazel;
	public LakeTemple lakeTemple;
	public Animator playerAnimator;

	Animator animator;

	protected override void Start() {
		base.Start();
		animator = GetComponentInChildren<Animator>();
	}

	public void EnterDialogue() {
		animator.SetTrigger("Enter");
	}

	public void ExitDialogue() {
		animator.SetTrigger("Exit");
	}

	public void EndDialogue() {
		hazel.gameObject.SetActive(true);
		lakeTemple.EnableFairy(false);
		ExitDialogue();
	}

	public void PlayerDismiss() {
		playerAnimator.SetTrigger("Dismiss");
	}
}
