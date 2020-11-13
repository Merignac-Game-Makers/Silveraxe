using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamZone : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera vCam;

    GameObject player;
    CamZones camZones;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.Instance.gameObject;
        camZones = GetComponentInParent<CamZones>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnTriggerEnter(Collider other) {
		if (other.gameObject == player) {
            camZones.SwapCams(this);
		}
	}
}
