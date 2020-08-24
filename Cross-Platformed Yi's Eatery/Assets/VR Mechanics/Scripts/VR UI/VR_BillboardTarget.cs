using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_BillboardTarget : MonoBehaviour
{
    public static Transform target;

    private void Awake()
    {
        target = this.gameObject.transform;
    }

    private void Update()
    {
        target = this.gameObject.transform;
    }
}
