using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to player
//Finds what object they are currently looking at and returns its name/tag
//will ignore the ground/environment and the player capsule
public class PlayerRadar : MonoBehaviour
{

    public float objectRadar; //How far the ray must be casted

    //shift layer bits to 8 and 9 (player and environment)
    private int ignoreLayers = 1 << 8 | 1 << 9;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectObject();
    }

    public void DetectObject()
    {
        RaycastHit hit;

        //Raycast from object origin forward and ignore objects on layer masks
        bool hitObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, objectRadar, ~ignoreLayers);

        //if an object was hit
        if (hitObject)
        {
            //draw a yellow ray from object position (origin) forward to the distance of the cast (objectRadar)
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.yellow);
            //Return hit object tag
            Debug.Log("Hit " + hit.collider.tag);
        }
        else
        {
            //draw a white ray
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * objectRadar, Color.white);
            Debug.Log("Did not hit anything");
        }

    }

   
}
