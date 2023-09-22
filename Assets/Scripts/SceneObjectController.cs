using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    private string labelText = "";

    private int lineCount = 60; // Adjust the number of lines
    private float cylinderRadius = 0.01f; // Adjust the radius of the cylinder

    private void OnDrawGizmos()
    {

        bool hasMeshRenderer = HasMeshRendererInChildren(transform);
        // If it is not a camera and has a mesh, plot line and add text above GameObjects
        if (activeSceneNames.Count > 0 && hasMeshRenderer )
        {
            Gizmos.color = Color.white;
            // Make the lines thicker 

            // Calculate the angle between each line
            float angleStep = 360.0f / lineCount;
            float lineHeight = 1.8f;
            // Draw the lines in a cylinder
            for (int i = 0; i < lineCount; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 startPoint = transform.position + new Vector3(Mathf.Cos(angle) * cylinderRadius, 0, Mathf.Sin(angle) * cylinderRadius);
                Vector3 endPoint = startPoint + Vector3.up * lineHeight;

                // Draw the line
                Gizmos.DrawLine(startPoint, endPoint);
            }

            // Add the text
            labelText = string.Join(", ", activeSceneNames);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 16;
            style.fontStyle = FontStyle.Bold;
            Handles.Label(transform.position + Vector3.up * 2, labelText, style);
        }

        // If it is a camera add only text
        else if (GetComponent<Camera>() !=null)
        {
            labelText = gameObject.name;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 18;
            style.fontStyle = FontStyle.Bold;
            Handles.Label(transform.position + Vector3.up * 1, labelText, style);
        }
    }

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
    
    // Function for checking if a gameobject or its children have a mesh
    private bool HasMeshRendererInChildren(Transform parentTransform)
    {
        // Check if the parentTransform has a MeshRenderer
        MeshRenderer meshRenderer = parentTransform.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            return true;
        }

        // Recursively check child objects
        foreach (Transform child in parentTransform)
        {
            if (HasMeshRendererInChildren(child))
            {
                return true;
            }
        }

        // If no MeshRenderer was found in the hierarchy, return false
        return false;
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