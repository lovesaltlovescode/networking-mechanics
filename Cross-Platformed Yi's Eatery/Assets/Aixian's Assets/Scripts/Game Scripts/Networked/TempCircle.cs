using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Temporary
/// Change player circle colour according to the tag
/// </summary>
public class TempCircle : NetworkBehaviour
{
    public SpriteRenderer circleArrow;

    // Start is called before the first frame update
    void Start()
    {
        switch (gameObject.tag)
        {
            case "XiaoBen":
                circleArrow.color = Color.white;
                break;
            case "DaFan":
                circleArrow.color = Color.blue;
                break;
            case "XiaoFan":
                circleArrow.color = Color.red;
                break;
            case "XiaoLi":
                circleArrow.color = Color.yellow;
                break;
            case "DaLi":
                circleArrow.color = Color.green;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
