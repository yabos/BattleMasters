using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionTraceAtk : HeroBattleAction
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

    bool isMove = false;
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
            //  그냥 되돌아 오는 모션을 애니메이션 커브로 해도 될 듯
            isMove = moveSomething(m_Owner.gameObject, m_Owner.InitPos); // change room bool

            // move GameObject to GameObject marker
            //if (isMove == false)
            m_Owner.PlayAnimation(Actor.AnimationActor.ANI_IDLE);
            if (timeTaken > 6)
            {
                m_Owner.ChangeState(EHeroBattleAction.HEROSTATE_IDLE);
                m_Owner.transform.localScale = Vector3.one;
            }
        }
    }

    float timeTaken;

    public bool moveSomething(GameObject start, Vector3 end)
    { 
        // return bool when finished moving
        if (start.transform.position == end)
        {
            return false;
        }
        else
        {
            timeTaken += (float)move(start, end);
            return true;
        }
    }
    public float move(GameObject start, Vector3 end)
    {
        start.transform.position = Vector3.Lerp(start.transform.position, end, Time.deltaTime * 10);
        start.transform.localScale = Vector3.Lerp(start.transform.localScale, Vector3.one, Time.deltaTime * 7);
        return Time.deltaTime * 10;
    }
}
