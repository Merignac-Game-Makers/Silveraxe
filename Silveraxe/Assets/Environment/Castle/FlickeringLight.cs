using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public float MaxReduction;
    public float MaxIncrease;
    public float RateDamping;
    public float Strength;
    public float radius;
    public bool StopFlickering;

    private Light _lightSource;
    private float _baseIntensity;
    private bool _flickering;
    private Vector3 _pos;

    public void Reset()
    {
        MaxReduction = 0.2f;
        MaxIncrease = 0.2f;
        RateDamping = 0.05f;
        radius = .01f;
        Strength = 300;
    }

    public void Start()
    {
        _pos = transform.position;
        _lightSource = GetComponent<Light>();
        if (_lightSource == null)
        {
            Debug.LogError("Flicker script must have a Light Component on the same GameObject.");
            return;
        }
        _baseIntensity = _lightSource.intensity;
        StartCoroutine(DoFlicker());
    }

    void Update()
    {
        if (!StopFlickering && !_flickering)
        {
            StartCoroutine(DoFlicker());
        }
    }

    private IEnumerator DoFlicker()
    {
        _flickering = true;
        while (!StopFlickering)
        {
            _lightSource.intensity = Mathf.Lerp(_lightSource.intensity, Random.Range(_baseIntensity - MaxReduction, _baseIntensity + MaxIncrease), Strength * Time.deltaTime);
            transform.position = _pos + new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
            yield return new WaitForSeconds(RateDamping);
        }
        _flickering = false;
    }
}
