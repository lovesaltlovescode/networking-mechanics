﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator customerAnim;
    [SerializeField] private Animator overheadFeedbackAnim;


    public void StopWalkingAnim()
    {
        customerAnim.SetBool("isWalking", false);
    }

    public void WalkingAnim()
    {
        customerAnim.SetBool("isWalking", true);
    }

    #region seated customer animations
    //customer sitting down on seat
    public void SitDownAnim()
    {
        customerAnim.SetBool("isSitting", true); 
        //Debug.Log("SitDownAnim called");
    }

    #region browsing menu and ordering animations
    //customer browses menu
    public void BrowseMenuAnim()
    {
        customerAnim.SetTrigger("readMenu");
        //Debug.Log("BrowseMenuAnim called");
    }


    //customer is ready to order
    public void OrderAnim()
    {
        customerAnim.SetBool("isOrdering", true);
        //Debug.Log("OrderAnim called");
    }


    //customer is ready to order
    public void WaitForFoodAnim()
    {
        customerAnim.SetBool("isOrdering", false);
        //Debug.Log("WaitForFoodAnim called");
    }
    #endregion


    #region eating animation
    //customer starts eating
    public void StartEatingAnim()
    {
        customerAnim.SetBool("isEating", true);
        //Debug.Log("StartEatingAnim called");
    }


    //customer stops eating and sits idly
    public void StopEatingAnim()
    {
        customerAnim.SetBool("isEating", false);
        //Debug.Log("StopEatingAnim called");
    }
    #endregion


    //customer standing up and leaving restaurant
    public void LeaveAnim()
    {
        customerAnim.SetTrigger("leave");
        overheadFeedbackAnim.SetTrigger("despawn");
        //Debug.Log("LeaveAnim called");
    }

    #endregion

    //----------------------------------------------------------------------------------------
    //customer curling up into a ball while being carried
    public void CurlUpAnim()
    {
        //Debug.Log("CurlUpAnim called, nothing is here though");
    }

}
