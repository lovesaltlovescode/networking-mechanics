using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Retrieves and prints the players' scores in the level
/// </summary>
public class LevelEvaluation : NetworkBehaviour
{
    #region Variables

    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private TextMeshProUGUI totalScore;
    [SerializeField] private TextMeshProUGUI customersServed;
    [SerializeField] private TextMeshProUGUI customersLost;
    //[SerializeField] private TextMeshProUGUI foodWasted;

    [SerializeField] private TextMeshProUGUI serversPerformance;
    [SerializeField] private TextMeshProUGUI chefPerformance;

    [Header("Stars")]
    [SerializeField] private TextMeshProUGUI starsAttained;
    [SerializeField] private TextMeshProUGUI oneStarScore;
    [SerializeField] private TextMeshProUGUI twoStarsScore;
    [SerializeField] private TextMeshProUGUI threeStarsScore;
    [SerializeField] private GameObject[] filledStars = new GameObject[3];
    [SerializeField] private GameObject[] greyStars = new GameObject[3];

    [Header("Player Performance")]
    [SerializeField] private Image serverStars;
    [SerializeField] private Image chefStars;

    [Header("Buttons")]
    [SerializeField] GameObject nextLevel;
    //[SerializeField] GameObject retryLevel;
    [Scene] [SerializeField] private string gameScene = string.Empty;

    #endregion

    public static CustomNetworkManager networkGameManager; //Network manager object

    //Property
    private CustomNetworkManager NetworkGameManager
    {
        get
        {
            if (networkGameManager != null)
            {
                return networkGameManager; //If there is a network room manager, then return that object
            }

            //If its null, then just get it
            return networkGameManager = NetworkManager.singleton as CustomNetworkManager;
            //Cast network manager as a singleton to get our custom network manager
        }

    }

    private void Awake()
    {
        oneStarScore.text = LevelStats.OneStarScore_current.ToString();
        twoStarsScore.text = LevelStats.TwoStarScore_current.ToString();
        threeStarsScore.text = LevelStats.ThreeStarScore_current.ToString();
    }

    private void Start()
    {
        UpdateEvaluationValues();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            UpdateEvaluationValues();
        }
    }

    #region Calculate and Update Values


    public void UpdateEvaluationValues()
    {
        levelNumber.text = LevelStats.Level.ToString();

        Evaluation_OverallPlayerPerformance.CalculateOverallScore();

        float overallScore = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.OverallScore);
        totalScore.text = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.OverallScore).ToString();

        serversPerformance.text = Mathf.RoundToInt(Evaluation_CustomerService.CalculateCustomerServiceScore()).ToString() + "%";
        DisplayServerStars();

        customersServed.text = Evaluation_CustomerService.NumCustomersServed.ToString();
        customersLost.text = Evaluation_CustomerService.NumCustomersLost.ToString();

        starsAttained.text = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.EvaluateScore(overallScore)).ToString() + "Stars";
        CalculateStars();
    }

    public void DisplayServerStars()
    {
        float serversPerformance = Evaluation_CustomerService.CalculateCustomerServiceScore();
        serversPerformance = Mathf.Round(serversPerformance * 10.0f) * 0.1f;

        Debug.Log("Servers performance decimal" + serversPerformance);
        serverStars.fillAmount = serversPerformance;
    }

    public void CalculateStars()
    {
        float overallScore = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.OverallScore);
        int starsAttained = Evaluation_OverallPlayerPerformance.EvaluateScore(overallScore);

        switch (starsAttained)
        {
            case 0:
                greyStars[0].SetActive(true);
                greyStars[1].SetActive(true);
                greyStars[2].SetActive(true);
                nextLevel.SetActive(false);
                break;

            case 1:
                filledStars[0].SetActive(true);
                greyStars[1].SetActive(true);
                greyStars[2].SetActive(true);
                break;

            case 2:
                filledStars[0].SetActive(true);
                filledStars[1].SetActive(true);
                greyStars[2].SetActive(true);
                break;

            case 3:
                filledStars[0].SetActive(true);
                filledStars[1].SetActive(true);
                filledStars[2].SetActive(true);
                break;
        }
    }

    #endregion

    //when next level button is pressed
    public void NextLevel()
    {
        Debug.Log("Next level");
        
        NetworkGameManager.ServerChangeScene(gameScene);
        NetworkGameManager.OnServerSceneChanged(gameScene);

        LevelStatsAnnouncer.MoveToNextLevel();
        ResetLevel();
    }

    //retry level
    public void RetryLevel()
    {
        Debug.Log("Retry level");

        NetworkGameManager.ServerChangeScene(gameScene);
        NetworkGameManager.OnServerSceneChanged(gameScene);
        ResetLevel();
    }

    public void ResetLevel()
    {
        GameManager.Instance.ResetLevel();

        //TableColliderManager.Instance.ClearTableList();
        Evaluation_OverallPlayerPerformance.ResetAllScores();

        LevelTimer.Instance.levelStarted = true;
        //NetworkServer.Destroy(this.gameObject);
    }

    [ClientRpc]
    public void RpcResetLevel()
    {
    }

}
