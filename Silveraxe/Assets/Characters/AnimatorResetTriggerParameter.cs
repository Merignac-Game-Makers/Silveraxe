using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorResetTriggerParameter : StateMachineBehaviour
{
	[SerializeField]
	private string triggerName = null;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.ResetTrigger(triggerName);
	}
}
