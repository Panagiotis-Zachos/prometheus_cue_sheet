using UnityEngine;
using System.Collections.Generic;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using SimpleJSON;

public class PollMajority : MonoBehaviour
{
    public List<GameObject> pollResults= new();

    private JSONNode jParse = null;
    private RequestSocket client;
    private string message = null;
    private bool gotMessage = false;
    private int errorCounter = 0;
    
    void Start()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        client = new RequestSocket();
        client.Connect("tcp://localhost:5555");
        client.SendFrame("Hello");
        Debug.Log("Connected to Server");
    }

    void Update()
    {
        gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
        if (errorCounter >= 100)
        {
            // Debug.Log("Connection Error. Resetting socket.");
            errorCounter = 0;
            client.Close();
            NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
            client = new RequestSocket();
            client.Connect("tcp://localhost:5555");
            client.SendFrame("Hello");
        }
        if (gotMessage)
        {
            errorCounter = 0;
            //jParse = JSON.Parse(message);
            Debug.Log(message);
            client.SendFrame("Ready");
        }
        else
        {
            errorCounter++;
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDestroy()
    {
        client.Close();
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}
