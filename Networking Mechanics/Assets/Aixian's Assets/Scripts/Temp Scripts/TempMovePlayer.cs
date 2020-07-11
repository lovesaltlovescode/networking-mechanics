using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TempMovePlayer : NetworkBehaviour
{

    // Start is called before the first frame update

    private void Update()
    {

        if (!hasAuthority)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            gameObject.transform.position += Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            gameObject.transform.position -= Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            gameObject.transform.position += Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            gameObject.transform.position -= Vector3.left;
        }

    }

}
