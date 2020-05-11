using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerTag : NetworkBehaviour
{
    public void ChangeMyTag()
    {
        if (hasAuthority && !isServer)
        {
            string n = PlayerSpawnSystem.playerTags[PlayerSpawnSystem.nextIndex].ToString();
            gameObject.tag = n;

        }
        else
        {
            return;
        }

        CmdChangedMyTag(PlayerSpawnSystem.playerTags[PlayerSpawnSystem.nextIndex].ToString());
    }

    [Command]
    public void CmdChangedMyTag(string n)
    {
        Debug.Log("Request to change the tag to: " + gameObject.tag);

        if (hasAuthority)
        {
            gameObject.tag = n;
        }
        
    }
}
