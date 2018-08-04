using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateAction : BattleState
{
    bool IsTurnOut = false;

    public override void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
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

        // 모두 다 행동을 끝냈으면 Ready 상태로 전이
        if (BattleManager.Instance.CheckAction() == false)
        { 
            m_StateManager.ChangeState(EBattleState.BattleState_Ready);
        }
    }
}
