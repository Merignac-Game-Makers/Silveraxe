using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorlogeDriver : MonoBehaviour
{
    public Horloge horloge;
    public float horlogeValue;      // mis à jour par l'animateur "Astres"


    // Update is called once per frame
    void Update()
    {
        horloge.horlogeValue = horlogeValue;
    }
}
