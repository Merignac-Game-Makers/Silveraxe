using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetIntegerParameter : StateMachineBehaviour
{

	[SerializeField]
	private string integerVariableName;
	[SerializeField]
	private int value;


	// OnStateExit is called when a transition ends and the state 
	//machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetInteger(integerVariableName, value);
	}


}
