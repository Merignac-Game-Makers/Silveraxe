
using UnityEngine;

public abstract class DialogueDispatcher : MonoBehaviour
{
	public virtual bool HasDialogue() {
		return true;
	}

	public virtual void SetStartNode() {}
}
