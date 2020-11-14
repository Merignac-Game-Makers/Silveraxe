using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Camera minimapCamera;

    public GameObject smallView;
    public RenderTexture smallMap;
    public int smallMapSize = 10;

    public GameObject largeView;
    public RenderTexture largeMap;
    public int largeMapSize = 150;

    bool isExpanded = false;

    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        minimapCamera.transform.position = new Vector3(player.position.x, 100, player.position.z);
    }

    public void Toggle() {
        isExpanded = !isExpanded;
        if (isExpanded) {
            minimapCamera.targetTexture = largeMap;
            minimapCamera.orthographicSize = largeMapSize;
		} else {
            minimapCamera.targetTexture = smallMap;
            minimapCamera.orthographicSize = smallMapSize;
        }
        smallView.SetActive(!isExpanded);
        largeView.SetActive(isExpanded);
    }
}
