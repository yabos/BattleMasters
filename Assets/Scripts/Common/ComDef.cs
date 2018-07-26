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
    //  턴 종료까지 시간 1000 = 1초
    public static readonly float TURN_MAX = 10000;
    public static readonly float TURNICON_START_POS_X = -1000;
    public static readonly float TURNICON_END_POS_X = 1000;
    public static readonly float TURNICON_POS_X_LENGTH = 2000;

    // 적 선택까지 제한시간
    public static readonly float SELECT_TARGET_LIMITTIME = 10;
    // 공격 타입 선택까지 제한시간
    public static readonly float SELECT_ACTIONTYPE_LIMITTIME = 5;

    // 트레이스시에 가는 거리 정도 (이 값만큼 더해짐)
    public static readonly float MOVE_TRACE_SPEED_X = 0.15f;

    // 전투모드시에 캐릭터 확대 정도.
    public static readonly float BATTLE_MOD_SCALE = 1.3f;
}

public class ResourcePath
{
    public static readonly string BattleRootPath = "Battle/Prefabs/Battle_Root";
    public static readonly string MapLoadPath = "Map/";
}