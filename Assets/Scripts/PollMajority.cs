using UnityEngine;
using System.Collections.Generic;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using SimpleJSON;

public class PollMajority : MonoBehaviour
{
    public List<GameObject> pollOptions= new();

    private JSONNode jParse = null;
    private RequestSocket client;
    private bool messageReceived;
    private bool messageRequested;
    
    void Start()
    {
        
    }

    void Update()
    {
        jParse = GetMessage();
        if (jParse != null)
        {
            pollOptions[jParse["Question 1"][0]].GetComponentInChildren<Renderer>().enabled = true;
        }
    }

    private void OnEnable()
    {
        var gcl = GameObject.Find("CueSheet_UIDocument");
        gcl.TryGetComponent<GUI_Controller>(out var gco);
        if (gco.GetCurrentlyActiveScene() != 99999)
        {
            ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use
            client = new RequestSocket();
            client.Connect("tcp://localhost:5555");
            messageRequested = false;
            messageReceived = false;
            Debug.Log("Connected to Server");

            foreach (GameObject gob in pollOptions)
            {
                gob.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
    }

    private void OnDisable()
    {
        client.Close();
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use
    }

    private void OnDestroy()
    {
        client.Close();
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use
    }

    private JSONNode GetMessage()
    {
        if (!messageReceived)
        {
            if (!messageRequested)
            {
                client.SendFrame("send_poll_result");
                messageRequested = true;
            }
            var gotMessage = client.TryReceiveFrameString(out var message);
            if (gotMessage)
            {
                jParse = JSON.Parse(message);
                messageReceived = true;
                messageRequested = false;
                return jParse;
            }
        }
        return null;
    }
}
