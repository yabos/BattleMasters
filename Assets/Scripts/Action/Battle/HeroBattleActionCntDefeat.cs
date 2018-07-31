using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionCntDefeat : HeroBattleAction
{
    public override void Initialize(Hero_Control owner, HeroBattleActionManager action_manager)
    {
        base.Initialize(owner, action_manager);
        ReadCommend(EActionCommend.COMMEND_CNT_DEFEAT);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart(data);

        UtilFunc.ChangeLayersRecursively(m_Owner.transform, "UI");

        m_Owner.StartCoroutine(ActionProc());
    }

    public override void DoEnd(EHeroBattleAction eNextAction)
    {
        base.DoEnd(eNextAction);

        m_Owner.StopCoroutine(ActionProc());
    }
}
