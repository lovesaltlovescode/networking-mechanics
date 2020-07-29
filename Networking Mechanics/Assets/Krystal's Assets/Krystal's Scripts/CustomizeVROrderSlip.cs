using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomizeVROrderSlip : MonoBehaviour
{
    //Order slip icons to toggle, depending on order
    [Header("Toggled icons")]
    [SerializeField] private GameObject[] isChicRoasted = new GameObject[2];
    [SerializeField] private GameObject[] isRicePlain = new GameObject[2];
    [SerializeField] private GameObject[] includesEgg = new GameObject[2];


    public void CustomizeOrderSlip(ChickenRice order)
    {
        isChicRoasted[Convert.ToInt32(order.RoastedChic)].SetActive(true);
        isRicePlain[Convert.ToInt32(order.RicePlain)].SetActive(true);
        includesEgg[Convert.ToInt32(order.HaveEgg)].SetActive(true);
    }
}
