using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_VROrderManagement : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Press '0' to send a random order to the VR player (AddOrderToList() in VR_OrderManagement), " +
            "\n '1' to spawn the first order in the currentlyDisplayedOrders list on the counter (CheckCanServeDish() in VR_OrderManagement), " +
            "\n '2' to see the orders in currentlyDisplayedOrders list (in VR_OrderManagement), " +
            "\n '3' to see the orders in hiddenOrderCount list (in VR_OrderManagement)");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            VR_OrderManagement.Instance.AddOrderToList(Random.value > 0.5f, Random.value > 0.5f, Random.value > 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            VR_OrderManagement.Instance.CheckCanServeDish(null, VR_OrderManagement.Instance.CurrentlyDisplayedOrders[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("currentlyDisplayedOrders list (in VR_OrderManagement): " + VR_OrderManagement.Instance.CurrentlyDisplayedOrders);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("hiddenOrderCount list (in VR_OrderManagement): " + VR_OrderManagement.Instance.HiddenOrders);
        }
    }
}
