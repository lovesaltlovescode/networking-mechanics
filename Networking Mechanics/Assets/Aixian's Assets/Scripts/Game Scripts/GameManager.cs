using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

/// <summary>
/// Singleton game manager to contain all server-controlled variables
/// positions and counts of ingredients and plates
/// </summary>
public class GameManager : NetworkBehaviour
{

    #region Singleton

    private static GameManager _instance;

    //property
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion

    #region Variables

    [Header("Customers")]

    [SyncVar]
    public float timeSinceLastSpawn = 0f;

    [SyncVar]
    public float currentNumWaitingCustomers = 0f;


    [Header("Ingredient Tray")]
    public Transform[] trayPositions; //where ingredients should be placed
    public GameObject[] ingredientsOnTray = new GameObject[4];
    public int ingredientsOnTrayCount; //number of ingredients on the tray

    [Header("Sink Positions")]
    public Transform[] sinkPositions; //where dirty plates should be placed in the sink

    public GameObject[] platesInSink = new GameObject[4]; 

    [Header("Plate counts")]
    public int platesInSinkCount; //number of plates in the sink
    public int cleanPlatesCount;
    public Transform[] cleanPlateSpawnPositions;
    public GameObject[] cleanPlatesOnTable;

    [Header("Drinks")]
    public Transform[] drinkPositions; //where the drink should be spawned
    public GameObject[] drinksOnCounter = new GameObject[2]; //number of drinks on the counter
    public int drinksCount; //number of drinks on the counter

    [Header("DrinksCooldown")]

    public Image cooldownImg;
    [SyncVar]
    public float cooldown = 5f;
    [SyncVar]
    public bool isCooldown = false;
    [SyncVar]
    public bool isDrinkCoroutineRunning = false;


    [Header("Dishes")]
    public Transform[] dishSpawnPoints;
    public GameObject[] dishesOnCounter = new GameObject[3] { null, null, null };
    //public int dishCount; //number of dishes on the counter

    public TextMeshProUGUI serverScoreText;

    [Header("Mood")]

    [SyncVar]
    public float currentShopMood = 50;
    public Slider moodIndicator;

    [Header("Mood feedback")]
    [SerializeField] private Animator moodIncreaseAnim;
    [SerializeField] private TextMeshProUGUI moodIncreaseText;

    [SerializeField] private Animator moodDecreaseAnim;
    [SerializeField] private TextMeshProUGUI moodDecreaseText;

    #endregion

