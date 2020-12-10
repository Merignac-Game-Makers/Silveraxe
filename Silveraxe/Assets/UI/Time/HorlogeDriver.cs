using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorlogeDriver : MonoBehaviour
{
    public Horloge horloge;
    public float horlogeValue;


    // Update is called once per frame
    void Update()
    {
        horloge.horlogeValue = horlogeValue;
    }
}
