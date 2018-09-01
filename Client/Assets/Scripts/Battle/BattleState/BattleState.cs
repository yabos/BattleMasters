using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState
{
    protected BattleScene m_Owner;
    protected BattleStateManager m_StateManager;
    protected float TimeElapsed;

    public virtual void Initialize(BattleScene owner, BattleStateManager state_manager)
    {
        m_Owner = owner;
        m_StateManager = state_manager;
    }

    public virtual IEnumerator DoStart(byte[] data = null)
    {
        TimeElapsed = 0.0f;
        yield return null;
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

    //public void ReceiveEvent(BattleEvent sender)
    //{
    //}

    public virtual void NotifyDamage(Hero damagedHero)
    {
    }

    public virtual void NotifyActiveSkill(Hero battle_player, int iSkillSequence)
    {
    }
}
