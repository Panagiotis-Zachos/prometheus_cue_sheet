using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // To change scenes through the menu
using System.IO;

using Newtonsoft.Json;
public class main_menu_controller : MonoBehaviour
{
    // Main Menu
    public VisualElement root;
    public UIDocument calibrateUI;
    public UIDocument cueSheetUI;

    private Button calibrateButton;
    private Button startPlayButton;

    private List<GameObject> rootObjects = new();
    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        calibrateUI.rootVisualElement.style.display = DisplayStyle.None;
        cueSheetUI.rootVisualElement.style.display = DisplayStyle.None;

        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            // Disable everything except cameras and Cue Sheet GUI
            if (!(gameObject.GetComponent<Camera>() || gameObject.GetComponent<UIDocument>()))
            {
                gameObject.SetActive(false);
            }
        }

        InitCameras();
        startPlayButton = root.Q<Button>("startSceneButton");
        startPlayButton.RegisterCallback<ClickEvent>((evt) =>
        {
            StartPlayButtonClbk();
        });

        calibrateButton = root.Q<Button>("calibrateButton");
        calibrateButton.RegisterCallback<ClickEvent>((evt) =>
        {
            StartCalibrateButtonClbk();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitCameras()
    {
        string json_Path = Application.dataPath + "/Calibration_JSON/";
        if (!Directory.Exists(json_Path))
        {
            return;
        }
        string[] files = Directory.GetFiles(json_Path, "*.json");

        foreach (string json_path in files)
        {
            StreamReader reader = new StreamReader(json_path);
            CameraSaveObject tmp = JsonConvert.DeserializeObject<CameraSaveObject>(reader.ReadToEnd());

            for (int i = 0; i < rootObjects.Count; ++i)
            {
                GameObject gameObject = rootObjects[i];
                if (gameObject.GetComponent<Camera>() && string.Compare(gameObject.name, tmp.Name) == 0)
                {
                    gameObject.transform.position = new Vector3(tmp.pos_x, tmp.pos_y, tmp.pos_z);
                    gameObject.transform.rotation = new Quaternion(tmp.quat_x, tmp.quat_y, tmp.quat_z, tmp.quat_w);
                }
            }
        }
    }

    private void StartPlayButtonClbk()
    {
        calibrateUI.rootVisualElement.style.display = DisplayStyle.None;
        root.style.display = DisplayStyle.None;
        cueSheetUI.rootVisualElement.style.display = DisplayStyle.Flex;

        var cal_script = GameObject.FindObjectOfType(typeof(GUI_Controller)) as GUI_Controller;
        cal_script.PlayStartTime();
    }

    private void StartCalibrateButtonClbk()
    {
        calibrateUI.rootVisualElement.style.display = DisplayStyle.Flex;
        root.style.display = DisplayStyle.None;
        cueSheetUI.rootVisualElement.style.display = DisplayStyle.None;
        var cal_script = GameObject.FindObjectOfType(typeof(calibrate_gui_controller)) as calibrate_gui_controller;
        cal_script.beginCalibrate();
    }
}
