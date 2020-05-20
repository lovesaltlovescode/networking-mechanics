using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes objects that have been picked up  follow the player
//Only follow when the tag is right
//When object is dropped, revert the tag
public class FollowObject : MonoBehaviour
{
    //the original tag of the object
    public string originalTag;

    //player to follow
    public Transform playerTarget;

    float speed = 5f;
    Vector3 playerDirection;

    const float EPSILON = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        //retrive original tag
        originalTag = gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "FollowPlayer")
        {
            //function to follow player
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        //playerDirection = (playerTarget.position - transform.position).normalized; //normalized so it does not speed up as the distance changes

        ////if distance between object and player is more than epsilon
        //if((transform.position - playerTarget.position).magnitude > EPSILON)
        //{
        //    //maintain same speed
        //    transform.Translate(playerDirection * Time.deltaTime * speed);
        //}

        transform.position = playerTarget.transform.position;

        if((transform.position - playerTarget.position).magnitude < EPSILON)
        {
            //reset tag if the object is on the player
            gameObject.tag = originalTag;
        }
    }
}
