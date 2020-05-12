using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class TestPlayer : NetworkBehaviour
{

    public TextMeshProUGUI playerName;

    [SyncVar]
    public Color hairColor;
    [SyncVar]
    public Color eyeColor;

    [SyncVar]
    public string name;

    [SyncVar]
    public string model;

    public void Start()
    {
        playerName.text = gameObject.tag;

    }

    public void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gameObject.transform.position += Vector3.left;

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gameObject.transform.position -= Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gameObject.transform.position += Vector3.forward;
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gameObject.transform.position -= Vector3.forward;
        }
    }

}
