using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_AllowMovement : MonoBehaviour
{
    public VR_PrimaryButtonEventListener buttonListener;
    [SerializeField] private VR_ArmSwinger MovementScript;
    [SerializeField] private HandHider HandHiderScript_Ray, HandHiderScript_Direct;


    void Start()
    {
        buttonListener.primaryButtonPress.AddListener(onPrimaryButtonEvent);
    }

    public void onPrimaryButtonEvent(bool pressed)
    {
        ToggleMovement(pressed);
    }

    private void ToggleMovement(bool enable)
    {
        Debug.Log("toggle movement " + enable);
        HandHiderScript_Ray.ToggleRay(!enable, false);
        HandHiderScript_Direct.ToggleDirectInteractor(!enable, false);

        MovementScript.vrCanMove = enable;
    }
}
