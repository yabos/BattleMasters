using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionIdle : HeroBattleAction
{
    public override void Initialize(Hero_Control owner, HeroBattleActionManager action_manager)
    {
        base.Initialize(owner, action_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart(data);

        m_Owner.PlayAnimation(Actor.AniType.ANI_IDLE);        

        UtilFunc.ChangeLayersRecursively(m_Owner.transform, "Default");
    }

    public override void DoEnd(EHeroBattleAction eNextAction)
    {
        base.DoEnd(eNextAction);

        if (IsMovingAction(eNextAction))
        {
            // activeMoving 변수 만들고 셋팅 
            // 추가로 myTurn 관련 부분 내 턴이 끝났을 때 처리하는 부분 추가
        }
    }
}
