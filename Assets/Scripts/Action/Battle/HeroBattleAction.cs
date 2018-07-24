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
    }

    public virtual void DoEnd(EHeroBattleAction eNextAction)
    {

    }
   
    public virtual void Update(float fTimeDelta)
    {
    }

    public virtual IEnumerator ActionProc()
    {
        yield break;
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
        float traceTime = 0;
        while (traceTime < duration)
        {
            traceTime += Time.deltaTime;

            Vector3 vPos = m_Owner.transform.position;
            vPos.x += m_Owner.MyTeam ? speed : (-1 * speed);
            m_Owner.transform.position = vPos;

            m_Owner.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected void InitHeroState(float duration, float delay = 0)
    {
        m_Owner.InitHeroTweenPosition(duration, delay);
        m_Owner.InitHeroTweenScale(duration, delay);
    }
}