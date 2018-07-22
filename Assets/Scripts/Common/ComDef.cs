using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public	class ComDef
{
    public enum GameState
    { 
        GAMESTATE_TITLE = 0,
        GAMESTATE_LOBBY,
        GAMESTATE_BATTLE
    }
}

public class SaveData
{
    public int iHeroNo;
}

public class Define
{
    public static readonly float TURN_MAX = 10000;
    public static readonly float TURNICON_START_POS_X = -1000;
    public static readonly float TURNICON_END_POS_X = 1000;
    public static readonly float TURNICON_POS_X_LENGTH = 2000;

    public static readonly float SELECT_TARGET_LIMITTIME = 3;
    public static readonly float SELECT_ACTIONTYPE_LIMITTIME = 5;

    public static readonly float TRACE_SPEED = 2;
}