using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionDie : HeroBattleAction
{
    public override void Initialize(Hero_Control owner, HeroBattleActionManager action_manager)
    {
        base.Initialize(owner, action_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart(data);

        m_Owner.PlayAnimation(Actor.AniType.ANI_DEFEAT);
        m_Owner.HeroDie();
    }

    public override void DoEnd(EHeroBattleAction eNextAction)
    {
        base.DoEnd(eNextAction);
    }
}