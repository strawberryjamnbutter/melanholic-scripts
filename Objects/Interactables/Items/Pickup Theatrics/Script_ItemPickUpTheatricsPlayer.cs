﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;

/// <summary>
/// For TimelineAndEnter, this will start tracking Enter/Space input after timeline is done
/// 
/// NOTE: Ensure to activate a CTA at the end of the theatric that tells player to press enter/space
/// NOTE: Because this reacts in LateUpdate() ensure to wrap this call with a bool check or before 
/// this properly exits it'll activate another theatric
/// </summary>
public class Script_ItemPickUpTheatricsPlayer : MonoBehaviour
{
    public enum DoneStates
    {
        TimelineAndEnter = 0,
        TimelineOnly = 1,
    }

    [SerializeField] private DoneStates DoneCondition;
    [SerializeField] private bool isDone;
    [SerializeField] private Script_ItemPickUpTheatric _theatric;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineAsset myTimeline;
    [SerializeField] private bool isTimelineDone;
    [SerializeField] private bool isEnterOrSpacePressed;
    [SerializeField] private Script_BgThemePlayer bgThemePlayer;
    private bool isDetectingEnter;

    public Script_ItemPickUpTheatric Theatric
    {
        get => _theatric;
    }

    void OnEnable()
    {
        director.stopped += ItemPickUpTheatricsDone;
    }

    void OnDisable()
    {
        director.stopped -= ItemPickUpTheatricsDone;
    }

    void Update()
    {
        // track enter or space
        if (
            isDetectingEnter &&
            (Input.GetButtonDown(Const_KeyCodes.Submit) || Input.GetButtonDown(Const_KeyCodes.Action1))
        )
        {
            Debug.Log("Detected enter or space!!!!!");    
            isEnterOrSpacePressed = true;
            isDetectingEnter = false;
        }
    }

    void LateUpdate()
    {
        if (isDone)     return;
        
        switch (DoneCondition)
        {
            case (DoneStates.TimelineAndEnter):
                if (isTimelineDone && isEnterOrSpacePressed)    Done();
                break;
            case (DoneStates.TimelineOnly):
                if (isTimelineDone)                             Done();
                break;
            default:
                break;
        }

        void Done()
        {
            Stop();
            FadeBgMusicIn();
            isDone = true;

            Debug.Log($"!!!Firing ItemPickUpTheatricDone Done for Director {director}, this: {this}");
            Script_ItemsEventsManager.ItemPickUpTheatricDone(this);
        }

        void Stop()
        {
            Script_ItemPickUpTheatricsManager.Control.HideItemPickUpTheatric(Theatric);
        }

        void FadeBgMusicIn()
        {
            bgThemePlayer.FadeOutStop(() => Script_Game.Game.UnPauseBgMusic());
        }
    }
    
    public void Play()
    {
        if (bgThemePlayer != null)
        {
            Script_Game.Game.PauseBgMusic();
            bgThemePlayer.gameObject.SetActive(true);
        }
        
        /// Changes State to Cut Scene and will revert to previous state afterwards
        Debug.Log("Play(): Starting ItemPickUp Theatric!!!");

        isDone = false;
        isTimelineDone = false;

        Script_ItemPickUpTheatricsManager.Control.ShowItemPickUpTheatric(Theatric);
        
        Debug.Log($"Playing myTimeline asset: {myTimeline}");
        director.Play(myTimeline);
    }

    private void ItemPickUpTheatricsDone(PlayableDirector aDirector)
    {
        if (aDirector.playableAsset == myTimeline)
        {
            isTimelineDone = true;
            
            if (DoneCondition == DoneStates.TimelineAndEnter)   isDetectingEnter = true;
        }        
    }
}
