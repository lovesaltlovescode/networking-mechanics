using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_PreparationBase : MonoBehaviour
{
    PreparationType preparationType = default;
    public enum PreparationType
    {
        //Preparation stage
        Cooking,
        Slicing,
        Moulding,
        Serving
    }

    public enum PreparationStates{

    }

    private void OnTriggerEnter(Collider other)
    {
        VR_IngredientProperties ingredientProperties = other.gameObject.GetComponent<VR_IngredientProperties>();

        if(ingredientProperties != null)
        {
            CanBeCooked(ingredientProperties);
        }
    }

    public void CanBeCooked(VR_IngredientProperties _ingredientProperties)
    {
        if (_ingredientProperties.NeedCooked) //if it needs to be cooked, 
        {

        }

    }

    public void CanBeMoulded(VR_IngredientProperties _ingredientProperties)
    {
        if(_ingredientProperties.CheckCanPerformStep(PreparationType.Moulding) >= 0)
        {
            //if moulded before or 
        }

    }

}
