using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionFakeDefeat : HeroBattleAction
{
    protected float m_fTimeElapsed;

    public override void Initialize(Hero_Control owner, HeroBattleActionManager action_manager)
    {
        base.Initialize(owner, action_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart(data);

        m_Owner.PlayAnimation(Actor.AnimationActor.ANI_FAKE);

        m_fTimeElapsed = 0.0f;
    }

    public override void DoEnd(EHeroBattleAction eNextAction)
    {
        base.DoEnd(eNextAction);
    }

    public override void Update(float fTimeDelta)
    {
        base.Update(fTimeDelta);

        m_fTimeElapsed += fTimeDelta;

        if (m_fTimeElapsed <= 0.5f)
        {            
            m_Owner.PlayAnimation(Actor.AnimationActor.ANI_FAKE);
        }
        else if (m_fTimeElapsed > 0.5f && m_fTimeElapsed <= 1f)
        {
            m_Owner.PlayAnimation(Actor.AnimationActor.ANI_DEFEAT);
        }
        else if (m_fTimeElapsed > 1f)
        {
            m_Owner.ChangeState(EHeroBattleAction.HeroAction_Idle);
        }
    }
}
