﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Script_TimelineController))]
public class Script_LevelBehavior_22 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool isUrsieCutsceneDone;
    /* ======================================================================= */
    
    [SerializeField] private Script_TrackedPushablesTriggerPuzzleController puzzleController;
    [SerializeField] private Script_LevelBehavior_24 LB24;

    [SerializeField] private Script_TileMapExitEntrance ktvRoomExit;

    [SerializeField] private Script_DemonNPC Ursie;
    [SerializeField] private Script_DemonNPC PecheMelba;
    [SerializeField] private Script_InteractableFullArt thankYouNote;

    [SerializeField] private Script_DialogueNode UrsieAfterUnlockNode;
    [SerializeField] private Script_DialogueNode[] psychicNodesQuestActive;
    [SerializeField] private Script_DialogueNode[] psychicNodesTalked;

    [SerializeField] private Script_DoorLock KTVRoomDoorCage;

    [SerializeField] private float OnUnlockDoneWaitTimeForDialogue;

    private bool spokenWithUrsie;
    private bool isInit = true;

    private void Awake()
    {
        ktvRoomExit.IsDisabled = true;
        
        // Setup puzzle rooms
        puzzleController.Setup();
        
        /// Set completion state after set up to ensure no race conditions and we're not
        /// resetting trackables to default position in their Setup
        if (LB24.IsCurrentPuzzleComplete)   LB24.PuzzleFinishedState();
    }

    // ------------------------------------------------------------------
    // Next Node Actions

    public void OnDeclinedUrsieQuest()
    {
        if (spokenWithUrsie)    return;
        
        Debug.Log("OnDeclinedUrsieQuest() switching out Ursie's dialogue nodes now**********");
        Ursie.SwitchPsychicNodes(psychicNodesTalked);

        spokenWithUrsie = true;
    }
    
    public void StartUnlockKTVRoomCutScene()
    {
        game.ChangeStateCutScene();

        // Remove Ursie's Bar interaction boxes so they aren't blocking the KTV Door.
        Ursie.RemoveExtraInteractableBoxes();
        
        // Play Timeline to unnlock KTV Room2
        GetComponent<Script_TimelineController>().PlayableDirectorPlayFromTimelines(0, 0);
    }

    public void OnUnlockCutSceneDone()
    {
        game.ChangeStateInteract();
        Ursie.SwitchPsychicNodes(psychicNodesQuestActive);
    }

    // Preaction when talking with Peche & Melba
    public void UpdateUrsieName()
    {
        Script_Names.UpdateUrsie();
    }

    // ------------------------------------------------------------------
    // Full Art Actions

    public void UpdateNamesInLetter()
    {
        Script_Names.UpdateMelba();
        Script_Names.UpdatePeche();
        Script_Names.UpdateUrsie();
    }

    // ------------------------------------------------------------------
    // Timeline Signals

    public void UnlockKTVRoom()
    {
        KTVRoomDoorCage.Unlock();
        ktvRoomExit.IsDisabled = false;
    }

    public void OnUnlockKTVRoomDone()
    {
        Ursie.FaceDirection(Directions.Right);
        Ursie.DefaultFacingDirection = Directions.Right;
        StartCoroutine(WaitForUrsieDialogue());

        IEnumerator WaitForUrsieDialogue()
        {
            yield return new WaitForSeconds(OnUnlockDoneWaitTimeForDialogue);
            Script_DialogueManager.DialogueManager.StartDialogueNode(UrsieAfterUnlockNode);
        }
    }

    // ------------------------------------------------------------------

    // If you complete the quest Ursie and MelbaPeche will have left, leaving a note.
    public void PuzzleCompleteState()
    {
        Ursie.gameObject.SetActive(false);
        PecheMelba.gameObject.SetActive(false);

        thankYouNote.gameObject.SetActive(true);
    }
    
    public override void InitialState()
    {
        base.InitialState();

        Ursie.gameObject.SetActive(true);
        PecheMelba.gameObject.SetActive(true);

        thankYouNote.gameObject.SetActive(false);
    }

    
    public override void Setup()
    {
        if (FinishedQuest())    PuzzleCompleteState();   
        else                    InitialState();

        bool FinishedQuest()
        {
            return LB24.IsCurrentPuzzleComplete && !isUrsieCutsceneDone;
        }
        
        isInit = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Script_LevelBehavior_22))]
public class Script_LevelBehavior_22Tester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_LevelBehavior_22 lb = (Script_LevelBehavior_22)target;
        if (GUILayout.Button("PuzzleCompleteState()"))
        {
            lb.PuzzleCompleteState();
        }

        if (GUILayout.Button("Unlock KTV Door"))
        {
            lb.UnlockKTVRoom();
        }
    }
}
#endif