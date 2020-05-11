using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChangePlayerTag : NetworkBehaviour
{

    [SerializeField] private NetworkIdentity objNetId;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ChangePlayerTag script is active!");
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            RpcChangePlayerTag(gameObject, PlayerSpawnSystem.playerTags[0].ToString());
        }
        
    }

    [ClientRpc]

    //from server, retrieve the player tag and propagate them to all clients accordingly
    public void RpcChangePlayerTag(GameObject obj, string playerTag)
    {

        var playerTagScript = gameObject.GetComponent<PlayerTag>();
        playerTagScript.ChangeMyTag();

        Debug.Log("Player tag is now: " + gameObject.tag);

        

        //for(var i = 0; i < PlayerSpawnSystem.playerTags.Length; i++)
        //{
        //    playerTag = PlayerSpawnSystem.playerTags[i];

        //    if(playerTag == gameObject.tag)
        //    {
        //        Debug.Log("Game object tag is: " + gameObject.tag + "Player tag var is: " + playerTag);
        //        break;
        //    }
        //}

        //obj.tag = playerTag;
        
    }

}
