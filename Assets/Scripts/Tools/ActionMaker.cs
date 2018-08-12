using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CommendType
{
    AnimationDelay,
    MoveForward,
    MoveForwardMoment,
    MoveBackward,
    MoveBackwardMoment,
    FadeOut,
}

[Serializable]
public struct ActionData
{    
    public CommendType commend;
    public float duration;
    public float dist;
    public Actor.AniType aniType;
}

public class ActionMaker : MonoBehaviour
{
    public GameObject myHero;
    public GameObject enemyHero;
    
    public List<ActionData> heroActionData = new List<ActionData>();    
    public List<ActionData> enemyActionData = new List<ActionData>();

    Actor heroActor;
    Actor enemyActor;

    HeroBattleActionCommendExcutor mActionCommendExcutor;

    public bool Loop = false;

    // Use this for initialization
    IEnumerator Start ()
    {
        myHero.transform.localRotation = Quaternion.Euler(0, 180, 0);
        myHero.transform.localScale = new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE);        
        enemyHero.transform.localScale = new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE);

        mActionCommendExcutor = new HeroBattleActionCommendExcutor();
        mActionCommendExcutor.Initialize(this);

        yield return new WaitForSeconds(1);

        heroActor = myHero.GetComponentInChildren<Actor>();
        //if (heroActor != null)
        //{
        //    StartCoroutine(ActionProc(heroActor, true));
        //}

        enemyActor = enemyHero.GetComponentInChildren<Actor>();
        //if (enemyActor != null)
        //{
        //    StartCoroutine(ActionProc(enemyActor, false));
        //}
    }

    public void PlayerOnce()
    {
        heroActor = myHero.GetComponentInChildren<Actor>();
        if (heroActor != null)
        {
            StartCoroutine(ActionProc(heroActor, true));
        }

        enemyActor = enemyHero.GetComponentInChildren<Actor>();
        if (enemyActor != null)
        {
            StartCoroutine(ActionProc(enemyActor, false));
        }

        myHero.transform.localPosition = Vector3.zero;
        enemyHero.transform.localPosition = Vector3.zero;
    }

    public void ExportText(bool myTeam)
    {
        //TBManager.Instance
        //heroActor.name
    }

    float myHeroElasedTime = 0;
    float enemyHeroElasedTime = 0;
    private void Update()
    {
        if (Loop == false) return;

        myHeroElasedTime += Time.deltaTime;
        if (myHeroElasedTime >= GetActionTime(true))
        {
            if (heroActor != null)
            {
                StartCoroutine(ActionProc(heroActor, true));
                myHero.transform.localPosition = Vector3.zero;
                myHeroElasedTime = 0;
            }
        }

        enemyHeroElasedTime += Time.deltaTime;
        if (enemyHeroElasedTime >= GetActionTime(false))
        {
            if (enemyActor != null)
            {
                StartCoroutine(ActionProc(enemyActor, false));
                enemyHero.transform.localPosition = Vector3.zero;
                enemyHeroElasedTime = 0;
            }
        }
    }

    float GetActionTime(bool isMyHero)
    {
        float result = 0;
        if (isMyHero)
        {
            foreach (var elem in heroActionData)
            {
                result += elem.duration;
            }
        }
        else
        {
            foreach (var elem in heroActionData)
            {
                result += elem.duration;
            }
        }

        return result;
    }

    public IEnumerator ActionProc(Actor actor, bool isMyTeam)
    {
        List<ActionData> ActionData;
        if (isMyTeam)
        {
            ActionData = new List<ActionData>(heroActionData);
        }
        else
        {
            ActionData = new List<ActionData>(enemyActionData);
        }

        for (int i = 0; i < ActionData.Count; ++i)
        {
            var param = GetExcuteCommend(actor, isMyTeam, ActionData[i]);

            yield return BattleActionCommendExcution(ActionData[i].commend.ToString(), param);
        }
    }

    object[] GetExcuteCommend(Actor actor, bool isMyTeam, ActionData actionData)
    {
        object[] param = new object[5];

        param[0] = actor;

        if (actionData.commend == CommendType.AnimationDelay || actionData.commend == CommendType.FadeOut)
        {
            param[1] = actionData.duration;
            param[2] = "0";
            param[3] = actionData.aniType;
        }
        else if (actionData.commend == CommendType.MoveForwardMoment ||
                actionData.commend == CommendType.MoveBackwardMoment)
        {
            param[1] = isMyTeam;
            param[2] = "0";
            param[3] = actionData.dist;
            param[4] = actionData.aniType;
        }
        else
        {
            param[1] = isMyTeam;
            param[2] = actionData.duration;
            param[3] = actionData.dist;
            param[4] = actionData.aniType;
        }

        return param;
    }

    public IEnumerator BattleActionCommendExcution(string key, params object[] list)
    {
        yield return mActionCommendExcutor.Excute(key, list);
    }
       
    public IEnumerator AnimationDelay(params object[] list)
    {
        Actor actor = list[0] as Actor;

        float delay = Convert.ToSingle(list[1]);
        Actor.AniType eAniType = (Actor.AniType)list[3];
        actor.PlayAnimation(eAniType);

        yield return new WaitForSeconds(delay);
    }

    public IEnumerator MoveForward(params object[] list)
    {
        Actor actor = list[0] as Actor;
        bool IsMyTeam = Convert.ToBoolean(list[1]);

        float duration = Convert.ToSingle(list[2]);
        float dist = Convert.ToSingle(list[3]);
        Actor.AniType eAniType = (Actor.AniType)list[4];

        float ElapsedTime = 0;
        float SumX = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;
            Vector3 vPos = actor.transform.position;
            float tickX = (Time.deltaTime / duration) * dist;
            SumX += tickX;
            if (SumX >= dist)
            {
                tickX = 0;
            }

            if (IsMyTeam == false)
            {
                tickX *= -1;
            }

            vPos.x += tickX;
            actor.transform.position = vPos;

            actor.PlayAnimation(eAniType);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveForwardMoment(params object[] list)
    {
        Actor actor = list[0] as Actor;
        bool IsMyTeam = Convert.ToBoolean(list[1]);

        float dist = Convert.ToSingle(list[3]);
        Actor.AniType eAniType = (Actor.AniType)list[4];

        if (IsMyTeam == false)
        {
            dist *= -1;
        }

        Vector3 vPos = actor.transform.position;
        vPos.x += dist;
        actor.transform.position = vPos;

        actor.PlayAnimation(eAniType);
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator MoveBackward(params object[] list)
    {
        Actor actor = list[0] as Actor;
        bool IsMyTeam = Convert.ToBoolean(list[1]);

        float duration = Convert.ToSingle(list[2]);
        float dist = Convert.ToSingle(list[3]);
        Actor.AniType eAniType = (Actor.AniType)list[4];

        float ElapsedTime = 0;
        float SumX = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;
            Vector3 vPos = actor.transform.position;
            float tickX = (Time.deltaTime / duration) * dist;
            SumX += tickX;
            if (SumX >= dist)
            {
                tickX = 0;
            }

            if (IsMyTeam)
            {
                tickX *= -1;
            }

            vPos.x += tickX;
            actor.transform.position = vPos;

            actor.PlayAnimation(eAniType);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveBackwardMoment(params object[] list)
    {
        Actor actor = list[0] as Actor;
        bool IsMyTeam = Convert.ToBoolean(list[1]);

        float dist = Convert.ToSingle(list[3]);
        Actor.AniType eAniType = (Actor.AniType)list[4];

        if (IsMyTeam)
        {
            dist *= -1;
        }

        Vector3 vPos = actor.transform.position;
        vPos.x += dist;
        actor.transform.position = vPos;

        actor.PlayAnimation(eAniType);
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator FadeOut(params object[] list)
    {
        Actor actor = list[0] as Actor;

        float delay = Convert.ToSingle(list[1]);
        Actor.AniType eAniType = (Actor.AniType)list[3];

        actor.PlayAnimation(eAniType);
        yield return HeroAlphaFade(actor, delay);
    }
    
    public IEnumerator HeroAlphaFade(Actor actor, float delay)
    {
        float ElapsedTime = delay;
        while (ElapsedTime >= 0)
        {
            ElapsedTime -= Time.deltaTime;

            for (int i = 0; i < actor.ListSR.Count; ++i)
            {
                actor.ListSR[i].color = new Color(1f, 1f, 1f, ElapsedTime / delay);
            }

            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < actor.ListSR.Count; ++i)
        {
            actor.ListSR[i].color = new Color(1f, 1f, 1f, 1f);
        }
    }
}