using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Video;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // To change scenes through the menu
using System.Text.RegularExpressions;
using System;
using System.Collections;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using SimpleJSON;
public class GUI_Controller : MonoBehaviour
{

    private VisualElement root;

    private Label scene_timer_label;
    private Label play_timer_label;

    //private Label camStatusLabel;
    //private Label projStatusLabel;

    private int currentlyActiveScene = 99999;

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

    private RequestSocket client;

    // Add Images
    public Texture2D StartButton;
    // Activation Buttons
    public Texture2D Activated_Static_Video;
    public Texture2D Activated_Animation;

    // Deactivation Buttons
    public Texture2D DeActivated_Static_Video;
    public Texture2D DeActivated_Animation;

    // Variables for buttons in GUI
    private Texture2D Static_Button;
    private Texture2D Animation_Button;
    private Texture2D Poll_Button;
 
    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        play_time_start = Time.time;
        scene_timer_label = root.Q<Label>("scene_timer");
        play_timer_label = root.Q<Label>("play_timer");

       
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

        InitColorSliders(root);
        InitMoveSliders(root);
        InitRotSliders(root);

        StartCoroutine(DoSomethingAfterStart());

        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLabelTime(play_timer_label, play_time_start);
        UpdateLabelTime(scene_timer_label, scene_time_start);
    }

    IEnumerator DoSomethingAfterStart()
    {
        yield return null; // Wait for the next frame

        // Scene selection buttons initialization
        InitStartSceneElements(root.Q<VisualElement>("scene_vis_element"), GetUniqueScenes(), GetDescriptionScene());

    }

    public void PlayStartTime()
    {
        play_time_start = Time.time;
        scene_time_start = Time.time;
    }

    public int GetCurrentlyActiveScene()
    {
        return currentlyActiveScene;
    }

    private void InitColorSliders(VisualElement root)
    {
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
    }

    private void InitMoveSliders(VisualElement root)
    {
        lightXSlider = root.Q<Slider>("x_offset_control");
        lightXSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'X'));
        lightYSlider = root.Q<Slider>("y_offset_control");
        lightYSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Y'));
        lightZSlider = root.Q<Slider>("z_offset_control");
        lightZSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Z'));
    }

    private void InitRotSliders(VisualElement root)
    {
        lightXRotSlider = root.Q<Slider>("x_rot_control");
        lightXRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'X'));
        lightYRotSlider = root.Q<Slider>("y_rot_control");
        lightYRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Y'));
        lightZRotSlider = root.Q<Slider>("z_rot_control");
        lightZRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Z'));
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
                foreach (int sceneNumber in soc.getSceneList())
                {
                    if (!uniqueScenes.Contains(sceneNumber))
                    {
                        uniqueScenes.Add(sceneNumber);
                        
                    }
                }
            }
        }
        uniqueScenes.Sort();

        return uniqueScenes;
       
    }
    private List<string> GetDescriptionScene()
    {
        // This method finds all the description given to each scene
        GameObject hand = GameObject.Find("Scene Sorter");
        List<string> sceneNames = hand.GetComponent<SceneSorter>().sortedSceneList;

        return sceneNames;

    }
    private void InitStartSceneElements(VisualElement midVisElement, List<int> uniqueScenes, List<string> descriptionNames)
    {
        var labelBorderWidthHor = 2;
        var labelBorderWidthVert = 2;
         // Initialize list of scene buttons
         List<Button> all_scenebuttons= new List<Button>();

        // get root objects in scene
        List<GameObject> rootObjects = new();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);
        List<GameObject> scene_GameObjects = new();

        for (int i = 0; i < uniqueScenes.Count; ++i)
        {
            scene_GameObjects.Clear();
            // Iterate through each found object to find the objects of the scene
            foreach (GameObject rootObject in rootObjects)
            {
                if (rootObject.GetComponent<SceneObjectController>().activeSceneNames.Contains(descriptionNames[i]))
                {
                    scene_GameObjects.Add(rootObject);
                }
            }

            // Initialize Buttons to be deactivated
            Poll_Button = DeActivated_Static_Video;
            Static_Button = DeActivated_Static_Video;
            Animation_Button = DeActivated_Animation;

            foreach (GameObject scene_GameObject in scene_GameObjects)
            {
                // Check for polls
                if (scene_GameObject.GetComponent<PollMajority>() != null || scene_GameObject.GetComponent<PollLiveCount>() != null)
                {
                    Poll_Button = Activated_Static_Video;
                }
                // Check for animations
                if (scene_GameObject.GetComponentInChildren<VideoPlayer>() != null)
                {
                    Animation_Button = Activated_Animation;
                }
                // Check for static projections
                if (scene_GameObject.GetComponentInChildren<VideoPlayer>() == null && scene_GameObject.GetComponent<PollMajority>() == null && scene_GameObject.GetComponent<PollLiveCount>() == null)
                {
                    Static_Button = Activated_Static_Video;
                }
            }



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

                text = "",
                style =
                {
                    // Set Texture2D StartButton as Background
                    backgroundImage =new StyleBackground(StartButton),
                    
                    width = Length.Percent(10),
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

            // Add each scene button to a list 
            all_scenebuttons.Add(sceneButton);
            sceneVisElement.Add(sceneButton);

            var cueNumberLabel = new Label()
            {
                text = uniqueScenes[i].ToString(),
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
            sceneVisElement.Add(cueNumberLabel);

            sceneButton.RegisterCallback<ClickEvent, int>((evt, index) =>
            {               
 
                StartSceneButtonClbk(sceneButton,uniqueScenes[index], all_scenebuttons);
            }, i);

            var sceneDescriptionLabel = new Label()
            {
                text = descriptionNames[i],

                style =
            {
                width = Length.Percent(18),
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
                text = "",
                style =
            {
                backgroundImage=new StyleBackground(Static_Button),
                //backgroundColor = Color.yellow,
                width = Length.Percent(10),
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

            var sceneAnimationProjLabel = new Label()
            {
                text = "",
                style =
            {
                backgroundImage=new StyleBackground(Animation_Button),
                //backgroundColor = Color.yellow,
                width = Length.Percent(15),
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
            sceneVisElement.Add(sceneAnimationProjLabel);

           

            var videoLabel = new Label()
            {
                text = "",
                style =
            {
                backgroundImage=new StyleBackground(Poll_Button),
                //backgroundColor = Color.yellow,
                width = Length.Percent(10),
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

            var commentaryLabel = new Label()
            {
                text = string.Join(", ", scene_GameObjects.Select(obj => obj.name).ToList()),
                style =
            {
                width = Length.Percent(30),
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
            sceneVisElement.Add(commentaryLabel);


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

    private void StartSceneButtonClbk(Button sceneButton,int sceneSelected, List<Button> all_scenebuttons) {

        // Adjust the background colors of the Start Buttons
        foreach (Button button in all_scenebuttons) {
            if (button == sceneButton)
            {
                // Set the background color of the clicked button
                Color newColor = new Color(0.3f, 0.9f, 0.3f, 0.9f);
                button.style.backgroundColor = newColor;
            }
            else if(all_scenebuttons.IndexOf(button)+1 < sceneSelected)
            {
                // Set the background color of previous buttons 
                Color newColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
                button.style.backgroundColor = newColor;
            }
            else
            {
                // Set the background color of next buttons 
                Color newColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);
                button.style.backgroundColor = newColor;
            }
        }

        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use
        client = new RequestSocket();
        client.Connect("tcp://localhost:5555");

        Debug.Log("Connected to Server");

        currentlyActiveScene = sceneSelected;
        client.SendFrame(currentlyActiveScene.ToString());

        while (!client.TryReceiveFrameString(out var message));

        client.Close();
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use

        // get root objects in scene
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();

        scene.GetRootGameObjects(rootObjects);

        scene_time_start = Time.time;

        // Two loops are separate here to guarantee correct callback order execution
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.TryGetComponent<SceneObjectController>(out var soc))
            {
                if (!soc.SceneExistsInList(sceneSelected))
                {
                    if (gameObject.GetComponent<Camera>())
                    {
                        gameObject.GetComponent<Camera>().targetDisplay = 7; // Turn off camera for this cue
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.TryGetComponent<SceneObjectController>(out var soc))
            {
                if (soc.SceneExistsInList(sceneSelected))
                {
                    if (gameObject.GetComponent<Camera>())
                    {
                        gameObject.GetComponent<Camera>().targetDisplay = soc.targetDisplay - 1; // Turn on camera for this cue
                    }
                    else
                    {
                        gameObject.SetActive(true);
                    }

                }
            }
        }
        //ChangeColor(sceneElement[1]);
        //CheckProjectorReady();
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
            var lightSceneObjCont = light.GetComponent<SceneObjectController>();
            if (lightSceneObjCont.SceneExistsInList(currentlyActiveScene))
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
            if (lightSceneObjCont.SceneExistsInList(currentlyActiveScene))
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
            if (lightSceneObjCont.SceneExistsInList(currentlyActiveScene))
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
