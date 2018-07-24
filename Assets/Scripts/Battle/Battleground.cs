using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattlePosType
{
    BPT_ATK_FAKE,
    BPT_CNT_ATK,
    BPT_FAKE_CNT,
    BPT_DRAW,
}

public class Battleground : MonoBehaviour
{
    public Transform[] BattleRegenPosMyTeam = new Transform[4];
    public Transform[] BattleRegenPosEnemy = new Transform[4];

    public Transform[] BattleActionPosMyTeam = new Transform[4];
    public Transform[] BattleActionPosEnemy = new Transform[4];   

    public Vector3 GetTeamPos(EHeroBattleAction stateAction, bool myTeam)
    {
        if (stateAction == EHeroBattleAction.HeroAction_AtkWin ||
            stateAction == EHeroBattleAction.HeroAction_AtkDefeat)
        {
            if (myTeam)
            {
                return BattleActionPosMyTeam[(int)BattlePosType.BPT_ATK_FAKE].position;
            }
            else
            {
                return BattleActionPosEnemy[(int)BattlePosType.BPT_ATK_FAKE].position;
            }
        }
        else if (stateAction == EHeroBattleAction.HeroAction_CntWin ||
                 stateAction == EHeroBattleAction.HeroAction_CntDefeat)
        {
            if (myTeam)
            {
                return BattleActionPosMyTeam[(int)BattlePosType.BPT_CNT_ATK].position;
            }
            else
            {
                return BattleActionPosEnemy[(int)BattlePosType.BPT_CNT_ATK].position;
            }
        }
        else if (stateAction == EHeroBattleAction.HeroAction_FakeWin ||
                 stateAction == EHeroBattleAction.HeroAction_FakeDefeat)
        {
            if (myTeam)
            {
                return BattleActionPosMyTeam[(int)BattlePosType.BPT_FAKE_CNT].position;
            }
            else
            {
                return BattleActionPosEnemy[(int)BattlePosType.BPT_FAKE_CNT].position;
            }
        }
        else
        {
            if (myTeam)
            {
                return BattleActionPosMyTeam[(int)BattlePosType.BPT_DRAW].position;
            }
            else
            {
                return BattleActionPosEnemy[(int)BattlePosType.BPT_DRAW].position;
            }
        }
    }
}
