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


    #endregion

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
        totalScore.text = Mathf.RoundToInt(Evaluation_OverallPlayerPerformance.OverallScore).ToString();
        serversPerformance.text = Mathf.RoundToInt(Evaluation_CustomerService.CalculateCustomerServiceScore()).ToString() + "%";
        customersServed.text = Evaluation_CustomerService.NumCustomersServed.ToString();
        customersLost.text = Evaluation_CustomerService.NumCustomersLost.ToString();

    }
}
