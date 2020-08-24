using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GameObject textBox;
    private bool tempBool = true;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleTextVisibility()
    {
        tempBool = !tempBool;
        textBox.SetActive(tempBool);
    }
}
