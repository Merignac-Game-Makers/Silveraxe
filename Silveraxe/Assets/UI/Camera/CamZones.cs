using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamZones : MonoBehaviour
{
    public int tailleUnitaire = 20;
    public int zonesParCôté = 5;
    public GameObject zonePrefab;


    public CamZone[] zones { get; set; }
    public CamZone playerZone { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        if (zonesParCôté % 2 == 1)  // obligatoirement impair
            zonesParCôté++;

        for (int i = -zonesParCôté/2; i<= zonesParCôté/2; i++) {
            for (int j = -zonesParCôté / 2; j <= zonesParCôté / 2; j++) {
                var zone = Instantiate(zonePrefab, transform);
                zone.transform.localScale = new Vector3(tailleUnitaire, tailleUnitaire, tailleUnitaire);
                zone.transform.localPosition = new Vector3(i * tailleUnitaire, 0, j * tailleUnitaire);
                zone.gameObject.name = "CZ" + i + j;
                var camZone = zone.GetComponent<CamZone>();
                zone.layer = gameObject.layer;

                camZone.vCam.LookAt = PlayerManager.Instance.transform;
                //camZone.vCam.m_Lens.FieldOfView = 20;
                camZone.vCam.gameObject.SetActive(false);

                if (i == 0 && j == 0)
                    playerZone = camZone;
            }
        }
        playerZone.vCam.gameObject.SetActive(true);
   }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapCams(CamZone caller) {
        playerZone.vCam.gameObject.SetActive(false);
        playerZone = caller;
        playerZone.vCam.gameObject.SetActive(true);
    }
}
