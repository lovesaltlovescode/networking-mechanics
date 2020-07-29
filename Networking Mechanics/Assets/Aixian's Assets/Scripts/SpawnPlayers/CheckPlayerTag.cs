using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

#region Summary

//Attached to player object in game scene
//Player will have their own tag based on the order they were spawned in
//Players will enable the models according to their tag
//Check for tag name 
//if it matches, then display that model

#endregion
public class CheckPlayerTag : NetworkBehaviour
{

    //array of possible models to display
    //assign in the inspector
    public GameObject[] characterModels = new GameObject[5];

    private void Start()
    {
        StartCoroutine(CheckPlayerTags());
    }

    //coroutine to check tag of player
    private IEnumerator CheckPlayerTags()
    {
        Debug.LogWarning("Coroutine still running"); //Should stop sometime, when game object tag is no longer untagged
        WaitForSeconds wait = new WaitForSeconds(0.01f); //wait for s before starting again
        while(true)
        {
            Debug.Log("Gameobject tag is " + gameObject.tag);
            CheckTag();


            if (gameObject.tag != "Untagged")
            {
                Debug.Log("Gameobjects tag is now: " + gameObject.tag);
                break;
            }
                yield return wait;
        }
       // Debug.Log("Stopped checking");
    }





    void CheckTag()
    {
        switch(gameObject.tag)
        {
            case "XiaoBen":
                Debug.Log("This player is Xiao Ben!");
                characterModels[0].SetActive(true);
                break;

            case "DaFan":
                Debug.Log("This player is Da Fan!");
                characterModels[1].SetActive(true);
                break;

            case "DaLi":
                Debug.Log("This player is Da Li!");
                characterModels[2].SetActive(true);
                break;

            case "XiaoFan":
                Debug.Log("This player is Xiao Dan!");
                characterModels[3].SetActive(true);
                break;

            case "XiaoLi":
                Debug.Log("This player is Xiao Li!");
                characterModels[4].SetActive(true);
                break;
        }
    }

}
