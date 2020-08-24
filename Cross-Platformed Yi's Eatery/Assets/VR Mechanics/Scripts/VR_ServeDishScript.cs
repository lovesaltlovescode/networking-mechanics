using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ServeDishScript : MonoBehaviour
{

    [SerializeField] private string PlateableTag = "Plateable";

    [SerializeField] private VR_DishEvaluation DishEvaluationScript;
    [SerializeField] private VR_DishBehaviour DishBehaviourScript;
    [SerializeField] private VR_DishFeedback DishFeedbackScript;
    [SerializeField] private VR_OrderSlipManager OrderSlipManagerScript;

    private int indexNum = 0;
    private void OnTriggerEnter(Collider other)
    {
        VR_OrderSlipBehaviour orderSlipBehaviour = other.gameObject.GetComponent<VR_OrderSlipBehaviour>();

        if (orderSlipBehaviour != null)
        {
            if (CheckCounterHasSpace())
            {
                CounterFeedback(CheckCounterHasSpace());

                // if there is space on the counter, remove the order slip and spawn the dish
                if (indexNum > -1)
                {
                    //SpawnDishOnCounter_NotNetworked.Instance.SpawnDish(indexNum, orderSlipBehaviour.OrderSlipOrder.RoastedChic, orderSlipBehaviour.OrderSlipOrder.RicePlain, orderSlipBehaviour.OrderSlipOrder.HaveEgg);
                    SpawnDishOnCounter.Instance.ServerSpawnDish(indexNum, orderSlipBehaviour.OrderSlipOrder.RoastedChic, orderSlipBehaviour.OrderSlipOrder.RicePlain, orderSlipBehaviour.OrderSlipOrder.HaveEgg);

                    OrderSlipManagerScript.RemoveOrderSlip(orderSlipBehaviour);

                    DishBehaviourScript.ClearDishAfterServing();
                }
                else //if the serve counter is full, give feedback
                {
                    Debug.Log("Service counter is too full to spawn dish");
                }
            }
            else
            {

            }
        }

    }


    private bool CheckCounterHasSpace()
    {
        indexNum = SpawnDishOnCounter_NotNetworked.Instance.CheckCounterHasSpace(); //returns -1 if there is no space

        return indexNum > -1;
    }

    private void CounterFeedback(bool isPositive)
    {
        if (isPositive)
        {
            Debug.Log("There's space on the counter");
        }
        else
        {
            Debug.Log("There is no space on the counter");
        }
    }
}
