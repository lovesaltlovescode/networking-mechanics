using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VR_IngredientInteraction : MonoBehaviour
{
    //cutting board-relevant variables
    public Transform cuttingBoardAttachPoint;
    public bool inCutBoardZone;
    public bool ingredientOnBoard;

    Rigidbody rb;

    public XRGrabInteractable grabInteractable;
    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        rb = GetComponent<Rigidbody>();

        grabInteractable.onSelectEnter.AddListener(StopAttach);
        grabInteractable.onSelectExit.AddListener(AttachToBoard);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "CuttingBoard")
        {
            inCutBoardZone = true;
            Debug.Log("Hit CuttingBoard Zone");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "CuttingBoard")
        {
            inCutBoardZone = false;
            Debug.Log("Exited CuttingBoard Zone");
        }
    }

    IEnumerator Attach()
    {
        while (Vector3.Distance(transform.position, cuttingBoardAttachPoint.position) > 0.03f)
        {
            transform.position = Vector3.Lerp(transform.position, cuttingBoardAttachPoint.position, 5 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 5 * Time.deltaTime);
            yield return null;
        }

        //ensures that we detect the food as fully stablised on the cutting board as OK to work with.
        ingredientOnBoard = true;
        rb.isKinematic = true;
        
    }

    public void StopAttach(XRBaseInteractor interactor) //called on selectenter
    {
        StopCoroutine("Attach"); //ensures to stop coroutine when selecting object again in the midst of coroutine.
        rb.isKinematic = false;
        ingredientOnBoard = false; //changes condition when grabbed by object.
    }

    public void AttachToBoard(XRBaseInteractor interactor) //called on selectexit
    {
        if (inCutBoardZone)
        {
            StartCoroutine("Attach");
        }
    }
}
