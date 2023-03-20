using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // To change scenes through the 

public class calibrate_gui_controller : MonoBehaviour
{
    public VisualElement root;
    public UIDocument MainUIDocument;

    private Button backButton;

    private Button previousCameraButton;
    private Button nextCameraButton;
    private Label currentCameraLabel;

    private int currentCamera = 0;

    private Slider cameraXSlider;
    private Slider cameraYSlider;
    private Slider cameraZSlider;
                   
    private Slider cameraXRotSlider;
    private Slider cameraYRotSlider;
    private Slider cameraZRotSlider;

    private List<Camera> playCameras = new();
    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        List<GameObject> rootObjects = new();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            if (gameObject.GetComponent<Camera>())
            {
                playCameras.Add(gameObject.GetComponent<Camera>());
            }
        }

        currentCameraLabel = root.Q<Label>("current_camera_label");

        previousCameraButton = root.Q<Button>("previous_camera_button");
        previousCameraButton.RegisterCallback<ClickEvent>((evt) =>
        {
            prevCameraClbk();
        });

        nextCameraButton = root.Q<Button>("next_camera_button");
        nextCameraButton.RegisterCallback<ClickEvent>((evt) =>
        {
            nextCameraClbk();
        });

        cameraXSlider = root.Q<Slider>("x_offset_control");
        cameraXSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'X'));
        cameraYSlider = root.Q<Slider>("y_offset_control");
        cameraYSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Y'));
        cameraZSlider = root.Q<Slider>("z_offset_control");
        cameraZSlider.RegisterValueChangedCallback(x => PosOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Z'));
        
        cameraXRotSlider = root.Q<Slider>("x_rot_control");
        cameraXRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'X'));
        cameraYRotSlider = root.Q<Slider>("y_rot_control");
        cameraYRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Y'));
        cameraZRotSlider = root.Q<Slider>("z_rot_control");
        cameraZRotSlider.RegisterValueChangedCallback(x => RotOffsetSliderValueChangedClbk(x.newValue, x.previousValue, 'Z'));

        backButton = root.Q<Button>("back_button");
        backButton.RegisterCallback<ClickEvent>((evt) =>
        {
            backButtonClbk();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void beginCalibrate()
    {
        for (int i = 0; i < playCameras.Count; ++i)
        {
            GameObject gameObject = playCameras[i].gameObject;
            if (gameObject.GetComponent<Camera>())
            {
                gameObject.SetActive(false);
            }
        }
        playCameras[0].gameObject.SetActive(true);
    }

    private void nextCameraClbk()
    {
        playCameras[currentCamera].gameObject.SetActive(false);
        currentCamera = (currentCamera + 1) % playCameras.Count;
        playCameras[currentCamera].gameObject.SetActive(true);

        cameraXSlider.SetValueWithoutNotify(0);
        cameraYSlider.SetValueWithoutNotify(0);
        cameraZSlider.SetValueWithoutNotify(0);

        cameraXRotSlider.SetValueWithoutNotify(0);
        cameraYRotSlider.SetValueWithoutNotify(0);
        cameraZRotSlider.SetValueWithoutNotify(0);
        cameraXRotSlider.SetValueWithoutNotify(0);
    }

    private void prevCameraClbk()
    {
        playCameras[currentCamera].gameObject.SetActive(false);
        currentCamera = ((currentCamera - 1) + playCameras.Count) % playCameras.Count;
        playCameras[currentCamera].gameObject.SetActive(true);

        cameraXSlider.SetValueWithoutNotify(0);
        cameraYSlider.SetValueWithoutNotify(0);
        cameraZSlider.SetValueWithoutNotify(0);

        cameraXRotSlider.SetValueWithoutNotify(0);
        cameraYRotSlider.SetValueWithoutNotify(0);
        cameraZRotSlider.SetValueWithoutNotify(0);
        cameraXRotSlider.SetValueWithoutNotify(0);
    }

    private void PosOffsetSliderValueChangedClbk(float sliderVal, float sliderPrevVal, char sliderDim)
    {
        var camera = playCameras[currentCamera];

        var moveAmount = sliderPrevVal - sliderVal;
        Transform cameraTransform = camera.GetComponentInParent(typeof(Transform)) as Transform;
        switch (sliderDim)
        {
            case 'X':
                cameraTransform.position += new Vector3(moveAmount, 0.0f, 0.0f);
                break;
            case 'Y':
                cameraTransform.position += new Vector3(0.0f, moveAmount, 0.0f);
                break;
            case 'Z':
                cameraTransform.position += new Vector3(0.0f, 0.0f, moveAmount);
                break;
        }
    }

    private void RotOffsetSliderValueChangedClbk(float sliderVal, float sliderPrevVal, char sliderDim)
    {
        var camera = playCameras[currentCamera];
        var moveAmount = sliderPrevVal - sliderVal;
        Transform cameraTransform = camera.GetComponentInParent(typeof(Transform)) as Transform;
        switch (sliderDim)
        {
            case 'X':
                cameraTransform.Rotate(new Vector3(moveAmount, 0.0f, 0.0f), Space.World);
                break;
            case 'Y':
                cameraTransform.Rotate(new Vector3(0.0f, moveAmount, 0.0f), Space.World);
                break;
            case 'Z':
                cameraTransform.Rotate(new Vector3(0.0f, 0.0f, moveAmount), Space.World);
                break;
         }
     }

    private void backButtonClbk()
    {
        for (int i = 0; i < playCameras.Count; ++i)
        {
            GameObject gameObject = playCameras[i].gameObject;
            if (gameObject.GetComponent<Camera>())
            {
                gameObject.SetActive(true);
            }
        }
        root.style.display = DisplayStyle.None;
        MainUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
