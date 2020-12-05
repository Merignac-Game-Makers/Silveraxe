using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horloge : MonoBehaviour
{
    public Animator skyAnimator;
    public Transform horloge;

    Vector3 rotation = Vector3.zero;
    public float horlogeValue;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation.z = -horlogeValue;
        horloge.rotation = Quaternion.Euler(rotation);
    }
}
