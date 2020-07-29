using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void CustomerSpawn();
    public static event CustomerSpawn OnCustomerSpawn;

    public static void CallSpawnEvent()
    {
        if (OnCustomerSpawn != null)
        {
            Debug.Log("Customer has spawned, OnSpawn event called");
            OnCustomerSpawn();
        }
      
    }


    public delegate void TableOccupied();
    public static event TableOccupied CustomersSeated;

    public static void CallCustomersSeatedEvent()
    {
        if (CustomersSeated != null)
        {
            Debug.Log("Table is occupied, GuestsSeated event called");
            CustomersSeated();
        }

    }


    public delegate void CustomerHeld();
    public static event CustomerHeld CustomersPickedUp;

    public static void CallCustomersPickedUpEvent()
    {
        if (CustomersPickedUp != null)
        {
            Debug.Log("Table is occupied, GuestsSeated event called");
            CustomersPickedUp();
        }

    }

}
