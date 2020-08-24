using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerAnimationManager : MonoBehaviour
{
    [SerializeField] public Animator serverAnim;

    //if not running and is holding something ie. interact button
    public void GrabAnim()
    {
        serverAnim.SetBool("isGrabbing", true);
    }

    //if holding something and running
    public void GrabRunAnim()
    {
        serverAnim.SetBool("isRunning", true);
    }


    //if not holding anything and running
    public void RunAnim()
    {
        serverAnim.SetBool("isRunning", true);
    }

    //if not running 
    public void StopRunAnim()
    {
        serverAnim.SetBool("isRunning", false);
    }

    //when holding nothing
    public void StopGrabAnim()
    {
        serverAnim.SetBool("isGrabbing", false);
    }

    //while washing
    public void WashAnim()
    {
        serverAnim.SetBool("isWashing", true);
    }

    //when finish washing
    public void StopWashAnim()
    {
        serverAnim.SetBool("isWashing", false);
    }
}
