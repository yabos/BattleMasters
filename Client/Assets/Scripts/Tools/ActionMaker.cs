#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public enum CommendType
{
    AnimationDelay,
    MoveForward,
    MoveForwardMoment,
    MoveBackward,
    MoveBackwardMoment,
    FadeOut,
}

public enum ActionExcutionType
{
    AtkWin,
    AtkDefeat,
    CntWin,
    CntDefeat,
    FakeWin,
    FakeDefeat,
    DrawAtkDefeat,
    DrawDefeatAtk,
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

    public ActionExcutionType heroExcType;
    public ActionExcutionType enmeyExcType;

    HeroBattleActionCommendExcutor mActionCommendExcutor;

    public bool Loop = false;

    // Use this for initialization
    IEnumerator Start ()
    {
        yield return new WaitForSeconds(1);

        Initialize();
    }

    private void Initialize()
    {
        heroActor = myHero.GetComponentInChildren<Actor>();
        enemyActor = enemyHero.GetComponentInChildren<Actor>();

        myHero.transform.localRotation = Quaternion.Euler(0, 180, 0);
        myHero.transform.localScale = new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE);
        enemyHero.transform.localScale = new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE);

        mActionCommendExcutor = new HeroBattleActionCommendExcutor();
        mActionCommendExcutor.Initialize(this);

        //TBManager.Instance.LoadTableAll();
    }

    public void PlayerOnce()
    {
        heroActor = myHero.GetComponentInChildren<Actor>();
        if (heroActor != null)
        {
            StartCoroutine(ActionProc(heroActor, true, () => 
            {
                myHero.transform.localPosition = Vector3.zero;
                heroActor.PlayAnimation(Actor.AniType.ANI_IDLE);
            }));
        }

        enemyActor = enemyHero.GetComponentInChildren<Actor>();
        if (enemyActor != null)
        {
            StartCoroutine(ActionProc(enemyActor, false, () => 
            {
                enemyHero.transform.localPosition = Vector3.zero;
                enemyActor.PlayAnimation(Actor.AniType.ANI_IDLE);
            }));
        }
    }

    public void ExportText(bool myTeam)
    {
        Initialize();

        string fileName = string.Empty;
        int heroNo = 0;

        if (myTeam)
        {
            heroNo = Global.TBMgr.GetHeroNoByName(heroActor.name);
            fileName = heroNo.ToString() + "_" + heroExcType.ToString() + ".txt";
        }
        else
        {
            heroNo = Global.TBMgr.GetHeroNoByName(enemyActor.name);
            fileName = heroNo.ToString() + "_" + enmeyExcType.ToString() + ".txt";
        }

        string stData = GetActionData(myTeam);
        string path = ResourcePath.CommendExcutePath + heroNo.ToString() + "/" + fileName;
        Debug.Log(path);
        File.WriteAllText(path, stData);
        AssetDatabase.Refresh();
    }

    public void LoadActionType(bool myTeam)
    {
        Initialize();

        string fileName = string.Empty;
        int heroNo = 0;

        if (myTeam)
        {
            heroActionData.Clear();

            heroNo = Global.TBMgr.GetHeroNoByName(heroActor.name);
            fileName = heroNo.ToString() + "_" + heroExcType.ToString() + ".txt";
        }
        else
        {
            enemyActionData.Clear();

            heroNo = Global.TBMgr.GetHeroNoByName(enemyActor.name);
            fileName = heroNo.ToString() + "_" + enmeyExcType.ToString() + ".txt";
        }

        string stData = GetActionData(myTeam);
        string path = ResourcePath.CommendExcutePath + heroNo.ToString() + "/" + fileName;

        string data = File.ReadAllText(path);
        ParsingActionData(data, myTeam);
    }

    void ParsingActionData(string data, bool myTeam)
    {
        string[] split = data.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < split.Length; ++i)
        {
            var action = split[i].Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            ActionData actionData = new ActionData();

            actionData.commend = GetCommendTypeByString(action[0].Trim());
            actionData.duration = float.Parse(action[1]);
            actionData.dist = float.Parse(action[2]);            
            actionData.aniType = heroActor.GetAniType(action[3].Trim());

            if (myTeam)
            {
                heroActionData.Add(actionData);
            }
            else
            {
                enemyActionData.Add(actionData);
            }
        }
    }

    CommendType GetCommendTypeByString(string type)
    {
        for (int i = 0; i <= (int)CommendType.FadeOut; ++i)
        {
            if (((CommendType)i).ToString() == type)
            {
                return (CommendType)i;
            }
        }

        Debug.LogError("No Search Find CommendType.");
        return CommendType.FadeOut;
    }

    Actor.AniType GetAniTypeTypeByString(string type)
    {
        for (int i = 0; i < (int)Actor.AniType.ANI_MAX; ++i)
        {
            if (((Actor.AniType)i).ToString() == type)
            {
                return (Actor.AniType)i;
            }
        }

        Debug.LogError("No Search Find Actor.AniType.");
        return Actor.AniType.ANI_MAX;
    }

    string GetActionData(bool myHero)
    {
        string stData = string.Empty;

        var data = heroActionData;
        if (myHero == false)
        {
            data = enemyActionData;
        }

        for (int i = 0; i < data.Count; ++i)
        {
            stData += data[i].commend.ToString() + ",";
            stData += data[i].duration.ToString() + ",";
            stData += data[i].dist.ToString() + ",";
            stData += heroActor.GetAniTypeClip(data[i].aniType) + "\n";
        }

        return stData;
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
                StartCoroutine(ActionProc(heroActor, true, () => 
                {
                    myHero.transform.localPosition = Vector3.zero;
                    
                }));

                myHeroElasedTime = 0;
            }
        }

        enemyHeroElasedTime += Time.deltaTime;
        if (enemyHeroElasedTime >= GetActionTime(false))
        {
            if (enemyActor != null)
            {
                StartCoroutine(ActionProc(enemyActor, false, () => 
                {
                    enemyHero.transform.localPosition = Vector3.zero;
                    enemyActor.PlayAnimation(Actor.AniType.ANI_IDLE);                    
                }));

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
            foreach (var elem in enemyActionData)
            {
                result += elem.duration;
            }
        }

        return result;
    }

    public IEnumerator ActionProc(Actor actor, bool isMyTeam, Action endAction)
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

        endAction();
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
        float duration = Convert.ToSingle(list[2]);
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
        yield return new WaitForSeconds(duration);
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
        float duration = Convert.ToSingle(list[2]);
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
        yield return new WaitForSeconds(duration);
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

#endif