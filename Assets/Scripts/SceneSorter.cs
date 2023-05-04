using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneSorter : MonoBehaviour
{

    public bool updateSceneList = false;
    public bool resetSceneList = false;
    public List<string> sceneList = new();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ResetScenes();
    }

    public void AddScenes(List<string> sceneNames)
    {
        if (!updateSceneList) { return; }

        foreach (string sceneName in sceneNames)
        {
            if (!sceneList.Contains(sceneName))
            {
                sceneList.Add(sceneName);
            }
        }
    }

    private void ResetScenes()
    {
        if (resetSceneList)
        {
            sceneList = new();
            resetSceneList = false;
        }
    }

    public void LateUpdate()
    {
        updateSceneList = false;
    }
}
