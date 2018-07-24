using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState
{
    protected BattleManager m_Owner;
    protected BattleStateManager m_StateManager;
    protected float m_fTimeElapsed;

    public virtual void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        m_Owner = owner;
        m_StateManager = state_manager;
    }

    public virtual void DoStart(byte[] data = null)
    {
        m_fTimeElapsed = 0.0f;
    }

    public virtual void DoEnd()
    {
    }

    public virtual void Update(float fTimeDelta)
    {
    }

    public virtual void OnScreenTouchDown()
    {
    }

    public virtual void OnScreenTouchUp()
    {
    }

    public void ReceiveEvent(BattleEvent sender)
    {
    }

    public virtual void NotifyDamage(Hero_Control damagedHero)
    {
    }

    public virtual void NotifyActiveSkill(Hero_Control battle_player, int iSkillSequence)
    {
    }
}
