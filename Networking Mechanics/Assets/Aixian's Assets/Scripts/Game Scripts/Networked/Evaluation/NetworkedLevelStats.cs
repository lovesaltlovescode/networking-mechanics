using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedLevelStats : NetworkBehaviour
{
    #region Singleton

    private static NetworkedLevelStats _instance;
    public static NetworkedLevelStats Instance { get { return _instance; } }

    private void Awake()
    {
        Debug.Log(this.gameObject.name);

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    [SyncVar]
    public int level = 1;

    [SyncVar]
    public int highestLevel = 1;

    [SyncVar]
    public float oneStarScore_current,
        twoStarScore_current, threeStarScore_current;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        level = LevelStats.Instance.level;
        highestLevel = LevelStats.Instance.highestLevel;

        oneStarScore_current = LevelStats.Instance.oneStarScore_current;
        twoStarScore_current = LevelStats.Instance.twoStarScore_current;
        threeStarScore_current = LevelStats.Instance.threeStarScore_current;
    }
}
