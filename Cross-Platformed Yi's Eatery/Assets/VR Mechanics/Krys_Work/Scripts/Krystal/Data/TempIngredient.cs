using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempIngredient : MonoBehaviour
{
    /*
    void Awake()
    {
        List<Dictionary<string, object>> heatedObjData = CSVReader.Read("heatingtest");

        for (var i = 0; i < heatedObjData.Count; i++)
        {
            Debug.Log("name " + heatedObjData[i]["obj name"]);
        }

    }
    */

    public float heatingTime = 5f, burnTime = 3f; //should be private
    public float optimalSizeFraction = 0.5f;

    public bool canBeCut = true; // should be private
    public bool isColliding = false;
    public bool inCookingUtensil = false;
    public bool isBurning = false;

    public Vector3 startSize;

    public Material cookingMat, burningMat, dryBurntMat;

    public CookingProgress cooking;



    private void Start()
    {
        cooking = new CookingProgress();

        startSize = GetComponent<Collider>().bounds.size;

        Debug.Log(cooking.TimeHeated);
    }


    public Material CheckMat()
    {
        return gameObject.GetComponent<Renderer>().material;
    }
   
    
    public void SwitchState(Material mat, TimerUI newTimer)
    {
        Debug.Log("SwitchMat");
        gameObject.GetComponent<Renderer>().material = mat;

        Debug.Log("switched material");
        Debug.Log("Rando string: " + newTimer.randoString);
        newTimer.ToggleHelper(true);
    }



}
