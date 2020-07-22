using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientBehaviour : MonoBehaviour
{
    //variables
    [SerializeField] private string floorTag = "Floor";
    [SerializeField] private float rotMinTime = 5f, updateFrequency = 0.2f;

    [SerializeField] private Image timeMeterImg;

    [Header("Optional")]
    [SerializeField] private ParticleSystem spoiledPFX;
    [SerializeField] private Image spoiledIcon;

    public bool isRotten = false;
    private Coroutine timerCoroutine;
    private bool isCoroutineRunning = false; //bool used to ensure that coroutine does not get called while coroutine is running


    private void OnTriggerEnter(Collider other) //if the ingredient has been placed on the floor,
    {
        Debug.Log("IngredientBehaviour: " + other.gameObject.tag);
        if (other.gameObject.CompareTag(floorTag) && !isRotten && !isCoroutineRunning)
        {
            Debug.Log("fresh ingredient left on floor");

            //start the coroutine that updates the timer indicating how much time is left before the food rots
            timerCoroutine = StartCoroutine("FoodRotTime");
        }

        if (other.gameObject.CompareTag(floorTag))
        {
            Debug.Log("IngredientBehaviour: Its the damn floor");
        }
    }

    private void OnTriggerExit(Collider other) //if the ingredient has been picked up,
    {
        if (other.gameObject.CompareTag(floorTag) && !isRotten && isCoroutineRunning) // if the ingredient hasn't gone bad yet,
        {
            Debug.Log("fresh ingredient picked up");

            //stop the coroutine prematurely
            StopCoroutine(timerCoroutine);

            //disable timer
            timeMeterImg.enabled = false;
        }
    }


    //coroutine that updates the timer indicating how much time is left for the food before it rots
    IEnumerator FoodRotTime()
    {
        isCoroutineRunning = true;

        //enable a timer above the timer image
        timeMeterImg.enabled = true;

        timeMeterImg.fillAmount = 1f;
        float timeLeft = rotMinTime;

        while(timeLeft > 0)
        {
            yield return new WaitForSeconds(updateFrequency);

            //calculate amount of time left
            timeLeft -= updateFrequency;

            //display amount of time left
            timeMeterImg.fillAmount = timeLeft / rotMinTime;

        }

        //enable particle effects and whatnot
        RotOnFloor();
    }


    //method that activates effects that show that food has rot
    public void RotOnFloor()
    {
        isCoroutineRunning = false;

        isRotten = true;
        Debug.Log("Food is rotten.");

        //disable timer
        timeMeterImg.enabled = false;

        if(spoiledIcon != null)
        {
            //enable icon indicating that food item is spoiled
            spoiledIcon.enabled = true;
        }

        if (spoiledPFX != null)
        {
            //play particle effects
            spoiledPFX.Play();
        }
    }

    public void Update()
    {
        if (!isRotten)
        {
            return;
        }

        RottenIngredient();
    }

    public void RottenIngredient()
    {
        if (isRotten)
        {
            //disable the normal ingredient
            gameObject.transform.GetChild(0).gameObject.SetActive(false);

            //enable the rotten ingredient
            gameObject.transform.GetChild(1).gameObject.SetActive(true);

            //change tag of parent and object
            gameObject.transform.parent.tag = "RottenIngredient";
            gameObject.tag = "RottenIngredient";

            gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("RottenIngredient");
            gameObject.layer = LayerMask.NameToLayer("RottenIngredient");
            
        }
    }


}//class

