using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

/// <summary>
/// Controls the Intro Sequence.
/// 
/// On Key Press will skip to the end of the end and activate Start Menu.
/// </summary>
public class Script_IntroController : MonoBehaviour
{
    [SerializeField] private float startScreenFrame;

    [SerializeField] private PlayableDirector introDirector;

    [SerializeField] private Script_IntroInputManager inputManager;

    void OnEnable()
    {
        inputManager.IsDisabled = false;
    }
    
    void Update()
    {
        inputManager.HandleEnterInput();
    }
    
    public void Play()
    {
        introDirector.Play();
    }

    // Skip to frame where Start Screen starts. Timeline will then initialize Start Screen via Signals.
    public void SkipToStartScreen()
    {
        introDirector.time = startScreenFrame / ((TimelineAsset)introDirector.playableAsset).editorSettings.fps;
        introDirector.Evaluate();
    }

    public void DisableInput()
    {
        inputManager.IsDisabled = true;
    }
}
