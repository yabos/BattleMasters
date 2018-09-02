using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHeroBattleAction
{
    HeroAction_Idle = 0,
    HeroAction_AtkWin,
    HeroAction_CntWin,
    HeroAction_FakeWin,
    HeroAction_AtkDefeat,
    HeroAction_CntDefeat,
    HeroAction_FakeDefeat,
    HeroAction_DrawAtkDefeat,
    HeroAction_DrawDefeatAtk,
    HeroAction_BattleWin,
    HeroAction_BattleLose,
    HeroAction_BattleDie,
    HeroAction_Max,
}

public class HeroBattleActionManager
{
    protected HeroBattleAction[] m_Actions = new HeroBattleAction[(int)EHeroBattleAction.HeroAction_Max]
    {
        new HeroBattleActionIdle(),
        new HeroBattleActionAtkWin(),
        new HeroBattleActionCntWin(),
        new HeroBattleActionFakeWin(),
        new HeroBattleActionAtkDefeat(),
        new HeroBattleActionCntDefeat(),
        new HeroBattleActionFakeDefeat(),        
        new HeroBattleActionDrawAtkDefeat(),
        new HeroBattleActionDrawDefeatAtk(),
        new HeroBattleActionWin(),
        new HeroBattleActionLose(),
        new HeroBattleActionDie(),
    };

    protected EHeroBattleAction m_eCurrentAction;
    protected EHeroBattleAction m_ePreviousAction;
    protected BattleHero m_Owner;

    public virtual void Initialize(BattleHero owner)
    {
        m_ePreviousAction = m_eCurrentAction = EHeroBattleAction.HeroAction_Idle;

        m_Owner = owner;
        
        for (int i = (int)EHeroBattleAction.HeroAction_Idle; i < (int)EHeroBattleAction.HeroAction_Max; i++)
        {
            m_Actions[i].Initialize(m_Owner, this);
        }

        m_Actions[(int)m_eCurrentAction].DoStart();
    }

    public virtual void Release()
    {
        for (int i = (int)EHeroBattleAction.HeroAction_Idle; i < (int)EHeroBattleAction.HeroAction_Max; i++)
        {
            m_Actions[i].Release();
        }
    }

    public virtual void Update(float fTimeDelta)
    {
        m_Actions[(int)m_eCurrentAction].Update(fTimeDelta);
    }
    
    //public virtual void OnTriggerEnter(Collider other)
    //{
    //    m_Actions[(int)m_eCurrentAction].OnTriggerEnter(other);
    //}

    //public virtual void OnCollisionActor(Actor target_actor)
    //{
    //    m_Actions[(int)m_eCurrentAction].OnCollisionActor(target_actor);
    //}   

    public virtual void NotifyDamage(int iDamageValue)
    {
        m_Actions[(int)m_eCurrentAction].NotifyDamage(iDamageValue);
    }

    public virtual void ChangeAction(EHeroBattleAction eAction, byte[] data = null, bool bRefresh = false)
    {
        if (m_eCurrentAction != eAction || bRefresh == true)
        {
            m_ePreviousAction = m_eCurrentAction;
            m_Actions[(int)m_ePreviousAction].DoEnd(eAction);

            m_eCurrentAction = eAction;
            m_Actions[(int)m_eCurrentAction].DoStart(data);
        }
    }

    public virtual void ActionRemind(EHeroBattleAction eAction, HeroBattleAction action)
    {
        m_Actions[(int)eAction] = action;
    }

    public EHeroBattleAction GetCurrentAction()
    {
        return m_eCurrentAction;
    }

    public EHeroBattleAction GetPreviousAction()
    {
        return m_ePreviousAction;
    }

    public virtual HeroBattleAction GetAction(EHeroBattleAction eAction)
    {
        return m_Actions[(int)eAction];
    }
    
    public IEnumerator AnimationDelay(params object [] list)
    {
        float delay = System.Convert.ToSingle(list[0]);
        float dummy = System.Convert.ToSingle(list[1]);
        string aniType = System.Convert.ToString(list[2]);

        Actor.AniType eAniType = m_Owner.GetAniType(aniType);
        m_Owner.PlayAnimation(eAniType);

        yield return new WaitForSeconds(delay);
    }

    public IEnumerator MoveForward(params object[] list)
    {
        float duration = System.Convert.ToSingle(list[0]);
        float dist = System.Convert.ToSingle(list[1]);
        string aniType = System.Convert.ToString(list[2]);
        Actor.AniType eAniType = m_Owner.GetAniType(aniType);

        float ElapsedTime = 0;
        float SumX = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;
            Vector3 vPos = m_Owner.transform.position;
            float tickX = (Time.deltaTime / duration) * dist;
            SumX += tickX;
            if (SumX >= dist)
            {
                tickX = 0;
            }

            if (m_Owner.IsMyTeam == false)
            {
                tickX *= -1;
            }

            vPos.x += tickX;
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(eAniType);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveForwardMoment(params object[] list)
    {
        float duration = System.Convert.ToSingle(list[0]);
        float dist = System.Convert.ToSingle(list[1]);        
        string aniType = System.Convert.ToString(list[2]);
        Actor.AniType eAniType = m_Owner.GetAniType(aniType);

        if (m_Owner.IsMyTeam == false)
        {
            dist *= -1;
        }

        Vector3 vPos = m_Owner.transform.position;
        vPos.x += dist;
        m_Owner.transform.position = vPos;

        m_Owner.PlayAnimation(eAniType);
        yield return new WaitForSeconds(duration);
    }

    public IEnumerator MoveBackward(params object[] list)
    {
        float duration = System.Convert.ToSingle(list[0]);
        float dist = System.Convert.ToSingle(list[1]);
        string aniType = System.Convert.ToString(list[2]);
        Actor.AniType eAniType = m_Owner.GetAniType(aniType);

        float ElapsedTime = 0;
        float SumX = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;
            Vector3 vPos = m_Owner.transform.position;
            float tickX = (Time.deltaTime / duration) * dist;
            SumX += tickX;
            if (SumX >= dist)
            {
                tickX = 0;
            }

            if (m_Owner.IsMyTeam)
            {
                tickX *= -1;
            }

            vPos.x += tickX;
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(eAniType);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveBackwardMoment(params object[] list)
    {
        float duration = System.Convert.ToSingle(list[0]);
        float dist = System.Convert.ToSingle(list[1]);
        string aniType = System.Convert.ToString(list[2]);
        Actor.AniType eAniType = m_Owner.GetAniType(aniType);

        if (m_Owner.IsMyTeam)
        {
            dist *= -1;
        }

        Vector3 vPos = m_Owner.transform.position;
        vPos.x += dist;
        m_Owner.transform.position = vPos;

        m_Owner.PlayAnimation(eAniType);
        yield return new WaitForSeconds(duration);
    }

    public IEnumerator FadeOut(params object[] list)
    {
        float duration = System.Convert.ToSingle(list[0]);
        float dummy = System.Convert.ToSingle(list[1]);
        string aniType = System.Convert.ToString(list[2]);        
        Actor.AniType eAniType = m_Owner.GetAniType(aniType);

        m_Owner.PlayAnimation(eAniType);
        yield return m_Owner.HeroAlphaFade(duration);
    }
}
