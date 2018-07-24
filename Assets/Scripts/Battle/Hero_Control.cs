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

    Actor mActor = null;
    Outline mOutline = null;

    EHeroBattleAction mHeroState = EHeroBattleAction.HeroAction_Idle;
    EAtionType mActionType = EAtionType.ACTION_MAX;

    Guid mHeroUid = new Guid();
    int mHeroNo = 0;
    int mHP = 0;
    int mMaxHP = 0;
    int mAtk = 0;
    int mDef = 0;
    float mSpeed = 0;
    string mStResPath = null;

    bool mMyTeam = false;
    bool mIsDie = false;
    bool mMyTurn = false;
   
    Transform mEf_HP = null;
    Hero_Control mTarget = null;
    GameObject mHeroObj = null;
    public Vector3 InitPos
    {
        get; set;
    }

    public Guid HeroUid
    {
        set { mHeroUid = value; }
        get { return mHeroUid; }
    }

    public int HeroNo
    {
        set { mHeroNo = value; }
        get { return mHeroNo; }
    }

    public int HP
    {
        set { mHP = value; }
        get { return mHP; }
    }

    public int MaxHP
    {
        set { mMaxHP = value; }
        get { return mMaxHP; }
    }

    public int Atk
    {
        set { mAtk = value; }
        get { return mAtk; }
    }

    public int Def
    {
        set { mDef = value; }
        get { return mDef; }
    }

    public float Speed
    {
        set { mSpeed = value; }
        get { return mSpeed; }
    }

    public string StResPath
    {
        set { mStResPath = value; }
        get { return mStResPath; }
    }

    public EAtionType ActionType
    {
        set { mActionType = value; }
        get { return mActionType; }
    }

    public Hero_Control Target
    {
        set { mTarget = value; }
        get { return mTarget; }
    }

    public bool MyTeam
    {
        set { mMyTeam = value; }
        get { return mMyTeam; }
    }

    public GameObject HeroObj
    {
        set { mHeroObj = value; }
        get { return mHeroObj; }
    }

    public EHeroBattleAction HeroState
    {
        set { mHeroState = value; }
        get { return mHeroState; }
    }

    public bool IsDie
    {
        set { mIsDie = value; }
        get { return mIsDie; }
    }

    public bool MyTurn
    {
        set { mMyTurn = value; }
        get { return mMyTurn; }
    }

    public Actor Actor
    {
        set { mActor = value; }
        get { return mActor; }
    }

    public Outline Outline
    {
        set { mOutline = value; }
        get { return mOutline; }
    }

    public void InitHero(int sortingOrder)
    {
        InitPos = transform.position;

        mActionManager = new HeroBattleActionManager();
        mActionManager.Initialize(this);

        Transform tObj = transform.Find("Obj");
        if (tObj != null)
        {
            mActor = tObj.GetComponent<Actor>();
            mActor.SR.sortingOrder = sortingOrder;
            mOutline = tObj.GetComponent<Outline>();

            mEf_HP = tObj.Find("ef_HP");
            if (mEf_HP == null)
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
        BattleManager.Instance.BattleUI.UpdatePosHPGauge(mHeroUid, mEf_HP);
    }

    public void PlayAnimation(Actor.AniType anim)
    {
        if (mActor != null)
        {
            mActor.PlayAnimation(anim);
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
        bcUI.UpdateHPGauge(mHeroUid, amount);

        GameObject goEfc = EffectManager.Instance().GetEffect(EffectManager.eEffectType.EFFECT_BATTLE_HIT); 
        if (goEfc != null)
        {
            Transform tCen = HeroObj.transform.Find("ef_Center");
            if( tCen != null )
            {
                Transform tEffect = BattleManager.Instance.transform.Find("Effect");
                
                goEfc.transform.parent = tEffect; 
                goEfc.transform.position = tCen.position;
                
                ParticleSystem [] pcs = goEfc.GetComponentsInChildren<ParticleSystem>();
                if (pcs != null)
                {
                    for (int i = 0; i < pcs.Length; ++i)
                    {
                        Renderer render = pcs[i].GetComponent<Renderer>();
                        if (render != null)
                        { 
                            render.sortingOrder = 1000;
                            render.sortingLayerName = "Hero";
                        }
                    }
                }
            }
        }

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

    public void ClearActionMode()
    {
        // BattleState 넣기 전까지 임시로 블러해제 넣는다. 나중에 빼야함
        BattleManager.Instance.ActiveBlur(false);
    }

    public void SetActionMode(EHeroBattleAction heroAction, Vector3 vPos)
    {
        SetPosition(vPos);
        SetScale(new Vector3(Define.BATTLE_MOD_SCALE, Define.BATTLE_MOD_SCALE, Define.BATTLE_MOD_SCALE));
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
}