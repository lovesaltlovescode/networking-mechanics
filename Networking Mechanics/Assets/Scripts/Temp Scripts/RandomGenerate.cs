using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


//Generate a random 6 digit number in the most efficient way possible

public class RandomGenerate : MonoBehaviour
{

    public int number;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log("Random Generate: Running");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateRandom();
        }
    }

    public void GenerateRandom()
    {
            number = Random.Range(100000, 999999);
            UnityEngine.Debug.Log("Code is: " + number);
    }


}
