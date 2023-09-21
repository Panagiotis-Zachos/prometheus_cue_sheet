using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class SceneSorter : MonoBehaviour
{

    public bool updateSceneList = false;
    public bool resetSceneList = false;
    public List<string> sortedSceneList = new();

    private List<GameObject> rootObjects = new();
    private List<GameObject> prevObjList = new();
    private List<GameObject> newObjList = new();

    // Start is called before the first frame update
    void Start()
    {
        CreateActiveSceneLists();
    }

    // Update is called once per frame
    void Update()
    {
        ResetScenes();
        AutoAttachSceneObjectController();
    }

    public void LateUpdate()
    {
        updateSceneList = false;
    }

    public void AddScenes(List<string> activeSceneNames)
    {
        if (!updateSceneList) { return; }

        foreach (string activeSceneName in activeSceneNames)
        {
            if (!sortedSceneList.Contains(activeSceneName))
            {
                sortedSceneList.Add(activeSceneName);
            }
        }
    }

    private void ResetScenes()
    {
        if (resetSceneList)
        {
            sortedSceneList = new();
            resetSceneList = false;
        }
    }

    private void CreateActiveSceneLists()
    {
        if (!Application.isPlaying){ return; }

        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        // Iterate through all game objects
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            var objectController = rootObjects[i].GetComponent<SceneObjectController>();
            if (objectController == null) { continue; }
            
            objectController.ResetSceneList(); // Reset the sceneList
            for (int j = 0; j < sortedSceneList.Count; j++)
            {
                if (objectController.activeSceneNames.Contains(sortedSceneList[j]))
                {
                    objectController.AddSceneList(j+1);
                }
            }
        }
    }

    private void AutoAttachSceneObjectController()
    {
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(newObjList);

        for (int i=0; i< newObjList.Count; i++)
        {
            if (prevObjList.Contains(newObjList[i])) continue;
            if (newObjList[i].GetComponent<SceneObjectController>() == null)
            {
                newObjList[i].AddComponent<SceneObjectController>();
            }
        }
        prevObjList = new List<GameObject>(newObjList);
    }
}
