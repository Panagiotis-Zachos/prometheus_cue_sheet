using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectController : MonoBehaviour
{
    public int sceneNumber;
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
}
