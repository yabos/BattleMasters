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

    public override IEnumerator DoStart(byte[] data = null)
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

        m_Owner.SetBattleStateActionStart(IsTurnOut);

        yield return new WaitForEndOfFrame();
    }

    public override void DoEnd()
    {
        m_Owner.SetBattleStateActionEnd();

        base.DoEnd();
    }

    public override void Update(float fTimeDelta)
    {
        // 행동중인 영웅이 없는지 체크
        if (BattleHeroManager.Instance.CheckAction() == false)
        {
            if (BattleHeroManager.Instance.IsMyTeamAllDie())
            {
                m_StateManager.ChangeState(EBattleState.BattleState_Lose);
            }
            else if (BattleHeroManager.Instance.IsEnemyAllDie())
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
