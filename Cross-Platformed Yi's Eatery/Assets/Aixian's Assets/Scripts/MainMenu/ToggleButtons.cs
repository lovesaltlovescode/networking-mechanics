using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Toggle host room button active on PC
//toggle find room button active on android
public class ToggleButtons : MonoBehaviour
{

    public GameObject hostRoomBtn;
    public GameObject findRoomBtn;


#if UNITY_EDITOR

    public void Awake()
    {
        Debug.Log("In editor!");
        hostRoomBtn.SetActive(true);
        findRoomBtn.SetActive(true); 
        
    }

#elif UNITY_STANDALONE_WIN

    public void Awake()
    {
        //Debug.Log("Standalone windows!");
        hostRoomBtn.SetActive(true);
        findRoomBtn.SetActive(true); 
        //findRoomBtn.SetActive(false); 
    }


#elif UNITY_ANDROID

    public void Awake()
    {
        Debug.Log("On android!");
        hostRoomBtn.SetActive(false);
        findRoomBtn.SetActive(true); 
    }
#endif
}
