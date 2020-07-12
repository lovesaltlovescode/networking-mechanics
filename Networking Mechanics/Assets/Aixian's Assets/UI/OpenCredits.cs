using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenCredits : MonoBehaviour
{
	public GameObject creditsPopup;

    // Start is called before the first frame update
    void Start()
    {
        creditsPopup.SetActive(false);
    }

    // Update is called once per frame
    public void openCredits()
    {
        creditsPopup.SetActive(true);
    }

	public void closeCredits()
    {
        creditsPopup.SetActive(false);
    }
}
