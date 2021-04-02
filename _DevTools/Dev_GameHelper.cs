﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Dev_GameHelper : MonoBehaviour
{
    public Vector3Int playerSpawn;
    public int level;
    public Directions facingDirection;
    [SerializeField] private Script_ExitMetadataObject playerDefaultSpawn;
    [SerializeField] private Script_ExitMetadataObject playerTeleportPos;
    [SerializeField] private Script_ExitMetadataObject IdsRoomEntrance;
    [SerializeField] private Script_ExitMetadataObject LastElevatorEntrance;

    public void DefaultPlayerSpawnPos()
    {
        playerSpawn = new Vector3Int(
            (int)playerDefaultSpawn.data.playerSpawn.x,
            (int)playerDefaultSpawn.data.playerSpawn.y,
            (int)playerDefaultSpawn.data.playerSpawn.z
        );
        level           = playerDefaultSpawn.data.level;
        facingDirection = playerDefaultSpawn.data.facingDirection;
    }

    public void ExitToLevel()
    {
        Teleport(playerTeleportPos);
    }

    public void ExitToIdsRoom()
    {
        Teleport(IdsRoomEntrance);
    }

    public void ExitToLastElevator()
    {
        Teleport(IdsRoomEntrance);
    }

    public void BuildSetup()
    {
        Script_Game.LevelsInactivate();
    }

    public void SetAllQuestsDoneToday()
    {
        Script_Game.Game.IdsRoomBehavior.isCurrentPuzzleComplete            = true;
        Script_Game.Game.KTVRoom2Behavior.IsCurrentPuzzleComplete           = true;
        Script_Game.Game.ElleniasRoomBehavior.isCurrentPuzzleComplete       = true;
        Script_Game.Game.EileensMindBehavior.isCurrentPuzzleComplete        = true;
        Script_Game.Game.WellsWorldBehavior.isCurrentMooseQuestComplete     = true;
        Script_Game.Game.GardenLabyrinthBehavior.isCurrentPuzzleComplete    = true;
    }

    public void SolveAllMynesMirrors()
    {
        Script_ScarletCipherManager.Control.Dev_ForceSolveAllMirrors();
    }

    private void Teleport(Script_ExitMetadata exit)
    {
        Script_Game.Game.Exit(
            exit.data.level,
            exit.data.playerSpawn,
            exit.data.facingDirection,
            isExit: true,
            isSilent: false,
            exitType: Script_Exits.ExitType.Default
        );   
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Dev_GameHelper))]
public class Dev_GameHelperTester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Dev_GameHelper t = (Dev_GameHelper)target;
        if (GUILayout.Button("DefaultPlayerSpawnPos()"))
        {
            t.DefaultPlayerSpawnPos();
        }
        
        if (GUILayout.Button("Go To:"))
        {
            t.ExitToLevel();
        }

        if (GUILayout.Button("Go To: Ids Room"))
        {
            t.ExitToIdsRoom();
        }

        if (GUILayout.Button("Go To: Last Elevator"))
        {
            t.ExitToLastElevator();
        }

        GUILayout.Space(12);

        if (GUILayout.Button("All Quests Done Today"))
        {
            t.SetAllQuestsDoneToday();
        }

        if (GUILayout.Button("Solve All Mirrors"))
        {
            t.SolveAllMynesMirrors();
        }

        GUILayout.Space(12);

        if (GUILayout.Button("Build Setup"))
        {
            t.BuildSetup();
        }
    }
}
#endif