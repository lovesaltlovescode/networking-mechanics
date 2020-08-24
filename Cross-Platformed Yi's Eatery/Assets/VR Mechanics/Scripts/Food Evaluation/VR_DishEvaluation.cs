using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VR_DishEvaluation : MonoBehaviour
{
    [SerializeField] private VR_DishFeedback DishFeedbackScript;

    [SerializeField] GameObject chicSlices_parent, rice_parent, cucumberSlices, eggHalf;
    private bool roastedChic, ricePlain, haveEgg = false;

    //GameObjects to toggle
    [Header("Toggled ingredients")]
    [SerializeField] private GameObject[] chicSlices_isRoasted = new GameObject[2];
    [SerializeField] private GameObject[] rice_isPlain = new GameObject[2];


    private ChickenRice correspondingDishDetails = null;


    private void Awake()
    {
        EmptyPlate();
    }

    public bool CheckCorrespondingOrderSlip(ChickenRice _orderSlipOrder)
    {
        if (CheckMinRequirements())
        {
            AssignDishDetails();

            return correspondingDishDetails.ChickenRiceLabel == _orderSlipOrder.ChickenRiceLabel;
        }

        return false;
    }

    private void AssignDishDetails()
    {
        correspondingDishDetails = OrderGeneration.Instance.CreateCustomOrder(roastedChic, ricePlain, haveEgg);
    }

    private bool CheckMinRequirements()
    {
        return CheckPlated_Chicken() && CheckPlated_Rice() && CheckPlated_Cucumber();
    }


    public void PlateChicken(bool isRoasted)
    {
        if (!CheckPlated_Chicken())
        {
            roastedChic = isRoasted;
            chicSlices_isRoasted[Convert.ToInt32(roastedChic)].SetActive(true);
            chicSlices_parent.SetActive(true);
        }
    }

    public void PlateRice(bool isPlain)
    {
        if (!CheckPlated_Rice())
        {
            ricePlain = isPlain;
            rice_isPlain[Convert.ToInt32(ricePlain)].SetActive(true);
            rice_parent.SetActive(true);
        }
    }

    public void PlateCucumber()
    {
        if (!CheckPlated_Cucumber())
        {
            cucumberSlices.SetActive(true);
        }
    }

    public void PlateEgg()
    {
        if (!CheckPlated_Egg())
        {
            haveEgg = true;
            eggHalf.SetActive(true);
        }
    }

    public void ServeDish()
    {
        //empty dish
        EmptyPlate();
    }

    public bool CheckPlated_Chicken()
    {
        return chicSlices_parent.activeInHierarchy;
    }

    public bool CheckPlated_Rice()
    {
        return rice_parent.activeInHierarchy;
    }

    public bool CheckPlated_Cucumber()
    {
        return cucumberSlices.activeInHierarchy;
    }

    public bool CheckPlated_Egg()
    {
        return eggHalf.activeInHierarchy;
    }


    //disables all the gameObjects on the plate
    public void EmptyPlate()
    {
        cucumberSlices.SetActive(false);
        eggHalf.SetActive(false);

        foreach (GameObject chickenType in chicSlices_isRoasted)
        {
            chickenType.SetActive(false);
        }

        foreach (GameObject riceType in rice_isPlain)
        {
            riceType.SetActive(false);
        }

        chicSlices_parent.SetActive(false);
        rice_parent.SetActive(false);
    }

}
