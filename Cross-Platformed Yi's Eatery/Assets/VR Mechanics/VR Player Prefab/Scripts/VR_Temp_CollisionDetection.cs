using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Temp_CollisionDetection : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
