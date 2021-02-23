using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetAction : MonoBehaviour
{

    public GameObject aim;              // l'objet sur lequel va s'exercer l'action
    public bool consumeLoot = true;     // le déclencheur est il détruit par l'action ?

    public abstract void Act(Loot loot);// Action
}
