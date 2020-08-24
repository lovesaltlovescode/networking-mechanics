using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempController : MonoBehaviour
{
    //Variables 
    //[SerializeField] private float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.position += transform.forward * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.position -= transform.right * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.position -= transform.forward * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.position += transform.right * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            gameObject.transform.position += transform.up * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            gameObject.transform.position -= transform.up * Time.deltaTime;
        }
    }
}
