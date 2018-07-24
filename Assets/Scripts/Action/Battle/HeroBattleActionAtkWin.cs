using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionAtkWin : HeroBattleAction
{
    protected float m_fTimeElapsed;

    public override void Initialize(Hero_Control owner, HeroBattleActionManager action_manager)
    {
        base.Initialize(owner, action_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart(data);

        m_Owner.PlayAnimation(Actor.AnimationActor.ANI_TRACE);

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
            Vector3 vPos = m_Owner.transform.position;
            vPos.x += m_Owner.MyTeam ? Define.TRACE_SPEED_X : (-1 * Define.TRACE_SPEED_X);
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(Actor.AnimationActor.ANI_TRACE);
        }
        else if (m_fTimeElapsed > 0.5f && m_fTimeElapsed <= 1f)
        {
            m_Owner.PlayAnimation(Actor.AnimationActor.ANI_ATK);
        }
        else if (m_fTimeElapsed > 1f)
        {
            var tp = m_Owner.gameObject.AddComponent<TweenPosition>();
            if (tp != null)
            {
                tp.style = UITweener.Style.Once;
                tp.duration = 0.3f;
                tp.from = m_Owner.transform.position;
                tp.to = m_Owner.InitPos;                
                tp.animationCurve = m_Owner.Actor.BackStepCurve;                
                tp.PlayForward();

                Object.Destroy(tp, tp.duration);
                m_Owner.ChangeState(EHeroBattleAction.HeroAction_Idle);
            }
        }
    }
}
