using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Script that checks and handles the switching of held items of the player
/// </summary>
/// 

////list of possible items that can be held
//public enum HeldItems //defaults to int
//{
//    nothing,
//    cucumber,
//    egg,
//    chicken
//}

//public class PlayerHoldItem : NetworkBehaviour
//{

//    //scene object prefab to contain the dropped items, as it has a netID
//    public GameObject sceneObjectPrefab;

//    //player attachment point
//    public GameObject attachmentPoint;
//    //player drop point, where items should be dropped
//    public GameObject dropPoint;

//    //PREFABS to be spawned
//    public GameObject cucumberPrefab;
//    public GameObject eggPrefab;
//    public GameObject chickenPrefab;

//    //when the helditem changes, call onchangeitem method
//    [SyncVar(hook = nameof(OnChangeItem))]
//    public HeldItems heldItem;

//    //method to start coroutine everytime helditem is changed
//    void OnChangeItem(HeldItem oldHeldItem, HeldItem newHeldItem)
//    {
//        StartCoroutine(ChangeItem(newHeldItem));
//    }

//    //coroutine to change the held item
//    IEnumerator ChangeItem(HeldItem newHeldItem)
//    {
//        //destroy existing held item
//        while(attachmentPoint.transform.childCount > 0)
//        {
//            Destroy(attachmentPoint.transform.GetChild(0).gameObject);
//            yield return null;
//        }

//        //depending on which held item is being held by player (in update)
//        //instantiate the corresponding prefab
//        switch (newHeldItem)
//        {
//            case HeldItem.cucumber:
//                Instantiate(cucumberPrefab, attachmentPoint.transform);
//                break;

//            case HeldItem.egg:
//                Instantiate(eggPrefab, attachmentPoint.transform);
//                break;

//            case HeldItem.chicken:
//                Instantiate(chickenPrefab, attachmentPoint.transform);
//                break;

//        }
//    }


//    // Update is called once per frame
//    void Update()
//    {
//        //if player does not have authority, do not carry out anything
//        if (!hasAuthority)
//        {
//            return;
//        }

//        //depending on which key is pressed
//        //change the held item
//        if (Input.GetKeyDown(KeyCode.Alpha0) && heldItem != HeldItems.nothing)
//        {
//            CmdChangeHeldItem(HeldItems.nothing);
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha1) && heldItem != HeldItems.cucumber)
//        {
//            CmdChangeHeldItem(HeldItems.cucumber);
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha2) && heldItem != HeldItems.egg)
//        {
//            CmdChangeHeldItem(HeldItems.egg);
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha3) && heldItem != HeldItems.chicken)
//        {
//            CmdChangeHeldItem(HeldItems.chicken);
//        }

//        //Drop item if player is currently holding something
//        if(Input.GetKeyDown(KeyCode.X) && heldItem != HeldItems.nothing)
//        {
//            CmdDropitem();
//        }
//    }


//    //sends a command from client to server to change the helditem to the selected item via update
//    [Command]
//    void CmdChangeHeldItem(HeldItems selectedItem)
//    {
//        heldItem = selectedItem;
//    }

//    //sends a command from client to server to drop the held item in the scene
//    [Command]
//    void CmdDropitem()
//    {
//        //instantiate scene object on the server at the drop point
//        Vector3 pos = dropPoint.transform.position;
//        Quaternion rot = dropPoint.transform.rotation;
//        GameObject newSceneObject = Instantiate(sceneObjectPrefab, pos, rot);

//        //set Rigidbody as non-kinematic on SERVER only (isKinematic = true in prefab)
//        newSceneObject.GetComponent<Rigidbody>().isKinematic = false;

//        //get sceneobject script from the sceneobject prefab
//        SceneObject sceneObject = newSceneObject.GetComponent<SceneObject>();

//        //set child object of the scene object from server
//        sceneObject.SetHeldItem(heldItem);

//        //sync var the helditem in scene object to the helditem in the player
//        sceneObject.heldItem = heldItem;

//        //set player's sync var to nothing so clients
//        heldItem = HeldItems.nothing;

//        //spawn the scene object on network for everyone to see
//        NetworkServer.Spawn(newSceneObject);


//    }
    
//    //cmdpickupitem to be called from sceneobject script
//    [Command]
//    public void CmdPickUpItem(GameObject sceneObject)
//    {
//        //set player's syncvar so clients can show the right held item
//        //according to which item the sceneobject currently contains
//        heldItem = sceneObject.GetComponent<SceneObject>().heldItem;

//        //destroy the scene object when it has been picked up
//        NetworkServer.Destroy(sceneObject);
//    }
//}
