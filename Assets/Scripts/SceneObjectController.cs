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


    private string labelText = "";

    private void OnDrawGizmos()
    {
        bool hasMeshRenderer = HasMeshRendererInChildren(transform);

        if (activeSceneNames.Count > 0 && hasMeshRenderer )
        {
            labelText = string.Join(", ", activeSceneNames);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1.9f);
            Gizmos.color = Color.white;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 14;
            Handles.Label(transform.position + Vector3.up * 2, labelText, style);
        }

        else if (GetComponent<Camera>() !=null)
        {
            labelText = gameObject.name;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 16;
            Handles.Label(transform.position + Vector3.up * 1, labelText, style);
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
