using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Find the object closest to the player and return it

public class ObjectRadar : MonoBehaviour
{
    public int radarRadius;
    public string detectableObjectTag;
    [SerializeField] private Collider[] objectColliders;

    



    public void CheckForVisibleObjects()
    {

        Debug.Log("Checking for visible objects");

        //return an array with all objects touching or inside the sphere, starting from player position
        objectColliders = Physics.OverlapSphere(transform.position, this.radarRadius);

        //loop through all the colliders
        for (int index = 0; index <= objectColliders.Length - 1; index++)
        {
            //collider object is the current object collider being found
            GameObject colliderObject = objectColliders[index].gameObject;

            //if the collider object has the same tag  as what is considered 'visible'/what we want the player to look for
            if (colliderObject.tag == this.detectableObjectTag)
            {

                //casts a ray from player's position in the colliderobject's position, and return the hit object info, only detect this layer
                bool hitOccurred = Physics.Raycast(this.transform.position, colliderObject.transform.position.normalized, out RaycastHit hit, 1 << 10);
                Debug.Log(hitOccurred);

                //if there was a hit and the gameobject's tag is what we are looking for
                if (hitOccurred && hit.collider.gameObject.tag == this.detectableObjectTag)
                {
                    // Do whatever you need to do with the resulting information
                    // here if the condition succeeds.
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                }
            }
        }
    }

    //Draw a gizmo that shows where the player is facing, only in scene view
    private void OnDrawGizmos()
    {
        //Draw a sphere to show position
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        //drawing blue sphere at position of this object with a radius
        Gizmos.DrawSphere(transform.position, radarRadius);

        //Draw a line to show rotation
        //shows direction we are facing
        Gizmos.color = Color.green;

        //Draw a green line starting from player position to where the radius of the overlap sphere ends
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * radarRadius);

    }

    public void Update()
    {
        CheckForVisibleObjects();

    }
}
