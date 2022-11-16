using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // To change scenes through the menu

public class GUI_Controller : MonoBehaviour
{

    public Button open_cue_button;
    public Button save_cue_button;
    public Button edit_cue_button;
    public Button save_settings_button;

    public Label scene_hour_label;
    public Label scene_min_label;
    public Label scene_sec_label;

    public Label play_hour_label;
    public Label play_min_label;
    public Label play_sec_label;

    public Button start_scene_1;
    public Button start_scene_2;
    public Button start_scene_3;


    private float play_time_start;
    private float scene_time_start;

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
        start_scene_1.clickable.clicked += () => StartScene(1);

        start_scene_2 = root.Q<Button>("start_scene_2");
        start_scene_2.clickable.clicked += () => StartScene(2);

        start_scene_3 = root.Q<Button>("start_scene_3");
        start_scene_3.clickable.clicked += () => StartScene(3);
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

    private void StartScene(int sceneSelected){
        Debug.Log("Button clicked");

        // get root objects in scene
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        scene_time_start = Time.time;

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];
            SceneObjectController soc = gameObject.GetComponent<SceneObjectController>();
            if (soc != null)
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
}
