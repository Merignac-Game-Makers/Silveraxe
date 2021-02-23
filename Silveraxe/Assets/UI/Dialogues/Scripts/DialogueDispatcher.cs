
using UnityEngine;

public abstract class DialogueDispatcher : MonoBehaviour
{
	protected VIDE_Assign dialogue;


	private void Start() {
		dialogue = GetComponent<VIDE_Assign>();
	}

	public virtual bool HasDialogue() {
		return true;
	}

	public virtual void SetStartNode() {
		dialogue.overrideStartNode = 0;
	}
}
