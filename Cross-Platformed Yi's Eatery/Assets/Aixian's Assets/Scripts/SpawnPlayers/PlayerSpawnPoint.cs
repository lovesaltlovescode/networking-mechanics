using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Summary

//Adds the spawn points to the list of spawn points, and removes when it is destroyed
//Draws a gizmo to show us where the spawn point will be facing, so we can adjust accordingly
//Only seen in the scene view

#endregion
public class PlayerSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        PlayerSpawnSystem.AddSpawnPoint(transform);   //add to the list
    }

    private void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoint(transform); //remove from list
    }

    //Draw a gizmo that shows where the spawn point will be facing, only seen in scene view
    private void OnDrawGizmos()
    {
        //Draw a sphere to show position
        Gizmos.color = Color.blue;

        //drawing blue sphere at position of this object with a radius of 1
        Gizmos.DrawSphere(transform.position, 1f);

        //Draw a line to show rotation
        //shows direction we are facing
        Gizmos.color = Color.green;

        //Draw a green line starting from object position, to forward 2 meters
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2); 
       
    }
}
