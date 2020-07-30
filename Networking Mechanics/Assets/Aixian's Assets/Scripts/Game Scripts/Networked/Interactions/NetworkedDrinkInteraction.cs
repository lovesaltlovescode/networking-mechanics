using System.Collections;
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
       GameManager.Instance.cooldownImg.fillAmount = 1; //cooldown should be full 
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
        if (!GameManager.Instance.isCooldown  && GameManager.Instance.drinksCount < 2)
        {
            networkedPlayerInteraction.ChangePlayerState(PlayerState.CanSpawnDrink);
        }
        else
        {
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


        //detect fridge
        networkedPlayerInteraction.DetectObject(networkedPlayerInteraction.detectedObject, 21, DetectFridge);

        //detected drink
        networkedPlayerInteraction.PickUpObject(networkedPlayerInteraction.detectedObject, 22, networkedPlayerInteraction.IsInventoryFull(), PlayerState.CanPickUpDrink);

        //if cooldown
        if (GameManager.Instance.isCooldown)
        {
            GameManager.Instance.cooldownImg.fillAmount += 1 / GameManager.Instance.cooldown * Time.deltaTime;
            if (GameManager.Instance.cooldownImg.fillAmount >= 1)
            {
                GameManager.Instance.cooldownImg.fillAmount = 1;
                GameManager.Instance.isCooldown = false;
            }
        }
    }

    #region Remote Methods

    public void SpawnDrink()
    {
        CmdSpawnDrink();
        //spawnedDrink = true;

    }

    public void PickUpDrink()
    {
        Debug.Log("Picked up drink called");

        networkedPlayerInteraction.CmdPickUpObject(networkedPlayerInteraction.detectedObject);

        CmdPickUpDrink();
        //change held item
        networkedPlayerInteraction.CmdChangeHeldItem(HeldItem.drink);

        //change state to do something
        //TODO: Serve customers
        networkedPlayerInteraction.playerState = PlayerState.HoldingDrink;

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
                Debug.Log("Object spawned is " + objectContainer.objToSpawn);

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

    #endregion
}
