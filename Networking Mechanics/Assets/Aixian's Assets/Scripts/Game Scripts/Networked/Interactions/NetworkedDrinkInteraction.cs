﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// If the player is looking at a drink machine, state changes
/// Spawn drink and allow players to pick up the drink
/// </summary>
public class NetworkedDrinkInteraction : NetworkBehaviour
{
    
    private NetworkedPlayerInteraction networkedPlayerInteraction;

    

    private void Awake()
    {
        networkedPlayerInteraction = GetComponent<NetworkedPlayerInteraction>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiseFridge();
    }

    [ServerCallback]
    public void InitialiseFridge()
    {
        RpcInitialiseFridge();
    }

    [ClientRpc]
    public void RpcInitialiseFridge()
    {
        GameManager.Instance.cooldownImg.fillAmount = 1; //cooldown should be full 
        GameManager.Instance.cooldownImg.color = Color.white;
    }

    #region Methods to Detect

    //check if player has detected the drink fridge
    public void DetectFridge()
    {
        if (!hasAuthority)
        {
            return;
        }
        //check if there are too many drinks on the counter
        if (!GameManager.Instance.isCooldown  && GameManager.Instance.drinksCount < 2 && !networkedPlayerInteraction.playerInventory)
        {
            networkedPlayerInteraction.ChangePlayerState(PlayerState.CanSpawnDrink, true);
        }
        else
        {
            if (networkedPlayerInteraction.playerInventory)
            {
                return;
            }
            networkedPlayerInteraction.ChangePlayerState(PlayerState.Default);
        }
    }


    #endregion

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (LevelTimer.Instance.hasLevelEnded)
        {
            InitialiseFridge();
        }

        //detect fridge
        //networkedPlayerInteraction.DetectObjectLookingAt(networkedPlayerInteraction.detectedObject, 21, DetectFridge);

        //detected drink
        networkedPlayerInteraction.PickUpObject(networkedPlayerInteraction.detectedObject, 22, networkedPlayerInteraction.IsInventoryFull(), PlayerState.CanPickUpDrink);

