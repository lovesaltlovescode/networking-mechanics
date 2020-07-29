using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;


#region Summary

//instance of this script is spawned in by the network manager into the game scene
//Handles spawning of players in the game scene

#endregion


public class PlayerSpawnSystem : NetworkBehaviour
{
    //DIFFERENT PREFABS TO BE SPAWNED BASED ON INDEX
    [SerializeField] private GameObject xiaoBenPrefab = null;
    [SerializeField] private GameObject daFanPrefab = null;
    [SerializeField] private GameObject xiaoFanPrefab = null;
    [SerializeField] private GameObject xiaoLiPrefab = null;
    [SerializeField] private GameObject daLiPrefab = null;



    //list of transforms in the scene
    //the different transforms will be the spawn points of players
    private static List<Transform> spawnPoints = new List<Transform>();

    //players spawn at different positions depending on the order they came (their index)
    //network manager will increment this number everytime a new player is spawned, so that players are always spawning at the next available point
    public static int nextIndex = 0;

    ////List to contain player tags to be assigned when spawned, follows same index as next index
    //public static List<string> playerTags = new List<string>{ "XiaoBen", "DaFan", "DaLi", "XiaoFan", "XiaoLi" };

    //temp variable to assign prefab as and assign authority to
    private GameObject playerInstance;

    //Add spawn point, adds to list 
    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform); //add the transform to the list

        //Order the spawn points by how they are arranged in the hierarchy, and adds them to the list in that order
        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    //function to remove spawn point, remove from list
    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);


    //when this gameobject starts existing on the server, we subscribe spawn player to on server readied,
    public override void OnStartServer() => CustomNetworkManager.OnServerReadied += SpawnPlayer;

    [ServerCallback] //prevents clients from using this method
    //when this object gets destroyed, unsubscribe from event
    private void OnDestroy() => CustomNetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    //take in connection from event
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex); //Returns the transform (spawn point) at the specified index

        if (spawnPoint == null) //if no spawn point
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}"); //Should not be seeing this error unless there are insufficient spawn points
            return;
        }

        //check for nextindex, and spawn the corresponding prefab in the corresponding position
        switch (nextIndex)
        {
            case 0:
                //spawn in the player, instantiate at spawn points index, facing same way that spawn point is facing
                playerInstance = Instantiate(xiaoBenPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
                Debug.Log("PlayerSpawnSystem - Spawned in Xiao Ben!");
                playerInstance.tag = "XiaoBen";
                break;

            case 1:
                //spawn in the player, instantiate at spawn points index, facing same way that spawn point is facing
                playerInstance = Instantiate(daFanPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
                Debug.Log("PlayerSpawnSystem - Spawned in Da Fan!");
                playerInstance.tag = "DaFan";
                break;

            case 2:
                //spawn in the player, instantiate at spawn points index, facing same way that spawn point is facing
                playerInstance = Instantiate(xiaoFanPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
                Debug.Log("PlayerSpawnSystem - Spawned in Xiao Fan!");
                playerInstance.tag = "XiaoFan";
                break;

            case 3:
                //spawn in the player, instantiate at spawn points index, facing same way that spawn point is facing
                playerInstance = Instantiate(xiaoLiPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
                Debug.Log("PlayerSpawnSystem - Spawned in Xiao Li!");
                playerInstance.tag = "XiaoLi";
                break;

            case 4:
                //spawn in the player, instantiate at spawn points index, facing same way that spawn point is facing
                playerInstance = Instantiate(daLiPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
                Debug.Log("PlayerSpawnSystem - Spawned in Da Li!");
                playerInstance.tag = "DaLi";
                break;
        }

        //spawn the object on the other clients, and pass in connection belonging to that client
        //because the player object that is being spawned in belongs to this player's connection and they will have authority over it
        NetworkServer.Spawn(playerInstance, conn);
        //playerInstance.tag = playerTags[nextIndex];

        //increment index for next player, each player gets their own spawnpoint
        nextIndex++;
    }



    //[ClientRpc]
    //public void RpcChangePlayerTag(GameObject obj, string clientPlayerTag)
    //{
    //    obj.tag = clientPlayerTag;
    //    Debug.Log("This client's player tag is: " + clientPlayerTag);
    //}

}
