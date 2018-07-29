using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleAction
{
    protected Hero_Control m_Owner;
    protected HeroBattleActionManager m_ActionManager;

    protected Vector3 m_veOriginalPos;

    public virtual void Initialize(Hero_Control owner, HeroBattleActionManager action_manager)
    {
        m_Owner = owner;
        m_ActionManager = action_manager;
    }

    public virtual void Release()
    {

    }

    public virtual void DoStart(byte[] data = null)
    {
        if (IsMovingAction(m_ActionManager.GetCurrentAction()))
        {
            m_Owner.IsActiveMoving = true;
        }
    }

    public virtual void DoEnd(EHeroBattleAction eNextAction)
    {
        if (IsMovingAction(m_ActionManager.GetCurrentAction()))
        {
            m_Owner.IsActiveMoving = false;
        }
    }

    public virtual void Update(float fTimeDelta)
    {
    }

    public virtual IEnumerator ActionProc()
    {
        yield break;
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

    protected IEnumerator AnimationDeley(float delay, Actor.AniType aniType)
    {
        m_Owner.PlayAnimation(aniType);

        yield return new WaitForSeconds(delay);
    }

    protected IEnumerator MoveForward(float duration, float speed, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = m_Owner.transform.position;
            vPos.x += m_Owner.IsMyTeam ? speed : (-1 * speed);
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveForwardDistance(float duration, float dist,  Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = m_Owner.transform.position;
            float posX = m_Owner.IsMyTeam ? ((ElapsedTime / duration) * dist) : (-1 * (ElapsedTime / duration) * dist);
            vPos.x = posX;
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveBackward(float duration, float speed, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = m_Owner.transform.position;
            vPos.x += m_Owner.IsMyTeam ? (-1 * speed) : speed;
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveBackwardDistance(float duration, float dist, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = m_Owner.transform.position;
            float posX = m_Owner.IsMyTeam ? (-1 * (ElapsedTime / duration) * dist) : ((ElapsedTime / duration) * dist);
            vPos.x = posX;
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }
}