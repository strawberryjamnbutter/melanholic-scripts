﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Store the run by idx
/// </summary>
public class Script_RunsManager : MonoBehaviour
{
    public enum Cycle
    {
        Weekend = 0,
        Weekday = 1,
    }
    
    [SerializeField] private Script_Run.DayId _startDay;
    [SerializeField] private Script_Run.DayId _weekendStartDay;
    
    [Tooltip("Current run index")] [SerializeField] private int _runIdx;
    [SerializeField] private int cycleCount;
    
    [SerializeField] private Cycle _runCycle;
    [SerializeField] private Cycle _nextRunCycle;
    [SerializeField] private Script_Run[] weekdayCycle;
    [SerializeField] private Script_Run[] weekendCycle;
    [SerializeField] private Script_Run[] all;

    [Space]

    [SerializeField] private Script_DaysTextContainer[] daysText = new Script_DaysTextContainer[3];
    
    [SerializeField] private Script_Game game;
    [SerializeField] private Script_EventCycleManager eventCycleManager;

    public int RunIdx
    {
        get => _runIdx;      
        private set
        {
            _runIdx = value;

            // Update UI
            HandleDaysCanvas();
        }
    }
    
    public Script_Run Run
    {
        get => all[RunIdx];
    }

    public Cycle RunCycle
    {
        get => _runCycle;
        private set => _runCycle = value;
    }

    public Script_Run[] Days
    {
        get => RunCycle == Cycle.Weekday ? weekdayCycle : weekendCycle;
    }

    public int CycleCount
    {
        get => cycleCount;
    }

    private Script_Run.DayId StartDay
    {
        get => _startDay;
        set => _startDay = value;
    }

    private Script_Run.DayId WeekendStartDay
    {
        get => _weekendStartDay;
    }
    
    public Script_Run GetRunByIdx(int i)
    {
        return all[i];
    }

    public int GetRunIdxByDayId(Script_Run.DayId dayId)
    {
        int idx = -1;
        
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].dayId == dayId) idx = i;
        }

        return idx;
    }

    public int IncrementRun()
    {
        switch (RunCycle)
        {
            case (Cycle.Weekend):
                RunIdx = IncrementRunIdxInCycle(RunIdx, weekendCycle);
                break;
            default: // (Cycle.Weekday)
                RunIdx = IncrementRunIdxInCycle(RunIdx, weekdayCycle);
                break;
        }

        return RunIdx;
    }

    public void SetRun(Script_Run.DayId dayId)
    {
        RunIdx = GetRunIdxByDayId(dayId);
        RunCycle = GetCycleByRunIds(RunIdx);
    }

    private int IncrementRunIdxInCycle(int runIdx, Script_Run[] cycle)
    {
        int newRunIdx           = runIdx;
        int cycleIdx            = 0;

        // translate to cycle idx
        for (int i = 0; i < cycle.Length; i++)
            if (cycle[i] == all[runIdx])    cycleIdx = i;
        
        cycleIdx++;
        eventCycleManager.EndOfDayJobs();
        
        if (cycleIdx >= cycle.Length)
        {
            ClearEventCycle();
            
            cycleIdx = 0;
            cycleCount++;
        }

        Script_Run newRun = cycle[cycleIdx];

        // translate back to all idx
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] == newRun)
            {
                newRunIdx = i;
                break;
            }
        }

        return newRunIdx;
    }

    public void StartWeekendCycle()
    {
        RunCycle = Cycle.Weekend;
        RunIdx = GetRunIdxByDayId(WeekendStartDay);
        
        StartDay = WeekendStartDay;

        cycleCount = 0;
    }

    public void StartWeekdayCycle()
    {
        RunCycle = Cycle.Weekday;
        RunIdx = 0;

        StartDay = GetRunByIdx(RunIdx).dayId;

        cycleCount = 0;
    }

    private void ClearEventCycle()
    {
        eventCycleManager.InitialState();
    }

    public Cycle GetCycleByRunIds(int i)
    {
        Script_Run run = all[i];

        foreach (Script_Run weekdayRun in weekdayCycle)
        {
            if (weekdayRun == run)  return Cycle.Weekday;
        }

        return Cycle.Weekend;
    }

    // ------------------------------------------------------------------
    // UI

    public void HandleDaysCanvas()
    {
        for (int i = 0; i < daysText.Length; i++)
        {
            if (i >= daysText.Length || i >= Days.Length)
                return;

            Script_DaysTextContainer dtContainer = daysText[i];
            
            dtContainer.Text = Days[i].displayName;
            dtContainer.IsCurrentDay = Days[i] == Run;
        }
    }

    // ------------------------------------------------------------------
    // Init

    public void Initialize()
    {
        // RunCycle = Cycle.Weekday;
        RunIdx = GetRunIdxByDayId(StartDay);
    }
    
    public void InitialState(int idx, Cycle cycle)
    {
        RunCycle = cycle;
        RunIdx = idx;

        ClearEventCycle();
    }

    public void Load(
        int idx,
        int _cycleCount
    )
    {
        Cycle cycle = GetCycleByRunIds(idx);
        cycleCount = _cycleCount;
        
        InitialState(idx, cycle);
    }

    public void Setup()
    {
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Script_RunsManager))]
public class Script_RunsManagerTester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_RunsManager t = (Script_RunsManager)target;
        if (GUILayout.Button("Exit via Last Elevator Increment"))
        {
            Script_Game game = Script_Game.Game;
            
            Model_Exit exitData = new Model_Exit(
                game.level,
                game.GetPlayer().transform.position,
                game.GetPlayer().FacingDirection
            );
            
            game.ElevatorCloseDoorsCutScene(
                null,
                null,
                Script_Elevator.Types.Last,
                exitData,
                Script_Exits.ExitType.SaveAndRestartOnLevel
            );
        }
        
        if (GUILayout.Button("IncrementRun()"))
        {
            t.IncrementRun();
        }

        if (GUILayout.Button("Print Cycle By Run Idx"))
        {
            Debug.Log($"mon {t.GetCycleByRunIds(0)}");
            Debug.Log($"tue {t.GetCycleByRunIds(1)}");
            Debug.Log($"wed {t.GetCycleByRunIds(2)}");
            Debug.Log($"thu {t.GetCycleByRunIds(3)}");
            Debug.Log($"fri {t.GetCycleByRunIds(4)}");
            Debug.Log($"sat {t.GetCycleByRunIds(5)}");
            Debug.Log($"sun {t.GetCycleByRunIds(6)}");
        }

        GUILayout.Space(12);

        if (GUILayout.Button("Handle Days Container Highlight"))
        {
            t.HandleDaysCanvas();
        }

        GUILayout.Space(12);

        if (GUILayout.Button("Weekday Start Setup"))
        {
            t.StartWeekdayCycle();
        }
        
        if (GUILayout.Button("Weekend Start Setup"))
        {
            t.StartWeekendCycle();
        }
    }
}
#endif