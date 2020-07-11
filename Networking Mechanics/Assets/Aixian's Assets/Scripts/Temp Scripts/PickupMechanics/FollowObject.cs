using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes objects that have been picked up stick to the player 
//Script is set active when objects have been picked up
//Disabled when object is dropped
public class FollowObject : MonoBehaviour
{

    //player to follow
    public Transform playerTarget;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    public void FollowPlayer()
    {
        //playerDirection = (playerTarget.position - transform.position).normalized; //normalized so it does not speed up as the distance changes

        ////if distance between object and player is more than epsilon
        //if((transform.position - playerTarget.position).magnitude > EPSILON)
        //{
        //    //maintain same speed
        //    transform.Translate(playerDirection * Time.deltaTime * speed);
        //}

        transform.position = new Vector3(playerTarget.transform.position.x, 0f, playerTarget.transform.position.z);
    }
}
