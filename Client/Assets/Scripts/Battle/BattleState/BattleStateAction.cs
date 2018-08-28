using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateAction : BattleState
{
    bool IsTurnOut = false;

    public override void Initialize(BattleScene owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart();

        if (data != null)
        {
            int place = 0;
            IsTurnOut = System.BitConverter.ToBoolean(data, place);
        }
        else
        {
            IsTurnOut = false;
        }

        TimeElapsed = 0;

        BattleManager.Instance.SetBattleStateActionStart(IsTurnOut);
    }

    public override void DoEnd()
    {
        base.DoEnd();

        BattleManager.Instance.SetBattleStateActionEnd();
    }

    public override void Update(float fTimeDelta)
    {
        TimeElapsed += fTimeDelta;

        // 행동중인 영웅이 없는지 체크
        if (BattleManager.Instance.CheckAction() == false)
        {
            if (BattleManager.Instance.IsMyTeamAllDie())
            {
                m_StateManager.ChangeState(EBattleState.BattleState_Lose);
            }
            else if (BattleManager.Instance.IsEnemyAllDie())
            {
                m_StateManager.ChangeState(EBattleState.BattleState_Win);
            }
            else
            {
                m_StateManager.ChangeState(EBattleState.BattleState_Ready);
            }
        }
    }
}
