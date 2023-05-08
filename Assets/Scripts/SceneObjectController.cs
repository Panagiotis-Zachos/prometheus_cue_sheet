using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneObjectController : MonoBehaviour
{
    public List<string> activeSceneNames = new();
    public bool alwaysActive = false;
    public int targetDisplay;

    private List<int> sceneList = new();
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
            sceneSorterScript.AddScenes(activeSceneNames);
        }
    }

    public List<int> getSceneList()
    {
        return sceneList;
    }

    public void AddSceneList(int element)
    {
        sceneList.Add(element);
    }

    public void ResetSceneList()
    {
        sceneList = new();
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
