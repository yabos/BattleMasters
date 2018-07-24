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
    protected Hero_Control m_Owner;

    public virtual void Initialize(Hero_Control owner)
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
}
