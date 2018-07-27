using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EAtionType
{
    ACTION_ATK,
    ACTION_COUNT,
    ACTION_FAKE,
    ACTION_MAX
}

public class Hero_Control : MonoBehaviour
{
    HeroBattleActionManager mActionManager;
    
    public Guid HeroUid { get; set; }
    public int HeroNo { get; set; } 
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public float Speed { get; set; }
    public string StResPath { get; set; }   

    public bool IsMyTeam { get; set; }      
    public bool IsMyTurn { get; set; }
    public bool IsActiveMoving { get; set; }   //  현재 치고받는 엑션을 하고있는지 여부
    public bool IsDie { get; set; }

    public EHeroBattleAction HeroState { get; set; }
    public EAtionType ActionType { get; set; }    
    public GameObject HeroObj { get; set; }
    public Actor Actor { get; set; }
    public Outline Outline { get; set; }
    public Transform Ef_HP { get; set; }
    public Vector3 InitPos { get; set; }

    public void InitHero(int sortingOrder)
    {
        InitPos = transform.position;

        mActionManager = new HeroBattleActionManager();
        mActionManager.Initialize(this);

        Transform tObj = transform.Find("Obj");
        if (tObj != null)
        {
            Actor = tObj.GetComponent<Actor>();
            Actor.SR.sortingOrder = sortingOrder;
            Outline = tObj.GetComponent<Outline>();

            Ef_HP = tObj.Find("ef_HP");
            if (Ef_HP == null)
            {
                Debug.LogError("Not Find ef_HP!");
            }
        }
    }

    void Update()
    {
        float fTimeDelta = Time.deltaTime;

        HPGaugePosUpdate();

        if (mActionManager != null)
        {
            mActionManager.Update(fTimeDelta);
        }
    }

    void HPGaugePosUpdate()
    {
        if (IsDie) return;

        if (BattleManager.Instance.BattleUI == null) return;
        BattleManager.Instance.BattleUI.UpdatePosHPGauge(HeroUid, Ef_HP);
    }

    public void PlayAnimation(Actor.AniType anim)
    {
        if (Actor != null)
        {
            Actor.PlayAnimation(anim);
        }
    }

    public void ChangeState(EHeroBattleAction eAction, byte[] data = null, bool bRefresh = false)
    {
        if (mActionManager != null)
        {
            mActionManager.ChangeAction(eAction, data, bRefresh);
        }
    }

    IEnumerator HeroDeathAlphaFade()
    {
        for (int iFadeCount = 7; iFadeCount >= 0; --iFadeCount)
        {
            for (int iAlpha = 10; iAlpha >= 0; --iAlpha)
            {
                Actor.SR.color = new Color(1f, 1f, 1f, (iAlpha) * 0.1f);
                yield return null;
            }

            float fWaitSeconds = iFadeCount * 0.001f;
            yield return new WaitForSeconds(fWaitSeconds);
        }
    }

    bool DamagedHero(Hero_Control atthero, int iSkillNo)
    {
        // if(immune) return false

        HP -= atthero.Atk;
        BattleUI_Control bcUI = BattleManager.Instance.BattleUI;
        if (bcUI == null) return false;

        float amount =  HP / MaxHP;
        bcUI.UpdateHPGauge(HeroUid, amount);

        if (HP <= 0)
        {
            //mHeroState = eHeroState.HEROSTATE_DIE;
            return false;
        }
        else
        {
            
        }

        return true;
    }

    IEnumerator DamagedHeroColor(float fDelay)
    {
        Actor.SR.color = new Color(1f, 142f/255f, 54f/255f);

        yield return new WaitForSeconds(fDelay);

        Actor.SR.color = Color.white;
    }

    public void ExcuteAction(EHeroBattleAction heroAction, Vector3 vPos)
    {
        SetPosition(vPos);
        SetScale(new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE));
        ChangeState(heroAction);
    }

    public void SetPosition(Vector3 vPos)
    {
        transform.position = vPos;
    }

    public void SetScale(Vector3 vScale)
    {
        transform.localScale = vScale;
    }

    public void InitHeroTweenPosition(float duration, float delay = 0)
    {
        var tp = gameObject.AddComponent<TweenPosition>();
        if (tp != null)
        {
            tp.style = UITweener.Style.Once;
            tp.duration = duration;
            tp.delay = delay;
            tp.from = transform.position;
            tp.to = InitPos;
            tp.animationCurve = Actor.BackStepCurve;
            tp.PlayForward();

            Destroy(tp, tp.duration);
        }
    }

    public void InitHeroTweenScale(float duration, float delay = 0)
    {
        var ts = gameObject.AddComponent<TweenScale>();
        if (ts != null)
        {
            ts.style = UITweener.Style.Once;
            ts.duration = duration;
            ts.delay = delay;
            ts.from = transform.localScale;
            ts.to = Vector3.one;
            ts.animationCurve = Actor.BackStepCurve;
            ts.PlayForward();

            Destroy(ts, ts.duration);
        }
    }

    public void InitHeroTween()
    {
        InitHeroTweenPosition(Define.ACTION_INIT_TIME);
        InitHeroTweenScale(Define.ACTION_INIT_TIME);
    }
}