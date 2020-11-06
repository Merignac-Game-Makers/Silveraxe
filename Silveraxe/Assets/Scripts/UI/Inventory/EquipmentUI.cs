using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    /// <summary>
    /// Keep reference and update the Equipment entry (MagicBook + 2 artefacts)
    /// </summary>
    public class EquipmentUI : MonoBehaviour
    {
        public ItemEntryUI bookSlot;
        public ItemEntryUI sunSlot;
        public ItemEntryUI moonSlot;
        
        public void Init(InventoryUI owner)
        {
            bookSlot.inventoryUI = owner;
            sunSlot.inventoryUI = owner;
            moonSlot.inventoryUI = owner;
        }

        //public void UpdateEquipment(EquipmentSystem equipment, StatSystem system)
        //{
        //    var book = equipment.GetItem(EquipmentItem.EquipmentSlot.Book);
        //    var sun = equipment.GetItem(EquipmentItem.EquipmentSlot.Sun);
        //    var moon = equipment.GetItem(EquipmentItem.EquipmentSlot.Moon);

        //    bookSlot.SetupEquipment(book);
        //    sunSlot.SetupEquipment(sun);
        //    moonSlot.SetupEquipment(moon);
        //}
    }
