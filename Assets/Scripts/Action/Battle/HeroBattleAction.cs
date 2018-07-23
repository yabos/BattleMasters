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

    public virtual void OnAnimEnd(string strAnimName)
    {
        if (string.IsNullOrEmpty(strAnimName))
        {
            return;
        }
    }

    public virtual void NotifyDamage(int iDamageValue)
    {
    }    
    
    protected virtual void PlaySound()
    {
    }    
}