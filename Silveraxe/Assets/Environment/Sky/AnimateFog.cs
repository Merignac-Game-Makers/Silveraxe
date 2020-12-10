using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateFog : MonoBehaviour
{

    public Light sunLight;

    public float fogIntensity;

    float sunHeight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.fogDensity = fogIntensity;
    }
}
