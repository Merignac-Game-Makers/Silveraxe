using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSet : MonoBehaviour
{
    public GameObject sword;
    public GameObject shield;
    public GameObject helmet;
    public GameObject armor;

    void Awake()
    {
        if (sword)
            sword.SetActive(false);
        if (shield)
            shield.SetActive(false);
        if (helmet)
            helmet.SetActive(false);
        if (armor)
            armor.SetActive(false);
    }

    public void Equip(GameObject item, GameObject holder) {
        for (int i = 0; i< holder.transform.childCount; i++) {
            Destroy(holder.transform.GetChild(0).gameObject);
		}
        var newItem = Instantiate(item, holder.transform, false);
        newItem.SetActive(true);
	}
}
