using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // To change scenes through the menu


public class GUI_Controller : MonoBehaviour
{

    private Button open_cue_button;
    private Button save_cue_button;
    private Button edit_cue_button;
    private Button save_settings_button;

    private Label scene_hour_label;
    private Label scene_min_label;
    private Label scene_sec_label;

    private Label play_hour_label;
    private Label play_min_label;
    private Label play_sec_label;

    private int currentlyActiveScene;
    private Button start_scene_1;
    private Button start_scene_2;
    private Button start_scene_3;


    private float play_time_start;
    private float scene_time_start;

    private List<Light> playLights = new List<Light>();

    private Slider redSlider;
    private Slider greenSlider;
    private Slider blueSlider;
    private Slider intensitySlider;

    private Slider lightXSlider;
    private Slider lightYSlider;
    private Slider lightZSlider;

    private Slider lightXRotSlider;
    private Slider lightYRotSlider;
    private Slider lightZRotSlider;

    // Start is called before the first frame update
    void Start()
    {
        // SceneManager.LoadScene('name-of-the-scene'); // For future use
        var root = GetComponent<UIDocument>().rootVisualElement;

        open_cue_button = root.Q<Button>("open_cue_file");
        save_cue_button = root.Q<Button>("save_cue_file");
        edit_cue_button = root.Q<Button>("save_settings");
        save_settings_button = root.Q<Button>("save_settings");

        play_time_start = Time.time;
        scene_hour_label = root.Q<Label>("scene_hours");
        scene_min_label = root.Q<Label>("scene_minutes");
        scene_sec_label = root.Q<Label>("scene_seconds");

        play_hour_label = root.Q<Label>("play_hours");
        play_min_label = root.Q<Label>("play_minutes");
        play_sec_label = root.Q<Label>("play_seconds");

        start_scene_1 = root.Q<Button>("start_scene_1");
        start_scene_1.clickable.clicked += () => StartSceneButtonClbk(1);

        start_scene_2 = root.Q<Button>("start_scene_2");
        start_scene_2.clickable.clicked += () => StartSceneButtonClbk(2);

        start_scene_3 = root.Q<Button>("start_scene_3");
        start_scene_3.clickable.clicked += () => StartSceneButtonClbk(3);

        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);
        
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.GetComponent<Light>())
            {
                playLights.Add(gameObject.GetComponent<Light>());
            }
        }

        redSlider = root.Q<Slider>("red_slider");
        redSlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'R'));
        greenSlider = root.Q<Slider>("green_slider");
        greenSlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'G'));
        blueSlider = root.Q<Slider>("blue_slider");
        blueSlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'B'));
        intensitySlider = root.Q<Slider>("intensity_slider");
        intensitySlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'I'));

        lightXSlider = root.Q<Slider>("x_offset_control");
        lightXSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'X'));
        lightYSlider = root.Q<Slider>("y_offset_control");
        lightYSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Y'));
        lightZSlider = root.Q<Slider>("z_offset_control");
        lightZSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Z'));

        lightXRotSlider = root.Q<Slider>("x_rot_control");
        lightXRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'X'));
        lightYRotSlider = root.Q<Slider>("y_rot_control");
        lightYRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Y'));
        lightZRotSlider = root.Q<Slider>("z_rot_control");
        lightZRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Z'));
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayTime(play_hour_label, play_min_label, play_sec_label);
        UpdateSceneTime(scene_hour_label, scene_min_label, scene_sec_label);
    }


    private void UpdatePlayTime(Label play_hour_label, Label play_min_label, Label play_sec_label)
    {
        var frame_time = Time.time;
        var time_hours = Mathf.Floor((frame_time - play_time_start) / 3600);
        var time_minutes = Mathf.Floor((frame_time - time_hours * 3600) / 60);
        var time_seconds = Mathf.Floor(frame_time - time_hours * 3600 - time_minutes * 60);

        play_hour_label.text = time_hours.ToString();
        play_min_label.text = time_minutes.ToString();
        play_sec_label.text = time_seconds.ToString();
    }

    private void UpdateSceneTime(Label scene_hour_label, Label scene_min_label, Label scene_sec_label)
    {
        var frame_time = Time.time;

        var scene_time_hours = Mathf.Floor((frame_time - scene_time_start) / 3600);
        var scene_time_minutes = Mathf.Floor((frame_time - scene_time_start - scene_time_hours * 3600) / 60);
        var scene_time_seconds = Mathf.Floor(frame_time - scene_time_start - scene_time_hours * 3600 - scene_time_minutes * 60);

        scene_hour_label.text = scene_time_hours.ToString();
        scene_min_label.text = scene_time_minutes.ToString();
        scene_sec_label.text = scene_time_seconds.ToString();
    }

    private void StartSceneButtonClbk(int sceneSelected){
        currentlyActiveScene = sceneSelected;

        // get root objects in scene
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        scene_time_start = Time.time;

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.TryGetComponent<SceneObjectController>(out var soc))
            {
                if (soc.sceneNumber != sceneSelected)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
        }
    }

    private void ColorSliderValueChangedClbk(float sliderVal, char sliderColor)
    {
        for (int i = 0; i < playLights.Count; ++i)
        {
            var light = playLights[i];
            if (light.GetComponent<SceneObjectController>().sceneNumber == currentlyActiveScene)
            {
                switch (sliderColor)
                {
                    case 'R':
                        light.color = new Vector4(sliderVal / 255.0f, light.color.g, light.color.b, 1.0f);
                        break;
                    case 'G':
                        light.color = new Vector4(light.color.r, sliderVal / 255.0f, light.color.b, 1.0f);
                        break;
                    case 'B':
                        light.color = new Vector4(light.color.r, light.color.g, sliderVal / 255.0f, 1.0f);
                        break;
                    case 'I':
                        light.intensity = sliderVal;
                        break;
                }
            }       
        }
    }

    private void PosOffsetSliderValueChangedClbk(float sliderVal, float sliderPrevVal, char sliderDim)
    {
        for (int i = 0; i < playLights.Count; ++i)
        {
            var light = playLights[i];
            var lightSceneObjCont = light.GetComponent<SceneObjectController>();
            if (lightSceneObjCont.sceneNumber == currentlyActiveScene)
            {
                var moveAmount = sliderPrevVal - sliderVal;
                Transform lightTransform = light.GetComponentInParent(typeof(Transform)) as Transform;
                switch (sliderDim)
                {
                    case 'X':
                        lightTransform.position = lightSceneObjCont.getInitTransform().position + new Vector3(moveAmount, 0.0f, 0.0f);
                        break;
                    case 'Y':
                        lightTransform.position = lightSceneObjCont.getInitTransform().position + new Vector3(0.0f, moveAmount, 0.0f);
                        break;
                    case 'Z':
                        lightTransform.position = lightSceneObjCont.getInitTransform().position + new Vector3(0.0f, 0.0f, moveAmount);
                        break;
                }
            }
        }
    }

    private void RotOffsetSliderValueChangedClbk(float sliderVal, float sliderPrevVal, char sliderDim)
    {
        for (int i = 0; i < playLights.Count; ++i)
        {
            var light = playLights[i];
            var lightSceneObjCont = light.GetComponent<SceneObjectController>();
            if (lightSceneObjCont.sceneNumber == currentlyActiveScene)
            {
                var moveAmount = sliderPrevVal - sliderVal;
                Transform lightTransform = light.GetComponentInParent(typeof(Transform)) as Transform;
                switch (sliderDim)
                {
                    case 'X':
                        lightTransform.Rotate(new Vector3(moveAmount, 0.0f, 0.0f), Space.World);
                        break;
                    case 'Y':
                        lightTransform.Rotate(new Vector3(0.0f, moveAmount, 0.0f), Space.World);
                        break;
                    case 'Z':
                        lightTransform.Rotate(new Vector3(0.0f, 0.0f, moveAmount), Space.World);
                        break;
                }
            }
        }
    }
}
