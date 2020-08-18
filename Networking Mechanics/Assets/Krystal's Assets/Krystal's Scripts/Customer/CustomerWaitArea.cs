using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomerWaitArea : NetworkBehaviour
{
    [SerializeField] private GameObject customerQueueingPrefab;
    private bool isCoroutineRunning = false;

    [Header("Tags and Layers")]
    [SerializeField] private string customerTag = "Customer";
    [SerializeField] private string detectableLayerName = "Pickuppable", undetectableLayerName = "Environment", playerLayer = "Player";

    [Header("Minor metrics")]
    [SerializeField] private float checkRad = 0.625f; //radius of the area to be checked for customers
    [SerializeField] private float numSeconds = 1; //number of seconds taken for customer to lerp from above player's head to pos in waiting area



    //toggles the layer the wait area collider is on
    public void ToggleWaitAreaDetection(bool setDetectable)
    {
        if (setDetectable)
        {
            gameObject.layer = LayerMask.NameToLayer(detectableLayerName);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer(undetectableLayerName);
        }
    }


    //checks whether the player is in the wait area
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            if (!other.GetComponent<NetworkedCustomerInteraction>().CustomerWaitAreaManager)
            {
                other.GetComponent<NetworkedCustomerInteraction>().CustomerWaitAreaManager = this;
            }

            other.gameObject.GetComponent<NetworkedCustomerInteraction>().isPlayerInWaitArea = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayer))
        {
            other.gameObject.GetComponent<NetworkedCustomerInteraction>().isPlayerInWaitArea = false;
        }
    }



    //spawn a queueing customer prefab in the waiting area
    //move the queueing customer into a random position in the wait area
    [Server]
    public void PutCustomerdown(GameObject customerBeingHeld)
    {
        //get the being held script
        CustomerBehaviour_BeingHeld _beingHeldScript = customerBeingHeld.GetComponent<CustomerBehaviour_BeingHeld>();
        //RpcTestBeingHeld(customerBeingHeld);

        Debug.Log(customerBeingHeld.name); 
        Debug.Log(_beingHeldScript); 

        //get the starting position of the customer
        Vector3 startPosition = customerBeingHeld.transform.position;

        //hide the beingHeld customer and spawn a queueing customer
        //customerBeingHeld.SetActive(false);

        GameObject returnedQueueingCustomer = Instantiate(customerQueueingPrefab, startPosition, Quaternion.identity).gameObject;

        NetworkServer.Spawn(returnedQueueingCustomer);

        //get a random position in the waiting area to put them down in
        Vector3 newPosition;

        while (true)
        {
            newPosition = GetRandomPosition();

            if (CheckPositionIsEmpty(newPosition, checkRad))
            {
                break;
            }
        }

        RpcPutCustomerDown(returnedQueueingCustomer, newPosition, customerBeingHeld);

        //pass the group size and last level of patience stored in the beingHeld customer to the new spawn
        returnedQueueingCustomer.GetComponent<CustomerBehaviour_Queueing>().CustomerResumesWaiting(_beingHeldScript.lastPatienceLevel, _beingHeldScript.groupSizeNum);


    }

    [ClientRpc]
    public void RpcTestBeingHeld(GameObject customerBeingHeld)
    {
        Debug.Log(customerBeingHeld.name); //null reference exception
    }

    [ClientRpc]
    public void RpcPutCustomerDown(GameObject returnedQueueingCustomer, Vector3 newPosition, GameObject customerBeingHeld)
    {
        //move the newly spawned customer from the player's hands to the waiting area
        StartCoroutine(LerpCustomerPos(returnedQueueingCustomer, newPosition));

        //Debug.Log(customerBeingHeld.name); //null reference exception

        //pass the group size and last level of patience stored in the beingHeld customer to the new spawn
        //returnedQueueingCustomer.GetComponent<CustomerBehaviour_Queueing>().CustomerResumesWaiting(_beingHeldScript.lastPatienceLevel, _beingHeldScript.groupSizeNum);

    }


    //lerp customer position from head to waiting area
    IEnumerator LerpCustomerPos(GameObject customerObj, Vector3 _endPos)
    {
        if (isCoroutineRunning)
        {
            //end coroutine if another coroutine is running
            yield break;
        }

        isCoroutineRunning = true;

        //get the starting pos of the customer
        Vector3 _startPos = customerObj.transform.position;

        //calculate the desired speed at which the customer should lerp to their position
        float dist = Vector3.Distance(_startPos, _endPos);
        float desiredSpeed = dist / numSeconds; //how fast the customer has to move to get to their position in numSeconds seconds

        //distance travelled by the customer
        float distTravelled = 0;

        while (distTravelled < dist)
        {
            distTravelled += desiredSpeed * Time.deltaTime;

            customerObj.transform.position = Vector3.Lerp(_startPos, _endPos, distTravelled / dist);

            yield return null;
        }

        isCoroutineRunning = false;

        yield return null;
    }

    // get a new position within the area provided
    Vector3 GetRandomPosition()
    {
        float halfWidth = (gameObject.GetComponent<Collider>().bounds.size.x) / 2;
        float halfDepth = (gameObject.GetComponent<Collider>().bounds.size.z) / 2;

        Vector3 positionOffset = gameObject.transform.position;

        Vector3 newPos = new Vector3(Random.Range(-halfWidth, halfWidth) + positionOffset.x, 0,
                                    Random.Range(-halfDepth, halfDepth) + positionOffset.z);

        return newPos;
    }

    //returns true if the coordinates passed in does not overlap with any customer colliders
    private bool CheckPositionIsEmpty(Vector3 pos, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(pos, radius);

        //if the current position is too close to other customers in the wait area, return false
        for (int j = 0; j < hitColliders.Length; j++)
        {
            if (hitColliders[j].gameObject.CompareTag(customerTag))
            {
                return false;
            }
        }

        return true;

    }
}
