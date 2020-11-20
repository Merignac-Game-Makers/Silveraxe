using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CombineUI : App
{
    public Transform objectHolder;
    [Range(100f, 350f)] public float size = 250;

    public Loot item { get; private set; }
    public InventoryEntry entry { get; private set; }

    private void Start() {
        //GetComponentInChildren<RawImage>().SizeToParent();
    }

    public void SetObject(InventoryEntry entry) {
        Clear();
        this.entry = entry;
        item = entry.item;
        GameObject obj = Instantiate(item.prefab, objectHolder, false);
        item.animate = false;
        SetObjLayer(obj);
        obj.transform.localPosition = Vector3.zero;
        objectHolder.GetComponent<RotateObject>().objectTransform = obj.transform;
        gameObject.SetActive(true);                                                 // afficher le panneau
        StartCoroutine(IScale(obj));                                                // taille
    }

    public void Clear() {
        item = null;
        for(int i= objectHolder.childCount-1; i>=0; i--) {
            Destroy(objectHolder.GetChild(i).gameObject);
        }
        gameObject.SetActive(false);                                                // masquer panneau
    }

    void SetObjLayer(GameObject obj) {
        var layer = LayerMask.NameToLayer("Interactable");                          // layer des objets intéractibles
        obj.layer = layer;
        for (int i = 0; i < obj.transform.childCount; i++)
            obj.transform.GetChild(i).gameObject.layer = layer;
    }

    IEnumerator IScale(GameObject obj) {
        yield return new WaitForEndOfFrame();
        Vector2 referenceResolution = uiManager.GetComponent<CanvasScaler>().referenceResolution;
        Vector2 currentResolution = new Vector2(Screen.width, Screen.height);
        float heightRatio = currentResolution.y / referenceResolution.y;
        var colliders = obj.GetComponentsInChildren<Collider>();
        float extents = 0;
        foreach (Collider collider in colliders) {
            extents = Mathf.Max(extents, collider.bounds.extents.magnitude);
        }

        obj.transform.localScale = Vector3.one / extents * size * heightRatio;
        //obj.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void Hide() {
        //gameObject.SetActive(false);
        inventoryUI.selectedEntry.Toggle();
    }
}
