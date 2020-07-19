using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedIngredientInteractions : NetworkBehaviour
{

    public GameObject ingredient; //prefab for the object container

    //attachment and drop points
    public GameObject attachPoint;
    public GameObject dropPoint;

    //inventory
    public GameObject inventory;

    public NetworkedPlayerInteraction networkedPlayerInteraction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Calling command");
            SpawnIngredient();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Attempting to pick up");
            PickUpIngredient();
        }
    }

    void CallCommand()
    {
        CmdPrintString();
    }

    [Command]
    public void CmdPrintString()
    {
        RpcPrintString("Help me");
    }

    [ClientRpc]
    public void RpcPrintString(string text)
    {
        Debug.Log(text);
    }

    #region SpawnIngredient

    void SpawnIngredient()
    {
        CmdSpawnIngredient();
    }

    [Command]
    void CmdSpawnIngredient()
    {
        
        RpcSpawnIngredient();
    }

    [ClientRpc]
    void RpcSpawnIngredient()
    {

        Vector3 pos = dropPoint.transform.position;
        Quaternion rot = dropPoint.transform.rotation;
        GameObject spawnedIngredient = Instantiate(ingredient, pos, rot);
        //Instantiate scene object on the server at a fixed position
        //Temporarily at drop pos
        Debug.Log("Spawn Ingredient:" + spawnedIngredient);

        //Debug.Log("Spawn plates - Spawned plate");

        ////spawn the scene object on network for everyone to see
        //NetworkServer.Spawn(dirtyPlate);
    }

    #endregion

    #region PickUpIngredient

    public void PickUpIngredient()
    {
        CmdPickUpIngredient();
    }

    [Command]
    public void CmdPickUpIngredient()
    {
        RpcPickUpIngredient();
    }

    [ClientRpc]
    public void RpcPickUpIngredient()
    {
        var pickUp = networkedPlayerInteraction.detectedObject;
        pickUp.transform.parent = attachPoint.transform;
    }

    #endregion
}
