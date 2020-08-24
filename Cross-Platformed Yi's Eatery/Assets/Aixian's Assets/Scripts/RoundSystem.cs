using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundSystem : NetworkBehaviour
{

    [SerializeField] private Animator animator = null;

    private CustomNetworkManager room;

    private CustomNetworkManager Room
    {
        get
        {
            if( room != null) { return room; }
            return room = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    [ServerCallback]
    public void StartCountdown()
    {
        animator.enabled = true;
        RpcStartCountdown();
    }

    public void CountdownEnded()
    {
        animator.enabled = false;
        NetworkServer.Destroy(gameObject);
    }

    #region Server

    public override void OnStartServer()
    {
        CustomNetworkManager.OnServerStopped += CleanUpServer;
        CustomNetworkManager.OnServerReadied += CheckToStartRound;
    }

    [ServerCallback]
    private void OnDestroy() => CleanUpServer();

    [Server]
    private void CleanUpServer()
    {
        CustomNetworkManager.OnServerStopped -= CleanUpServer;
        CustomNetworkManager.OnServerReadied -= CheckToStartRound;
    }

    [ServerCallback]
    public void StartRound()
    {
        LevelTimer.Instance.StartTimer();
        RpcStartRound();
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        animator.enabled = true;
        RpcStartCountdown();
    }

    #endregion

    #region Clients

    [ClientRpc]
    public void RpcStartCountdown()
    {
        animator.enabled = true;

    }

    [ClientRpc]
    public void RpcStartRound()
    {
        Debug.Log("Start round");
    }


    #endregion

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
