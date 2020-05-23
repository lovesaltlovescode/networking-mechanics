using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Get animator component and set bools for anim to play

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();   
    }

    //function to move player
    public void Move(float blend)
    {
        anim.SetFloat("Blend", blend);
    }

    //function make player happy
    public void Jump(bool jump)
    {
        anim.SetBool("hhh", jump);
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(anim.GetFloat("Blend"));
    }
}
