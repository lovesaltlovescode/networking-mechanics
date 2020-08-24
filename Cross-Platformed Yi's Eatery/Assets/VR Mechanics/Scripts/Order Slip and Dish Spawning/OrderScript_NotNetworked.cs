using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderScript_NotNetworked : MonoBehaviour
{
    [SerializeField] private ChickenRice.PossibleChickenRiceLabel dishLabel;
    [SerializeField] private GameObject orderIcon;
    private bool isDishCold = false;

    #region Getters and Setters
    public ChickenRice.PossibleChickenRiceLabel DishLabel
    {
        get { return dishLabel; }
        private set { dishLabel = value; }
    }
    public GameObject OrderIcon
    {
        get { return orderIcon; }
        private set { orderIcon = value; }
    }

    public bool IsDishCold
    {
        get { return isDishCold; }
    }
    #endregion


    #region Specific properties of dish
    private bool roastedChic, ricePlain, haveEgg;

    #region Getters and Setters
    public bool RoastedChic
    {
        get { return roastedChic; }
        private set { roastedChic = value; }
    }
    public bool RicePlain
    {
        get { return ricePlain; }
        private set { ricePlain = value; }
    }
    public bool HaveEgg
    {
        get { return haveEgg; }
        private set { haveEgg = value; }
    }

    #endregion
    #endregion

    private void Awake()
    {
        ToggleIcon(true);

        #region Set bools
        /*
        //set the properties based on the dish label
        switch (dishLabel)
        {
            #region Roasted chic
            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRice:
                RoastedChic = true;
                RicePlain = true;
                HaveEgg = false;
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWPlainRiceEgg:
                RoastedChic = true;
                RicePlain = true;
                HaveEgg = true;
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBall:
                RoastedChic = true;
                RicePlain = false;
                HaveEgg = false;
                break;

            case ChickenRice.PossibleChickenRiceLabel.RoastedChicWRiceBallEgg:
                RoastedChic = true;
                RicePlain = false;
                HaveEgg = true;
                break;
            #endregion

            #region Steamed chic
            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRice:
                RoastedChic = false;
                RicePlain = true;
                HaveEgg = false;
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWPlainRiceEgg:
                RoastedChic = false;
                RicePlain = true;
                HaveEgg = true;
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBall:
                RoastedChic = false;
                RicePlain = false;
                HaveEgg = false;
                break;

            case ChickenRice.PossibleChickenRiceLabel.SteamedChicWRiceBallEgg:
                RoastedChic = false;
                RicePlain = false;
                HaveEgg = true;
                break;
            #endregion
        }
        */
        #endregion
    }

    public void ToggleIcon(bool _enableIcon)
    {
        orderIcon.SetActive(_enableIcon);
    }


}