    private void Awake()
    {
        //if there is already an instance of this object in the scene, destroy this
        if(_instance && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    private void Start()
    {
        //StartLevel();
    }

    public void StartLevel()
    {
        if (moodIncreaseText && moodDecreaseText)
        {
            moodIncreaseText.gameObject.SetActive(false);
            moodDecreaseText.gameObject.SetActive(false);
        }

        serverScoreText.text = "0";
        moodIndicator.interactable = false;
        moodIndicator.value = 50f;

        LevelTimer.Instance.StartTimer();
    }

    #region Reset Level


    //Method to reset all values when level is started
    public void ResetLevel()
    {
        if (moodIncreaseText && moodDecreaseText)
        {
            moodIncreaseText.gameObject.SetActive(false);
            moodDecreaseText.gameObject.SetActive(false);
        }


        timeSinceLastSpawn = 0f;
        currentNumWaitingCustomers = 0f;
        ResetPlates();
        ResetDrinks();
        ResetDishes();

        serverScoreText.text = "0";
        moodIndicator.value = 50;
        currentShopMood = 50;
        LevelTimer.Instance.StartTimer();

    }

    public void ResetPlates()
    {
        for (int i = 0; i < platesInSink.Length - 1; i++)
        {
            NetworkServer.Destroy(platesInSink[i]);
            platesInSink[i] = null;
        }

        platesInSinkCount = 0;
    }

    public void ResetDrinks()
    {
        for (int i = 0; i < drinksOnCounter.Length - 1; i++)
        {
            NetworkServer.Destroy(drinksOnCounter[i]);
            drinksOnCounter[i] = null;
        }
        drinksCount = 0;

        isCooldown = false;
        isDrinkCoroutineRunning = false;
    }

    public void ResetDishes()
    {
        for (int i = 0; i < dishesOnCounter.Length - 1; i++)
        {
            NetworkServer.Destroy(dishesOnCounter[i]);
            dishesOnCounter[i] = null;
        }
    }

    #endregion

    #region Handle Scores and Customers

    //add score
    [ServerCallback]
    public void AddServerScore(float score)
    {
        float updatedServerScore = Evaluation_OverallPlayerPerformance.UpdateCustomerServiceScore(score);
        RpcAddServerScore(score, updatedServerScore);
    }


    //For client to call command
    [Command]
    public void CmdAddServerScore(float score)
    {
        float updatedServerScore = Evaluation_OverallPlayerPerformance.UpdateCustomerServiceScore(score);
        RpcAddServerScore(score, updatedServerScore);
    }

    [ClientRpc]
    public void RpcAddServerScore(float score, float updatedServerScore)
    {
        serverScoreText.text = updatedServerScore.ToString();
    }

    //reduce score
    [ServerCallback]
    public void ReduceServerScore(float score)
    {
        float updatedServerScore = Evaluation_OverallPlayerPerformance.UpdateCustomerServiceScore(score, true);
        RpcReduceServerScore(score, updatedServerScore);   
    }

    [ClientRpc]
    public void RpcReduceServerScore(float score, float updatedServerScore)
    {
        
        serverScoreText.text = updatedServerScore.ToString();
    }

    #endregion

    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        IncrementMood(5);
    //    }

    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        IncrementMood(5, true);
    //    }
        
    //}

    //decrease = true if mood decrease

    [ServerCallback]
    public void IncrementMood(float value, bool decrease = false)
    {
        if (decrease)
        {
            float updatedShopMood = currentShopMood -= value;

            //dont allow mood to decrease below 0
            if (updatedShopMood < 0 || currentShopMood <= 0)
            {
                return;
            }
            else
            {
                RpcDecreaseMood(updatedShopMood, value);
            }
        }
        else
        {
            float updatedShopMood = currentShopMood += value;

            //dont allow mood to increase above 100
            if (updatedShopMood > 100 || currentShopMood >= 100)
            {
                return;
            }
            else
            {
                RpcIncreaseMood(updatedShopMood, value);
            }
            
        }
        
        currentShopMood = Mathf.RoundToInt(moodIndicator.value);

        //Debug.Log("Current shop mood " + currentShopMood);

    }

    [Command]
    public void CmdIncrementMood(float value)
    {
        float updatedShopMood = currentShopMood += value;

        //dont allow mood to increase above 100
        if (updatedShopMood > 100 || currentShopMood >= 100)
        {
            return;
        }
        else
        {
            RpcIncreaseMood(updatedShopMood, value);
        }

        currentShopMood = Mathf.RoundToInt(moodIndicator.value);

        //Debug.Log("Current shop mood " + currentShopMood);

    }

    [ClientRpc]
    public void RpcDecreaseMood(float updatedShopMood, float value)
    {
        if (updatedShopMood > moodIndicator.minValue)
        {
            moodIndicator.value = updatedShopMood;
        }

        StartCoroutine(FadeInFadeOutText("-" + value, moodDecreaseText));
    }

    [ClientRpc]
    public void RpcIncreaseMood(float updatedShopMood, float value)
    {
        //StartMoveMoodIndicator(value);
        if (updatedShopMood < moodIndicator.maxValue)
        {
            moodIndicator.value = updatedShopMood;
        }
        StartCoroutine(FadeInFadeOutText("+" + value, moodIncreaseText, true));
    }

    //public void StartMoveMoodIndicator(float value, bool decrease = false)
    //{
    //    if (isMoodCoroutineRunning)
    //    {
    //        //bool used to ensure that coroutine does not get called while coroutine is running
    //        return;
    //    }

    //    isMoodCoroutineRunning = true;

    //    if (decrease)
    //    {
    //        StartCoroutine(MoveMoodIndicator(value, true));
    //    }
    //    else
    //    {
    //        StartCoroutine(MoveMoodIndicator(value));
    //    }

    //}

    //IEnumerator MoveMoodIndicator(float value, bool decrease = false)
    //{
    //    if (decrease)
    //    {
    //        currentShopMood -= value;
    //    }
    //    else
    //    {
    //        currentShopMood += value;
    //    }

    //    float updatedMood = currentShopMood;


    //    while (moodIndicator.value != updatedMood)
    //    {
    //        moodIndicator.value = Mathf.MoveTowards(currentShopMood, updatedMood, 1*Time.deltaTime);
    //    }

    //    yield return null;
    //    isMoodCoroutineRunning = false;
    //}

    #region DisplayMoodText

    //if gain points, play green text anim 
    IEnumerator FadeInFadeOutText(string _text, TextMeshProUGUI textToDisplay, bool _moodIncrease = false, bool _fadeIn = true, bool _fadeOut = true)
    {
        textToDisplay.text = _text;

        textToDisplay.gameObject.SetActive(true);


        if (_fadeIn)
        {
            if (_moodIncrease)
            {
                moodIncreaseAnim.SetBool("fadeIn", true);
                //Debug.Log("fade in bool set to true");
                yield return null;

                //Debug.Log("fade in clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(0.4f);
            }
            else
            {

                moodDecreaseAnim.SetBool("fadeIn", true);
                //Debug.Log("fade in bool set to true");
                yield return null;

                //Debug.Log("fade in clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(0.4f);
            }


        }

        if (_fadeOut)
        {
            if (_moodIncrease)
            {
                moodIncreaseAnim.SetBool("fadeIn", false);
                //Debug.Log("fade in bool set to false");
                yield return null;

                //Debug.Log("fade out clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(0.6f);
            }
            else
            {
                moodDecreaseAnim.SetBool("fadeIn", false);
                //Debug.Log("fade in bool set to false");
                yield return null;

                //Debug.Log("fade out clip length: " + wordAnim.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(0.6f);
            }


        }

        textToDisplay.gameObject.SetActive(false);
        // Debug.Log("set words to false");

        yield return null;
    }

    #endregion

}
