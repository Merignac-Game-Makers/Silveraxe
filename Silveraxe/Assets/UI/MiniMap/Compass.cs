using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public GameObject compassArrow;

    Quaternion arrowRotation;
    Transform player;
    RectTransform rt;

    void Start()
    {
        player = App.playerManager.transform;
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        rt.sizeDelta = new Vector2(Screen.width / 10f, Screen.width / 10f);

        // orientation de l'aiguille
        arrowRotation = Quaternion.Euler(Vector3.Cross(player.rotation.eulerAngles, Vector3.right));
        compassArrow.transform.localRotation = arrowRotation;
    }

    public void Show(bool on) {
        gameObject.SetActive(on);
	}
}
