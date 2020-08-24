using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_DishBehaviour : MonoBehaviour
{
    public bool isDebug = false;

    [SerializeField] private string PlateableTag = "Plateable";

    [SerializeField] private Collider dishBehaviourTrigger;
    [SerializeField] private VR_DishEvaluation DishEvaluationScript;
    [SerializeField] private VR_DishFeedback DishFeedbackScript;

    [SerializeField] private GameObject ServeDishTrigger, PlateIngredientTrigger;
    private GameObject plateOccupiedBy;

    private void Awake()
    {
        ToggleAllTriggers(true);
        ToggleServingTrigger(false);
        TogglePlatingTrigger(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("on trigger enter: " + other.gameObject.name);
        if (other.gameObject.CompareTag(PlateableTag) && plateOccupiedBy == null)
        {
            VR_OrderSlipBehaviour orderSlipBehaviour = other.gameObject.GetComponent<VR_OrderSlipBehaviour>();
            VR_IngredientProperties ingredientProperties = other.gameObject.GetComponent<VR_IngredientProperties>();

            if(orderSlipBehaviour != null)
            {
                plateOccupiedBy = other.gameObject;

                if (DishEvaluationScript.CheckCorrespondingOrderSlip(orderSlipBehaviour.OrderSlipOrder) || isDebug)
                {
                    ToggleServingTrigger(true);
                    DishFeedbackScript.CorrectDish();
                }
            }
            else if(ingredientProperties != null)
            {
                plateOccupiedBy = other.gameObject;

                PlateIngredientTrigger.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == plateOccupiedBy)
        {
            VR_OrderSlipBehaviour orderSlipBehaviour = other.gameObject.GetComponent<VR_OrderSlipBehaviour>();
            VR_IngredientProperties ingredientProperties = other.gameObject.GetComponent<VR_IngredientProperties>();

            if (orderSlipBehaviour != null)
            {
                ToggleServingTrigger(false);
            }
            else if (ingredientProperties != null)
            {
                TogglePlatingTrigger(false);
            }

            plateOccupiedBy = null;
        }
    }


    public void ClearDishAfterServing()
    {
        //disable the plate trigger
        ToggleAllTriggers(false);

        Debug.Log("animate the plate disappearing");

        //empty the plate
        DishEvaluationScript.EmptyPlate();

        plateOccupiedBy = null;

    }

    private void ToggleAllTriggers(bool enable, bool toggleServing = true, bool togglePlating = true)
    {
        dishBehaviourTrigger.enabled = enable;

        if (toggleServing)
            ToggleServingTrigger(enable);

        if (togglePlating)
            TogglePlatingTrigger(enable);
    }

    private void ToggleServingTrigger(bool enable)
    {
        ServeDishTrigger.SetActive(enable);

        if(!enable)
            DishFeedbackScript.DisableEffects();
    }

    private void TogglePlatingTrigger(bool enable)
    {
        PlateIngredientTrigger.SetActive(enable);

        if (!enable)
            DishFeedbackScript.DisableEffects();
    }
}
