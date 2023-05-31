using UnityEngine;
using System.Collections.Generic;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using SimpleJSON;

public class PollLiveCount : MonoBehaviour
{
    public Color color_1;
    public Color color_2;

    public GameObject pollResultsGO;
    private List<Component> children = new();

    private JSONNode jParse = null;
    private RequestSocket client;

    private bool waitReceive;
    private bool liveVoteCountRequestSent;

    private float answerTotal = 0;
    private float answerCount = 0;

    void Start()
    {

    }

    void Update()
    {
        jParse = GetMessage();
        if (jParse != null)
        {
            if (jParse["answer"] == 0)
            {
                answerCount++;
            }
            answerTotal++;

            int answerEnd = Mathf.RoundToInt(answerCount / answerTotal * children.Count);
            for (int i = 0; i < answerEnd; i++)
            {
                children[i].GetComponent<Renderer>().material.color = color_1;
            }
            for (int i = answerEnd; i < children.Count; i++)
            {
                children[i].GetComponent<Renderer>().material.color = color_2;
            }
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
            liveVoteCountRequestSent = false;
            waitReceive = true;
            Debug.Log("Connected to Server");

            answerTotal = 0;
            answerCount = 0;
            foreach (Component child in pollResultsGO.GetComponentsInChildren<Renderer>())
                children.Add(child);
            //option_1.GetComponent<Renderer>().material.color = color_1;
            //option_2.GetComponent<Renderer>().material.color = color_2;
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
        if (!liveVoteCountRequestSent)
        {
            client.SendFrame("start_live_count");
            liveVoteCountRequestSent = true;
        }

        if (waitReceive)
        {
            var gotMessage = client.TryReceiveFrameString(out var message);
            if (gotMessage)
            {
                switch (message)
                {
                    case "0":
                        Debug.Log("Live vote count started...");
                        client.SendFrame("send_submited_answer");
                        break;
                    case "Voting over":
                        waitReceive = false;
                        Debug.Log("Voting over.");
                        client.Close();
                        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use
                        break;
                    default:
                        jParse = JSON.Parse(message);
                        client.SendFrame("send_submited_answer");
                        return jParse;
                }
            }
        }
        
        return null;
    }
}