        CmdStopCooldown();

    }

    [Command]
    public void CmdStopCooldown()
    {
        //if cooldown
        if (GameManager.Instance.isCooldown)
        {
            RpcStartCooldown();
        }
    }

    [ClientRpc]
    public void RpcStartCooldown()
    {
        //GameManager.Instance.cooldownImg.fillAmount += 1 / GameManager.Instance.cooldown * Time.deltaTime;
        if (!GameManager.Instance.isDrinkCoroutineRunning)
        {
            StartCoroutine("DrinkCooldown");
            GameManager.Instance.isCooldown = true;
        }
        else if (GameManager.Instance.isDrinkCoroutineRunning)
        {
            GameManager.Instance.isCooldown = false;
        }
    }

    //coroutine that updates the timer indicating how much time is left for the food before it rots
    IEnumerator DrinkCooldown()
    {
        GameManager.Instance.isDrinkCoroutineRunning = true;

        //reset fill amount and colour
        GameManager.Instance.cooldownImg.color = new Color32(63, 63, 63, 255);
        GameManager.Instance.cooldownImg.fillAmount = 1f;

        float timeLeft = GameManager.Instance.cooldown;

        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(0.2f);
            timeLeft -= 0.1f * 2f;

            //display amount of time left
            GameManager.Instance.cooldownImg.fillAmount = timeLeft / GameManager.Instance.cooldown;

        }

        GameManager.Instance.isDrinkCoroutineRunning = false;
        GameManager.Instance.cooldownImg.fillAmount = 1;
        GameManager.Instance.cooldownImg.color = Color.white;


    }


    #region Remote Methods

    public void SpawnDrink()
    {
        if (GameManager.Instance.isDrinkCoroutineRunning)
        {
            return;
        }

        CmdSpawnDrink();
        //spawnedDrink = true;

    }

    public void PickUpDrink()
    {
        
        //Debug.Log("Picked up drink called");

        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);

        CmdPickUpDrink();
        //change held item
        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.drink);

        //change state to do something
        //TODO: Serve customers
        networkedPlayerInteraction.ChangePlayerState(PlayerState.HoldingDrink);

    }

    public void ServeDrink()
    {

        if (networkedPlayerInteraction.detectedObject && networkedPlayerInteraction.detectedObject.tag == "Customer" && 
            networkedPlayerInteraction.detectedObject.layer == LayerMask.NameToLayer("Queue") ||
            networkedPlayerInteraction.detectedObject.layer == LayerMask.NameToLayer("Table"))
        {
            //Debug.Log("Serve drinks");
            CmdServeDrink(networkedPlayerInteraction.detectedObject, gameObject);

            networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.nothing);
            networkedPlayerInteraction.ChangePlayerState(PlayerState.Default, true);
        }
        else
        {
            Debug.Log("NetworkeddrinkInteraction - Unable to serve drinks");
        }

        

    }

    #endregion

    #region Commands
    [Command]
    public void CmdSpawnDrink()
    {
        if (GameManager.Instance.isCooldown)
        {
            return;
        } 
       
        for(int i = 0; i < GameManager.Instance.drinksOnCounter.Length; i++)
        {
            //only if there is nothing in that spot
            if (!GameManager.Instance.drinksOnCounter[i])
            {
                Vector3 pos = GameManager.Instance.drinkPositions[i].transform.position;
                Quaternion rot = GameManager.Instance.drinkPositions[i].transform.rotation;

                GameObject spawnedDrinkObject = Instantiate(networkedPlayerInteraction.objectContainerPrefab, pos, rot);
                GameManager.Instance.drinksOnCounter[i] = spawnedDrinkObject;

                //set Rigidbody as non-kinematic on the instantiated object only (isKinematic = true in prefab)
                spawnedDrinkObject.GetComponent<Rigidbody>().isKinematic = false;

                //get sceneobject script from the sceneobject prefab
                ObjectContainer objectContainer = spawnedDrinkObject.GetComponent<ObjectContainer>();

                //instantiate the right item as a child of the object
                objectContainer.SetObjToSpawn(HeldItem.dirtyplate);

                //sync var the helditem in object container to the helditem in the player
                objectContainer.objToSpawn = HeldItem.drink;
                //Debug.Log("Object spawned is " + objectContainer.objToSpawn);

                //change layer of the container
                spawnedDrinkObject.layer = LayerMask.NameToLayer("Drink");

                ////spawn the scene object on network for everyone to see
                NetworkServer.Spawn(spawnedDrinkObject);

                RpcSpawnDrink(spawnedDrinkObject, i);
                return;
            }

        }
    }

    [ClientRpc]
    public void RpcSpawnDrink(GameObject spawnedDrinkObject, int i)
    {

        //change layer of the container
        spawnedDrinkObject.layer = LayerMask.NameToLayer("Drink");
        GameManager.Instance.drinksOnCounter[i] = spawnedDrinkObject;
        //increase drink count
        GameManager.Instance.drinksCount += 1;

        GameManager.Instance.isCooldown = true;
        GameManager.Instance.cooldownImg.fillAmount = 0;
    }

    [Command]
    public void CmdPickUpDrink()
    {
        //Debug.Log("CmdPickUpDrink called");

        RpcPickUpDrink();
    }

    [ClientRpc]
    public void RpcPickUpDrink()
    {
        GameManager.Instance.drinksCount -= 1;

        //Debug.Log("Rpc called, reduce drinks count");
        //Debug.Log("Drinks count is: " + GameManager.Instance.drinksCount);
    }

    [Command]
    public void CmdServeDrink(GameObject detectedObject, GameObject player)
    {
            RpcServeDrink(detectedObject, player);


    }

    [ClientRpc]
    public void RpcServeDrink(GameObject detectedObject, GameObject player)
    {
        var drink = networkedPlayerInteraction.playerInventory;

        //call method to increase patience here
        detectedObject.GetComponent<CustomerPatience>().IncreasePatience(CustomerPatienceStats.drinkPatienceIncrease);
        detectedObject.GetComponent<CustomerFeedback>().PlayHappyPFX();

        Destroy(drink);

        GameObject playerInventory = player.transform.GetChild(0).GetChild(1).gameObject;

        playerInventory = null;
    }

    #endregion
}
