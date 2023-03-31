using System.Collections.Generic;
using UnityEngine;

public class SceneObjectController : MonoBehaviour
{
    public List<int> sceneList = new();
    public bool alwaysActive = false;
    public int targetDisplay;

    private Transform initTransform;
    // Start is called before the first frame update
    void Start()
    {
        initTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform getInitTransform()
    {
        return initTransform;
    }

    public bool SceneExistsInList(int sceneSelected)
    {
        if (alwaysActive)
        {
            return true;
        }
        foreach (int number in sceneList)
        {
            if (number == sceneSelected)
            {
                return true;
            }
        }
        return false;
    }
}
