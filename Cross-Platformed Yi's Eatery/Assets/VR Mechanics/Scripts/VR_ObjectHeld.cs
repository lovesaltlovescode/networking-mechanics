using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class VR_ObjectHeld : MonoBehaviour
{
    private GameObject _objectBeingHeld = null;
    public bool ObjectBeingHeld { get { return _objectBeingHeld; } }

    public void SetObjectBeingHeld(GameObject obj)
    {
        _objectBeingHeld = obj;
    }
}
