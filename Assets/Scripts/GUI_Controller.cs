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

    private float time_start;
    // Start is called before the first frame update
    void Start()
    {
        // SceneManager.LoadScene('name-of-the-scene'); // For future use
        var root = GetComponent<UIDocument>().rootVisualElement;

        open_cue_button = root.Q<Button>("open_cue_file");
        save_cue_button = root.Q<Button>("save_cue_file");
        edit_cue_button = root.Q<Button>("save_settings");
        save_settings_button = root.Q<Button>("save_settings");

        time_start = Time.time;
        scene_hour_label = root.Q<Label>("scene_hours");
        scene_min_label = root.Q<Label>("scene_minutes");
        scene_sec_label = root.Q<Label>("scene_seconds");

        play_hour_label = root.Q<Label>("play_hours");
        play_min_label = root.Q<Label>("play_minutes");
        play_sec_label = root.Q<Label>("play_seconds");

        start_scene_1 = root.Q<Button>("start_scene_1");
        start_scene_1.clicked += start_scene;
    }

    // Update is called once per frame
    void Update()
    {
        updateTotalTime(play_hour_label, play_min_label, play_sec_label);
    }


    private void updateTotalTime(Label play_hour_label, Label play_min_label, Label play_sec_label)
    {
        var frame_time = Time.time;
        var time_hours = Mathf.Floor((frame_time - time_start) / 3600);
        var time_minutes = Mathf.Floor((frame_time - time_hours * 3600) / 60);
        var time_seconds = Mathf.Floor(frame_time - time_hours * 3600 - time_minutes * 60);

        play_hour_label.text = time_hours.ToString();
        play_min_label.text = time_minutes.ToString();
        play_sec_label.text = time_seconds.ToString();
    }

    private void start_scene(){
        Debug.Log("Button clicked");
    }
}
