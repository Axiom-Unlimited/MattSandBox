using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour {

    public bool showGUI;

    /// <summary>
    /// If SceneChangeActivated Bool is Equal to True:
    ///     1. Starts Timer
    ///     2. Starts Fade Animation
    ///     3. Once Timer Reaches 0, the Scene Will Change
    /// </summary>


    [Header("Scene Changer")]
    [Tooltip("Changes Scene on Timer")]
    public bool sceneChangedOnTimer;

    [Header("Adjustable Variables")]
    [Tooltip("Scene will need to be put in the Build Settings")]
    public string sceneName;
    [Tooltip("Set as an Image that Turns Black on Trigger(Fade)")]
    public Animator transitionAnimation;
    [Tooltip("Set Time for Scene")]
    public float setSceneTransitionTime;
    private float curTransitionTime;

	// Use this for initialization
	void Start () {
        curTransitionTime = setSceneTransitionTime;

	}
	
	void Update () {

        //Update Time when Activated
        if (sceneChangedOnTimer)
        {
            curTransitionTime -= Time.deltaTime;
        }

        //Change Scene
        if(curTransitionTime <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }

    }

    public void changeSceneOnBtn()
    {
        sceneChangedOnTimer = true;
        transitionAnimation.SetTrigger("Fade");
    }

    private void OnGUI()
    {
        if (showGUI)
            GUILayout.Label(curTransitionTime.ToString());
    }
}
