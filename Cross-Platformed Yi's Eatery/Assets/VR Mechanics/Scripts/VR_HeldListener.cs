using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VR_HeldListener : XRBaseInteractable
{
    private bool _beingHeld = false;
    public bool BeingHeld { get { return _beingHeld; } }

    public void ToggleBeingHeld(bool enable)
    {
        Debug.Log("toggle being held: " + enable);
        _beingHeld = enable;
    }

    protected override void Awake()
    {
        base.Awake();
        onSelectEnter.AddListener(ObjBeingHeld);
        onSelectExit.AddListener(ObjDropped);
    }

    private void OnDestroy()
    {
        onSelectEnter.RemoveListener(ObjBeingHeld);
        onSelectExit.RemoveListener(ObjDropped);
    }

    private void ObjBeingHeld(XRBaseInteractor interactor)
    {
        VR_ObjectHeld objBeingHeldScript = interactor.gameObject.GetComponent<VR_ObjectHeld>();
        
        if(objBeingHeldScript != null)
        {
            objBeingHeldScript.SetObjectBeingHeld(this.gameObject);
        }

        ToggleBeingHeld(true);
    }


    private void ObjDropped(XRBaseInteractor interactor)
    {
        VR_ObjectHeld objBeingHeldScript = interactor.gameObject.GetComponent<VR_ObjectHeld>();

        if (objBeingHeldScript != null)
        {
            objBeingHeldScript.SetObjectBeingHeld(null);
        }

        ToggleBeingHeld(false);
    }

}
