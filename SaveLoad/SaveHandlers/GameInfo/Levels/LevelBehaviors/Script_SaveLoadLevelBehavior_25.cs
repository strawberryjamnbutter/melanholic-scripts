﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_SaveLoadLevelBehavior_25 : Script_SaveLoadLevelBehavior
{
    [SerializeField] private Script_LevelBehavior_25 LB25;

    public override void Save(Model_RunData data)
    {
        Model_LevelBehavior_25 lvlModel = new Model_LevelBehavior_25(
            _isPuzzleComplete: LB25.isPuzzleComplete,
            _spokenWithEllenia: LB25.spokenWithEllenia
        );
        
        data.levelsData.LB25 = lvlModel;
    }

    public override void Load(Model_RunData data)
    {
        if (data.levelsData == null)
        {
            Debug.Log("There is no levels state data to load.");
            return;
        }

        if (data.levelsData.LB25 == null)
        {
            Debug.Log("There is no LB25 state data to load.");
            return;
        }

        Model_LevelBehavior_25 lvlModel = data.levelsData.LB25;
        LB25.isPuzzleComplete           = lvlModel.isPuzzleComplete;
        LB25.spokenWithEllenia          = lvlModel.spokenWithEllenia;

        Debug.Log($"-------- LOADED {name} --------");
        Script_Utils.DebugToConsole(lvlModel);
    }
}
