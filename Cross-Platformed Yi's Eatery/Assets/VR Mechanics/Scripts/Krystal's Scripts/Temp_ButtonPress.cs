using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_ButtonPress : MonoBehaviour
{
    [SerializeField] GameObject toggledText;
    private bool isVisible = true;
    public void ToggleText()
    {
        isVisible = !isVisible;
        toggledText.SetActive(isVisible);
    }
}
