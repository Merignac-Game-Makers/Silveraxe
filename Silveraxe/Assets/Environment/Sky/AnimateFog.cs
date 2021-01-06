using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateFog : MonoBehaviour
{

    public Light sunLight;
    [HideInInspector]
    public float fogIntensity;


    // Update is called once per frame
    void Update()
    {
        RenderSettings.fogDensity = fogIntensity;
    }
}
