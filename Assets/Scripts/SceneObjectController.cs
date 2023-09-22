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

    private Camera[] activeProjectors;
    private List<Camera> activeLongProjectors;
    private List<Camera> activeBackProjectors;

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
            SearchActiveProjectors();
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

    private void SearchActiveProjectors()
    {
        activeProjectors = Camera.allCameras;
        activeLongProjectors = new();
        activeBackProjectors = new();
        for (int i=0; i<activeProjectors.Length; i++)
        {
            if (activeProjectors[i].targetDisplay == 0) continue; // Ignore GUI Camera
            if (activeProjectors[i].fieldOfView < 50)
            {
                activeLongProjectors.Add(activeProjectors[i]);
            }
            else
            {
                activeBackProjectors.Add(activeProjectors[i]);
            }
        }
    }

    [EditorCools.Button(row: "row-1", space: 10f)]
    private void LongProj1() => MoveToLongProj1();

    [EditorCools.Button(row: "row-1")]
    private void LongProj2() => MoveToLongProj2();

    [EditorCools.Button(row: "row-2", space: 5f)]
    private void BackProj1() => MoveToBackProj1();

    [EditorCools.Button(row: "row-2")]
    private void BackProj2() => MoveToBackProj2();

    private void MoveToLongProj1()
    {
        try
        {
            transform.position = activeLongProjectors[0].transform.position + new Vector3(0, 0, 5 * activeLongProjectors[0].transform.position.z / Mathf.Abs(activeLongProjectors[0].transform.position.z));
        }
        catch
        {
            Debug.Log("Did you set the correct FOV? Otherwise there are not enough projectors on the scene.");
        }
    }

    private void MoveToLongProj2()
    {
        try
        {
            transform.position = activeLongProjectors[1].transform.position + new Vector3(0, 0, 5 * activeLongProjectors[1].transform.position.z / Mathf.Abs(activeLongProjectors[1].transform.position.z));
        }
        catch
        {
            Debug.Log("Did you set the correct FOV? Otherwise there are not enough projectors on the scene.");
        }
    }

    private void MoveToBackProj1()
    {
        try
        {
            transform.position = activeBackProjectors[0].transform.position + new Vector3(0, 0, 5 * activeBackProjectors[0].transform.position.z / Mathf.Abs(activeBackProjectors[0].transform.position.z));
        }
        catch
        {
            Debug.Log("Did you set the correct FOV? Otherwise there are not enough projectors on the scene.");
        }
    }

    private void MoveToBackProj2()
    {
        try
        {
            transform.position = activeBackProjectors[1].transform.position + new Vector3(0, 0, 5 * activeBackProjectors[1].transform.position.z / Mathf.Abs(activeBackProjectors[1].transform.position.z));
        }
        catch
        {
            Debug.Log("Did you set the correct FOV? Otherwise there are not enough projectors on the scene.");
        }
    }
}