using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Use this script to test out custom character spawning

public class MMONetworkManager : NetworkManager
{
    public GameObject customPlayer2;

    public override void Start()
    {
        Debug.Log("MMONetworkManager has started");
    }


    //Specify generic traits of character to be spawned

    public class CreateMMOCharacterMessage : MessageBase
    {
        public Model model;
        public string name;
        public Color hairColor;
        public Color eyeColor;
    }

    public enum Model
    {
        XiaoBen,
        DaFan,
        DaLi,
        XiaoFan,
        XiaoLi
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        //register the message and handle it using oncreatecharacter function
        //Everytime this message is sent, it will run this function
        NetworkServer.RegisterHandler<CreateMMOCharacterMessage>(OnCreateCharacter);
    }

    //Called on client when connected to server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        //When a client is connected to the server,
        //we can send a message here, by creating a new instance and specify the custom trait

        CreateMMOCharacterMessage characterMessage = new CreateMMOCharacterMessage
        {
            model = Model.XiaoBen, //drawing from the enum created earlier
            name = "Joe Gaba Gaba",
            hairColor = Color.red,
            eyeColor = Color.green
        };


        //Each message will be send with the relevant information to the server with a message ID
        conn.Send(characterMessage); //sends the message with the relevant character information to the server with a message id
    }

    //function to handle the message when it gets registered
    //This runs everytime a message is sent, when a client connects, a prefab is spawned with the same message passed in...
    void OnCreateCharacter(NetworkConnection conn, CreateMMOCharacterMessage message)
    {
        
        ////if this model is dafan, spawn a separate prefab instead
        //if (message.model == Model.DaFan)
        //{
        //    Debug.Log("this is dafan, spawn a separate prefab");
        //    Instantiate(customPlayer2);
        //    customPlayer2.tag = message.model.ToString();
        //    return;

        //}

        // instantiate the player prefab 
        GameObject gameobject = Instantiate(playerPrefab);


        //change that player's tag to fit their message's model.
        //each player prefab will have a different tag as this function is called multiple times, and different messages are being sent at each time
        TestPlayer player = gameobject.GetComponent<TestPlayer>();

        player.hairColor = message.hairColor;
        player.eyeColor = message.eyeColor;
        player.name = message.name;
        player.model = message.model.ToString();
        playerPrefab.tag = message.model.ToString();

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);

    }


}
