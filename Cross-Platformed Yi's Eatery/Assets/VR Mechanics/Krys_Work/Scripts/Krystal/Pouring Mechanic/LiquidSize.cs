using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSize : MonoBehaviour
{
    [SerializeField] private bool canHeightChange = true; // set to true when obj is supposed to resize vertically

    public GameObject liquid; //gameobj that represents the liquid
    public float diameterLimit = 0.05f;
    public float diameterIncrement = 0.002f;
    public float heightLimit = 0.05f;
    public float heightIncrement = 0.002f;

    private bool hasStarted = false;

    //public Coroutine pouring = null;

    // Use this for initialization
    void Start()
    {
        //set the size of the obj to 0 at the start
        liquid.gameObject.transform.localScale = new Vector3(0, 0, 0);

    }

    public void StartResizeCoroutine()
    {
        if(liquid.gameObject.transform.localScale.y < heightLimit)
        {
            StartCoroutine("ResizeLiquid");
        }
    }

    public void StopResizeCoroutine()
    {
        Debug.Log("Stopping resize coroutine");

        StopCoroutine("ResizeLiquid");
        hasStarted = false;

    }

    IEnumerator ResizeLiquid()
    {
        if (hasStarted)
        {
            yield break;
        } else
        {
            hasStarted = true;
        }

        Debug.Log("Coroutine started");

        //wait for tilt animation to complete
        yield return new WaitForSeconds(.1f);

        //if the liquid's diameter on the bottom of the container is still smaller than diameterLimit, increase its width by diameterIncrement
        for (float diameter = liquid.gameObject.transform.localScale.z; liquid.gameObject.transform.localScale.x < diameterLimit; diameter += diameterIncrement)
        {
            //increase diameter of liquid
            liquid.gameObject.transform.localScale = new Vector3(liquid.gameObject.transform.localScale.x + diameterIncrement, liquid.gameObject.transform.localScale.y + 0.0001f, liquid.gameObject.transform.localScale.z + diameterIncrement);

            //Debug.Log("x: " + liquid.gameObject.transform.localScale.x + " y: " + liquid.gameObject.transform.localScale.y + " z: " + liquid.gameObject.transform.localScale.z);

            yield return new WaitForSeconds(.05f);
        }


        if (canHeightChange) //if the liquid's diameter has reached diameterLimit but can increase in height, increase the height of the liquid
        {
            for (float height = liquid.gameObject.transform.localScale.y; liquid.gameObject.transform.localScale.y < heightLimit; height += heightIncrement)
            {
                //increase height of liquid
                liquid.gameObject.transform.localScale = new Vector3(liquid.gameObject.transform.localScale.x, liquid.gameObject.transform.localScale.y + heightIncrement, liquid.gameObject.transform.localScale.z);

                //Debug.Log("x: " + liquid.gameObject.transform.localScale.x + " y: " + liquid.gameObject.transform.localScale.y + " z: " + liquid.gameObject.transform.localScale.z);

                yield return new WaitForSeconds(.05f);
            }

        }

        hasStarted = false;

    }

    /*
    IEnumerator ResetLiquidHeight()
    {
        float tempDiameter = liquid.gameObject.transform.localScale.x;
        float tempHeight = liquid.gameObject.transform.localScale.y;

        //if the liquid's diameter on the bottom of the container is still smaller than diameterLimit, increase its width by diameterIncrement
        for (float i = 20; liquid.gameObject.transform.localScale.z > 0; i--)
        {
            //increase diameter of liquid
            liquid.gameObject.transform.localScale -= new Vector3(tempDiameter / 20, tempHeight / 20, tempDiameter / 20);

            yield return new WaitForSeconds(.05f);
        }

        liquid.gameObject.transform.localScale = new Vector3(0, 0, 0);
    }*/
}
