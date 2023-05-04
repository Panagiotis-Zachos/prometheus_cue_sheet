using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneObjectController : MonoBehaviour
{
    public List<int> sceneList = new();
    public List<string> sceneNames = new();
    public bool alwaysActive = false;
    public int targetDisplay;

    private Transform initTransform;
    private SceneSorter sceneSorterScript;
    // Start is called before the first frame update
    void Start()
    {
        initTransform = transform;
        sceneSorterScript = GameObject.Find("Scene Sorter").GetComponent<SceneSorter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying){
            sceneSorterScript.AddScenes(sceneNames);
        }
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
