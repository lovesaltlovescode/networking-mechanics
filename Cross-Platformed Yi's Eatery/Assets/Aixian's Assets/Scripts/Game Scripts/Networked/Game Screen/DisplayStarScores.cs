using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class DisplayStarScores : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI oneStarScore;
    [SerializeField] private TextMeshProUGUI twoStarsScore;
    [SerializeField] private TextMeshProUGUI threeStarsScore;
    [SerializeField] private GameObject[] filledStars = new GameObject[3];
    //[SerializeField] private GameObject[] greyStars = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        DisplayScores();
    }

    public void DisplayScores()
    {
        oneStarScore.text = LevelStats.Instance.oneStarScore_current.ToString();
        twoStarsScore.text = LevelStats.Instance.twoStarScore_current.ToString();
        threeStarsScore.text = LevelStats.Instance.threeStarScore_current.ToString();
        RpcDisplayScores();
    }

    public void RpcDisplayScores()
    {
        filledStars[0].SetActive(false);
        filledStars[1].SetActive(false);
        filledStars[2].SetActive(false);
        //oneStarScore.text = LevelStats.Instance.oneStarScore_current.ToString();
        //twoStarsScore.text = LevelStats.Instance.twoStarScore_current.ToString();
        //threeStarsScore.text = LevelStats.Instance.threeStarScore_current.ToString();
    }

    public void ToggleStars()
    {
        if (Evaluation_OverallPlayerPerformance.Instance.customerServiceScore >= LevelStats.Instance.oneStarScore_current
            && Evaluation_OverallPlayerPerformance.Instance.customerServiceScore <= LevelStats.Instance.twoStarScore_current)
        {
            filledStars[0].SetActive(true);
            filledStars[1].SetActive(false);
            filledStars[2].SetActive(false);
        }
        else if (Evaluation_OverallPlayerPerformance.Instance.customerServiceScore >= LevelStats.Instance.twoStarScore_current
            && Evaluation_OverallPlayerPerformance.Instance.customerServiceScore <= LevelStats.Instance.threeStarScore_current)
        {
            filledStars[0].SetActive(true);
            filledStars[1].SetActive(true);
            filledStars[2].SetActive(false);
        }
        else if (Evaluation_OverallPlayerPerformance.Instance.customerServiceScore >= LevelStats.Instance.threeStarScore_current)
        {
            filledStars[0].SetActive(true);
            filledStars[1].SetActive(true);
            filledStars[2].SetActive(true);
        }
        else
        {
            filledStars[0].SetActive(false);
            filledStars[1].SetActive(false);
            filledStars[2].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DisplayScores();
        ToggleStars();
    }
}
