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

    [Header("Buttons")]
    [SerializeField] GameObject nextLevel;
    //[SerializeField] GameObject retryLevel;

    #endregion

    private void Awake()
    {
        oneStarScore.text = LevelStats.OneStarScore_current.ToString();
        twoStarsScore.text = LevelStats.TwoStarScore_current.ToString();
        threeStarsScore.text = LevelStats.ThreeStarScore_current.ToString();
    }

    private void Start()
    {
        Evaluation_OverallPlayerPerformance.CalculateOverallScore();
        UpdateEvaluationValues();
    }

    [ServerCallback]
    public void UpdateEvaluationValues()
    {
        RpcUpdateEvaluationValues();
    }

    [ClientRpc]
    public void RpcUpdateEvaluationValues()
    {
        levelNumber.text = LevelStats.Level.ToString();

        float overallScore = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.OverallScore);
        totalScore.text = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.OverallScore).ToString();

        serversPerformance.text = Mathf.RoundToInt(Evaluation_CustomerService.CalculateCustomerServiceScore()).ToString() + "%";

        customersServed.text = Evaluation_CustomerService.NumCustomersServed.ToString();
        customersLost.text = Evaluation_CustomerService.NumCustomersLost.ToString();

        starsAttained.text = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.EvaluateScore(overallScore)).ToString() + "Stars";
        CalculateStars();
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
}
