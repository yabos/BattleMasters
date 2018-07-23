using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHeroBattleAction
{
    HEROSTATE_IDLE = 0,
    HEROSTATE_TRACE_ATK,
    HEROSTATE_CNT_ATK,
    HEROSTATE_FAKE_ATK,
    HEROSTATE_FAKE_DEFEAT,
    HEROSTATE_BREAK_DEFEAT,
    HEROSTATE_CNT_DEFEAT,
    HEROSTATE_ATK_DEFEAT,
    HEROSTATE_DEFEAT_ATK,
    HEROSTATE_WIN,
    HEROSTATE_LOSE,
    HEROSTATE_DIE,
    HERO_BTL_ACT_MAX,
}

public class HeroBattleActionManager
{
    protected HeroBattleAction[] m_Actions = new HeroBattleAction[(int)EHeroBattleAction.HERO_BTL_ACT_MAX]
    {
        new HeroBattleActionIdle(),
        new HeroBattleActionTraceAtk(),
        new HeroBattleActionCntAtk(),
        new HeroBattleActionFakeAtk(),
        new HeroBattleActionFakeDefeat(),
        new HeroBattleActionBreakDefeat(),
        new HeroBattleActionCntDefeat(),
        new HeroBattleActionAtkDefeat(),
        new HeroBattleActionDefeatAtk(),
        new HeroBattleActionWin(),
        new HeroBattleActionLose(),
        new HeroBattleActionDie(),
    };

    protected EHeroBattleAction m_eCurrentAction;
    protected EHeroBattleAction m_ePreviousAction;
    protected Hero_Control m_Owner;

    public virtual void Initialize(Hero_Control owner)
    {
        m_ePreviousAction = m_eCurrentAction = EHeroBattleAction.HEROSTATE_IDLE;

        m_Owner = owner;
        
        for (int i = (int)EHeroBattleAction.HEROSTATE_IDLE; i < (int)EHeroBattleAction.HERO_BTL_ACT_MAX; i++)
        {
            m_Actions[i].Initialize(m_Owner, this);
        }

        m_Actions[(int)m_eCurrentAction].DoStart();
    }

    public virtual void Release()
    {
        for (int i = (int)EHeroBattleAction.HEROSTATE_IDLE; i < (int)EHeroBattleAction.HERO_BTL_ACT_MAX; i++)
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
