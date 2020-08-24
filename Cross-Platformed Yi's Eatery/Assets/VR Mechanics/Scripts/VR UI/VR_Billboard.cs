using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Billboard : MonoBehaviour
{
    private void Update()
    {
        if(VR_BillboardTarget.target != null)
            transform.LookAt(VR_BillboardTarget.target);
    }
}
