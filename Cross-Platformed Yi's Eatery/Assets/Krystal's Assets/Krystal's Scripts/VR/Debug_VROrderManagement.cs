using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Debug_VROrderManagement : NetworkBehaviour
{
    private void Start()
    {
        Debug.Log("Press '0' to send a random order to the VR player (AddOrderToList() in VR_OrderManagement), " +
            "\n 'a' to spawn the first order in the currentlyDisplayedOrders list on the counter (CheckCanServeDish() in VR_OrderManagement), " +
            "\n 'b' to see the orders in currentlyDisplayedOrders list (in VR_OrderManagement), " +
            "\n 'c' to see the orders in hiddenOrderCount list (in VR_OrderManagement)");
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //VR_OrderManagement.Instance.AddOrderToList(Random.value > 0.5f, Random.value > 0.5f, Random.value > 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            MobileVR_OrderManagement.Instance.CheckCanServeDish(null, MobileVR_OrderManagement.Instance.CurrentlyDisplayedOrders[0]);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("currentlyDisplayedOrders list (in VR_OrderManagement): " + MobileVR_OrderManagement.Instance.CurrentlyDisplayedOrders);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("hiddenOrderCount list (in VR_OrderManagement): " + MobileVR_OrderManagement.Instance.HiddenOrders);
        }
    }

    public void CheckServeDish()
    {
        MobileVR_OrderManagement.Instance.CheckCanServeDish(null, MobileVR_OrderManagement.Instance.CurrentlyDisplayedOrders[0]);
    }
}
