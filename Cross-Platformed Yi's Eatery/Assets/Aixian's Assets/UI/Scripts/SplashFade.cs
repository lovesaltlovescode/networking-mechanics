using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashFade : MonoBehaviour
{
	public Image splashScreen;
	public TextMeshProUGUI contText;
	public GameObject contBtn;
	public Canvas menuScreen;

    public void fadeOut()
    {
			splashScreen.CrossFadeAlpha(0, 0.5f, false);
			contText.CrossFadeAlpha(0, 0.5f, false);
			contBtn.SetActive(false);
			menuScreen.gameObject.SetActive(true);
    }
}
