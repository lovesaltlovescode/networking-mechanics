using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionScript : MonoBehaviour
{

    int ignoreMasks = 1 << 8 | 1 << 9;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InteractRaycast();
    }

    void InteractRaycast()
    {
        //get player position
        Vector3 playerPos = transform.position;

        //direction where the ray is being cast
        Vector3 forwardDir = transform.forward; //straight ahead from the camera

        //
        Ray interactionRay = new Ray(playerPos, forwardDir);
        RaycastHit interactionRayHit;

        float interactionRayLength = 5f;
        Vector3 interactionRayEndpoint = (forwardDir * interactionRayLength);
        Debug.DrawLine(playerPos, interactionRayEndpoint);

        //Only hit the ignoremaks, so i must reverse
        bool hitFound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength, ~ignoreMasks);
        if (hitFound)
        {
            GameObject hitGameobject = interactionRayHit.transform.gameObject;
            string hitFeedback = hitGameobject.name;
            Debug.Log("Hit: " + hitFeedback);
        }
        else
        {
            string nothingHitFeedback = "";
            Debug.Log("Hit: " + nothingHitFeedback);
        }

    }
}
