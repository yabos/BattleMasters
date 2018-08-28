using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBattleState
{
    BattleState_Load,
    BattleState_Ready,
    BattleState_Normal,
    BattleState_Action,
    BattleState_Win,
    BattleState_Lose,
    BattleState_End,
    BattleState_Max,
}

public class BattleStateManager
{
    protected BattleState[] m_States = new BattleState[(int)EBattleState.BattleState_Max]
    {
        new BattleStateLoad(),
        new BattleStateReady(),
        new BattleStateNormal(),
        new BattleStateAction(),
        new BattleStateWin(),
        new BattleStateLose(),
        new BattleStateEnd(),
    };

    protected EBattleState m_eCurrentState;
    protected EBattleState m_ePreviousState;

    protected BattleScene m_Owner;

    public void Initialize(BattleScene owner)
    {
        m_Owner = owner;

        m_ePreviousState = m_eCurrentState = EBattleState.BattleState_Load;

        for (int i = (int)EBattleState.BattleState_Load; i < (int)EBattleState.BattleState_Max; i++)
        {
            m_States[i].Initialize(m_Owner, this);
        }

        m_States[(int)m_eCurrentState].DoStart();
    }
   
    public void Update(float fTimeDelta)
    {
        m_States[(int)m_eCurrentState].Update(fTimeDelta);
    }

    public void OnScreenTouchDown()
    {
        m_States[(int)m_eCurrentState].OnScreenTouchDown();
    }

    public void OnScreenTouchUp()
    {
        m_States[(int)m_eCurrentState].OnScreenTouchUp();
    }

    public void ChangeState(EBattleState eNextState, byte[] data = null, bool bRefresh = false)
    {
        if (m_eCurrentState != eNextState || bRefresh == true)
        {
            m_ePreviousState = m_eCurrentState;
            m_States[(int)m_ePreviousState].DoEnd();

            m_eCurrentState = eNextState;
            m_States[(int)m_eCurrentState].DoStart(data);
        }
    }    

    public void ReceiveEvent(BattleEvent sender)
    {
        m_States[(int)m_eCurrentState].ReceiveEvent(sender);
    }

    public virtual void NotifyDamage(Hero damagedHero)
    {
        m_States[(int)m_eCurrentState].NotifyDamage(damagedHero);
    }    

    public virtual void NotifyActiveSkill(Hero battle_player, int iSkillSequence)
    {
        m_States[(int)m_eCurrentState].NotifyActiveSkill(battle_player, iSkillSequence);
    }
}