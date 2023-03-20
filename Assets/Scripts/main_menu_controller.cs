using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // To change scenes through the menu


public class main_menu_controller : MonoBehaviour
{
    // Main Menu
    public VisualElement root;
    public UIDocument calibrateUI;
    public UIDocument cueSheetUI;

    private Button calibrateButton;
    private Button startPlayButton;
    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        calibrateUI.rootVisualElement.style.display = DisplayStyle.None;
        cueSheetUI.rootVisualElement.style.display = DisplayStyle.None;

        List<GameObject> rootObjects = new();
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

    private void StartPlayButtonClbk()
    {
        root.style.display = DisplayStyle.None;
        cueSheetUI.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void StartCalibrateButtonClbk()
    {
        root.style.display = DisplayStyle.None;
        calibrateUI.rootVisualElement.style.display = DisplayStyle.Flex;
        var cal_script = GameObject.FindObjectOfType(typeof(calibrate_gui_controller)) as calibrate_gui_controller;
        cal_script.beginCalibrate();
        //calibrateUI.GetComponent<calibrate_gui_controller>;
    }
}
