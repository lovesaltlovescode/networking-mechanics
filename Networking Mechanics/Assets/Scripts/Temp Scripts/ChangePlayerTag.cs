using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChangePlayerTag : NetworkBehaviour
{

    // Start is called before the first frame update

    private void Update()
    {
        if(PlayerSpawnSystem.nextIndex == 1)
        {
            gameObject.name = "XiaoBen";
        }
        else if(PlayerSpawnSystem.nextIndex == 2)
        {
            gameObject.name = "DaFan";
        }
        else if(PlayerSpawnSystem.nextIndex == 3)
        {
            gameObject.name = "DaLi";
        }
        else if(PlayerSpawnSystem.nextIndex == 4)
        {
            gameObject.name = "XiaoFan";
        }
        else if(PlayerSpawnSystem.nextIndex == 5)
        {
            gameObject.name = "XiaoLi";
        }
    }

}
