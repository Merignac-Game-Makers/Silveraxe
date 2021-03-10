using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateFog : MonoBehaviour
{

    public Light sunLight;
    public Renderer water;
    public Color waterDayColor;
    public Color waterNightColor;

    Material mat;
    Color baseColor;
    Color reflectionColor;

    [HideInInspector]
    public float fogIntensity;
    [HideInInspector]
    public float waterState;

	private void Start() {
        mat = water.material;
        baseColor = mat.GetColor("_BaseColor");
        reflectionColor = mat.GetColor("_ReflectionColor");
    }

    void Update()
    {
        RenderSettings.fogDensity = fogIntensity;
        baseColor.a = .5f + waterState * .5f;
        mat.SetColor("_BaseColor", baseColor);
        reflectionColor = waterDayColor * waterState + waterNightColor * (1-waterState);
        mat.SetColor("_ReflectionColor", reflectionColor);
    }
}
