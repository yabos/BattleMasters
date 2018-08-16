using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleAction
{
    protected Hero m_Owner;
    protected HeroBattleActionManager m_ActionManager;
    protected List<string> m_Commends;
    protected Vector3 m_veOriginalPos;

    public virtual void Initialize(Hero owner, HeroBattleActionManager action_manager)
    {
        m_Owner = owner;
        m_ActionManager = action_manager;
        m_Commends = new List<string>();
    }

    public virtual void Release()
    {

    }

    public virtual void DoStart(byte[] data = null)
    {
        if (IsMovingAction(m_ActionManager.GetCurrentAction()))
        {
            m_Owner.IsAction = true;
        }
    }

    public virtual void DoEnd(EHeroBattleAction eNextAction)
    {
        if (IsMovingAction(m_ActionManager.GetCurrentAction()) && eNextAction != EHeroBattleAction.HeroAction_BattleDie)
        {
            m_Owner.IsAction = false;
        }

        // idle Action을 제외하고 모든 행동 끝에는 targetHero를 초기화 시킨다.
        if (m_ActionManager.GetCurrentAction() != EHeroBattleAction.HeroAction_Idle)
        {
            m_Owner.BattleTargetHero = null;

            // 전투시 공격 승리자를 더 높게 설정했던 SortingOrder를 원래대로 초기화.
            m_Owner.SetDefaultSortingOrder();
        }
    }

    public virtual void Update(float fTimeDelta)
    {
    }

    protected bool IsMovingAction(EHeroBattleAction action)
    {
        if (action == EHeroBattleAction.HeroAction_AtkWin ||
            action == EHeroBattleAction.HeroAction_CntWin ||
            action == EHeroBattleAction.HeroAction_FakeWin ||
            action == EHeroBattleAction.HeroAction_AtkDefeat ||
            action == EHeroBattleAction.HeroAction_CntDefeat ||
            action == EHeroBattleAction.HeroAction_FakeDefeat ||
            action == EHeroBattleAction.HeroAction_DrawAtkDefeat ||
            action == EHeroBattleAction.HeroAction_DrawDefeatAtk)
        {
            return true;
        }

        return false;
    }

    //public virtual void OnCollisionActor(Actor target_actor)
    //{
    //}

    //public virtual void OnCollisionEnter(Collision collision)
    //{
    //}

    //public virtual void OnTriggerEnter(Collider other)
    //{
    //}

    //public virtual void OnTriggerStay(Collider other)
    //{
    //}
   
    public virtual void NotifyDamage(int iDamageValue)
    {
    }    
    
    protected virtual void PlaySound()
    {
    }

    protected void ReadCommend(Hero.EActionCommend actionCommend)
    {
        var textAsset = m_Owner.GetActionCommend(actionCommend);
        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split("\r\n".ToCharArray());
            for (int i = 0; i < lines.Length; ++i)
            {
                if (string.IsNullOrEmpty(lines[i])) { continue; }

                m_Commends.Add(lines[i]);
            }
        }
    }

    protected IEnumerator ActionProc()
    {
        for( int i = 0; i < m_Commends.Count; ++i)
        {
            string commend = m_Commends[i];
            string[] param = commend.Split(",".ToCharArray());
            object[] list = new object[param.Length - 1];
            for (int j = 1; j < param.Length; ++j)
            {
                list[j - 1] = param[j].Trim();
            }
            
            if (m_Owner.IsDie || m_Owner.BattleTargetHero.IsDie)
            {
                continue;
            }

            yield return m_Owner.BattleActionCommendExcution(param[0], list);
        }

        if (m_Owner.IsDie)
        {
            m_Owner.ChangeState(EHeroBattleAction.HeroAction_BattleDie);
        }
        else
        {
            m_Owner.ChangeState(EHeroBattleAction.HeroAction_Idle);
        }
    }    
}