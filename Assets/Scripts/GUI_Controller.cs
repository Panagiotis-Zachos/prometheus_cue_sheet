using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // To change scenes through the menu
using System.Text.RegularExpressions;
using System;

public class GUI_Controller : MonoBehaviour
{

    private Label scene_timer_label;
    private Label play_timer_label;

    private Label camStatusLabel;
    private Label projStatusLabel;

    private int currentlyActiveScene;

    private float play_time_start;
    private float scene_time_start;

    private List<Light> playLights = new();

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

    private TextField rText;
    private TextField gText;
    private TextField bText;
    private TextField iText;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        play_time_start = Time.time;
        scene_timer_label = root.Q<Label>("scene_timer");
        play_timer_label = root.Q<Label>("play_timer");

        // Scene selection buttons initialization
        camStatusLabel = root.Q<Label>("tracking_ready_label");
        projStatusLabel = root.Q<Label>("projector_ready_label");
        InitStartSceneElements(root.Q<VisualElement>("scene_vis_element"), GetUniqueScenes());

        // Scene light controls initialization

        List<GameObject> rootObjects = new();
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
        rText = root.Q<TextField>("redText");
        redSlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'R', rText));
        rText.RegisterValueChangedCallback(x => UpdateTextFieldDisplayClbk(rText, redSlider));

        greenSlider = root.Q<Slider>("green_slider");
        gText = root.Q<TextField>("greenText");
        greenSlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'G', gText));
        gText.RegisterValueChangedCallback(x => UpdateTextFieldDisplayClbk(gText, greenSlider));

        blueSlider = root.Q<Slider>("blue_slider");
        bText = root.Q<TextField>("blueText");
        blueSlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'B', bText));
        bText.RegisterValueChangedCallback(x => UpdateTextFieldDisplayClbk(bText, blueSlider));

        intensitySlider = root.Q<Slider>("intensity_slider");
        iText = root.Q<TextField>("intText");
        intensitySlider.RegisterValueChangedCallback(x => ColorSliderValueChangedClbk(x.newValue, 'I', iText));
        iText.RegisterValueChangedCallback(x => UpdateTextFieldDisplayClbk(iText, intensitySlider));


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
        UpdateLabelTime(play_timer_label, play_time_start);
        UpdateLabelTime(scene_timer_label, scene_time_start);
    }

    private List<int> GetUniqueScenes()
    {
        // This method finds all unique scenes via parsing the available GameObjects and looking at their 'SceneNumber' property

        List<int> uniqueScenes = new();

        // get root objects in scene
        List<GameObject> rootObjects = new();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.TryGetComponent<SceneObjectController>(out var soc))
            {
                if (uniqueScenes.IndexOf(soc.sceneNumber) == -1)
                {
                    uniqueScenes.Add(soc.sceneNumber);
                }
            }
        }
        uniqueScenes.Sort();

        return uniqueScenes;
    }

    private void InitStartSceneElements(VisualElement midVisElement, List<int> uniqueScenes)
    {
        var labelBorderWidthHor = 2;
        var labelBorderWidthVert = 2;
        for (int i = 0; i < uniqueScenes.Count; ++i)
        {
            var sceneVisElement = new VisualElement()
            {
                style =
                {

                    //width = Length.Percent(100),
                    height = Length.Percent(100 / uniqueScenes.Count),

                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.NoWrap,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,

                    marginLeft = 0,
                    marginRight = 0,
                    marginBottom = 0,
                    marginTop = 0,

                    paddingLeft = 0,
                    paddingRight = 0,
                    paddingBottom = 0,
                    paddingTop = 0,
                }
            };

            var sceneButton = new Button()
            {
                text = "Start",
                style =
                {
                    fontSize = 16,
                    width = Length.Percent(8),
                    height = Length.Percent(100),

                    marginLeft = 0,
                    marginRight = 0 ,
                    marginBottom = 0,
                    marginTop = 0,

                    paddingLeft = 0,
                    paddingRight = 0,
                    paddingBottom = 0,
                    paddingTop = 0,

                    borderLeftWidth = labelBorderWidthHor,
                    borderRightWidth = labelBorderWidthHor,
                    borderBottomWidth = labelBorderWidthVert,
                    borderTopWidth = labelBorderWidthVert,

                    borderLeftColor = Color.black,
                    borderRightColor = Color.black,
                    borderBottomColor = Color.black,
                    borderTopColor = Color.black
                }
            };

            sceneButton.RegisterCallback<ClickEvent, int>((evt, index) =>
            {
                StartSceneButtonClbk(uniqueScenes[index]);
            }, i);
            sceneVisElement.Add(sceneButton);

            var cueNumberLabel = new Label()
            {
                text = uniqueScenes[i].ToString(),
                style =
            {
                width = Length.Percent(4),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(cueNumberLabel);

            var sceneDescriptionLabel = new Label()
            {
                text = "X",

                style =
            {
                width = Length.Percent(11),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleLeft,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black

            }
            };
            sceneVisElement.Add(sceneDescriptionLabel);

            var sceneStaticProjLabel = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(6.5f),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(sceneStaticProjLabel);

            var sceneDynamicProjLabel = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(7),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(sceneDynamicProjLabel);

            var sceneProj1Label = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(9),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(sceneProj1Label);

            var sceneProj2Label = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(9),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(sceneProj2Label);

            var sceneProj3Label = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(9),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(sceneProj3Label);

            var sceneProj4Label = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(9),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(sceneProj4Label);

            var cameraLabel = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(6),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(cameraLabel);

            var lightsLabel = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(6),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(lightsLabel);

            var videoLabel = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(8),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(videoLabel);

            var audioLabel = new Label()
            {
                text = "X",
                style =
            {
                width = Length.Percent(8),
                height = Length.Percent(100),

                marginLeft = 0,
                marginRight = 0 ,
                marginBottom = 0,
                marginTop = 0,

                paddingLeft = 0,
                paddingRight = 0,
                paddingBottom = 0,
                paddingTop = 0,

                borderLeftWidth = labelBorderWidthHor,
                borderRightWidth = labelBorderWidthHor,
                borderBottomWidth = labelBorderWidthVert,
                borderTopWidth = labelBorderWidthVert,

                borderLeftColor = Color.black,
                borderRightColor = Color.black,
                borderBottomColor = Color.black,
                borderTopColor = Color.black,

                unityTextAlign = TextAnchor.MiddleCenter,
                whiteSpace = WhiteSpace.Normal,
                fontSize = 16,

                color = Color.black
            }
            };
            sceneVisElement.Add(audioLabel);

            midVisElement.Add(sceneVisElement);
        }
    }

    private void UpdateLabelTime(Label timer_label, float time_start)
    {
        var frame_time = Time.time;
        
        var scene_time_hours = Mathf.Floor((frame_time - time_start) / 3600);
        var scene_time_minutes = Mathf.Floor((frame_time - time_start - scene_time_hours * 3600) / 60);
        var scene_time_seconds = Mathf.Floor(frame_time - time_start - scene_time_hours * 3600 - scene_time_minutes * 60);

        timer_label.text = string.Format("{0:00}:{1:00}:{2:00}", scene_time_hours, scene_time_minutes, scene_time_seconds);

    }

    private void StartSceneButtonClbk(int sceneSelected) {
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
        CheckCameraReady();
        CheckProjectorReady();
    }

    private void CheckCameraReady()
    {
        var camRdy = true;
        camStatusLabel.text = "Not Ready";
        camStatusLabel.style.backgroundColor = Color.red;
        while (!camRdy) ;

        if (camRdy)
        {
            camStatusLabel.text = "Ready";
            camStatusLabel.style.backgroundColor = Color.green;
        }
    }
    
    private void CheckProjectorReady()
    {
        var projRdy = true;
        projStatusLabel.text = "Not Ready";
        projStatusLabel.style.backgroundColor = Color.red;
        while (!projRdy) ;
        if (projRdy)
        {
            projStatusLabel.text = "Ready";
            projStatusLabel.style.backgroundColor = Color.green;
        }
    }
    private void UpdateTextFieldDisplayClbk(TextField txtF, Slider cSlid)
    {
        txtF.value = Regex.Replace(txtF.text, @"[^0-9 ]", "");
        try
        {
            if (double.Parse(txtF.value) > 255)
            {
                txtF.value = "" + 255;
            }
            txtF.SetValueWithoutNotify(txtF.value);
            cSlid.value = float.Parse(txtF.value);
        }
        catch (FormatException)
        {
            Debug.Log("Text in this textField is in the wrong format");
        }
    }

    private void ColorSliderValueChangedClbk(float sliderVal, char sliderColor, TextField txtF)
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
            txtF.SetValueWithoutNotify(sliderVal.ToString());
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
