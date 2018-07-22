using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum eHeroState
{
    HEROSTATE_IDLE = 0,
    HEROSTATE_TRACE_ATK,
    HEROSTATE_CNT_ATK,
    HEROSTATE_FAKE_ATK,
    HEROSTATE_FAKE_DEFEAT,
    HEROSTATE_BREAK_DEFEAT,
    HEROSTATE_CNT_DEFEAT,
    HEROSTATE_DRAW,
}

public enum EAtionType
{
    ACTION_ATK,
    ACTION_COUNT,
    ACTION_FAKE,
    ACTION_MAX
}

public class Hero_Control : MonoBehaviour
{
    Actor mActor = null;
    Outline mOutline = null;

    eHeroState mHeroState = eHeroState.HEROSTATE_IDLE;
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

    SpriteRenderer mSR = new SpriteRenderer();
    Transform mEf_HP = null;
    Hero_Control mTarget = null;    
    GameObject mHeroObj = null;
    Vector3 mInitPos;

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

    public eHeroState HeroState
    {
        set { mHeroState = value; }
        get { return mHeroState; }
    }

    public SpriteRenderer SR
    {
        set { mSR = value; }
        get { return mSR; }
    }

    public bool IsDie
    {
        set { mIsDie = value; }
        get { return mIsDie; }
    }

    public void InitHero()
    {
        mInitPos = transform.position;

        Transform tObj = transform.Find("Obj");
        if (tObj != null)
        {
            mActor = tObj.GetComponent<Actor>();
            mOutline = tObj.GetComponent<Outline>();

            mEf_HP = tObj.Find("ef_HP");
            if (mEf_HP == null)
            {
                Debug.LogError("Not Find ef_HP!");
            }
        }

        SpriteRenderer[] sr = transform.GetComponentsInChildren<SpriteRenderer>();
        if (sr != null && sr.Length > 0)
        {
            for (int i = 0; i < sr.Length; ++i)
            {
                if (sr[i].name.Equals("Shadow") == false)
                {
                    SR = sr[i];
                }
            }
        }
    }

    void Update()
    {
        HPGaugePosUpdate();
        UpdateState();
    }

    float fElasedTime = 0;
    void UpdateState()
    {
        if (mHeroState == eHeroState.HEROSTATE_TRACE_ATK)
        {
            fElasedTime += Time.deltaTime;

            PlayAnm(Actor.AnimationActor.ANI_TRACE);

            if (fElasedTime <= 0.5f)
            {
                Vector3 vPos = transform.position;
                if (MyTeam)
                    vPos.x += 0.1f;
                else
                    vPos.x -= 0.1f;

                transform.position = vPos;
            }
            else if (fElasedTime > 0.5f)
            {
                PlayAnm(Actor.AnimationActor.ANI_ATK);
                mHeroState = eHeroState.HEROSTATE_IDLE;
                Debug.Log("HEROSTATE_TRACE_ATK");
            }
        }
        else if (mHeroState == eHeroState.HEROSTATE_CNT_ATK)
        {            
            fElasedTime += Time.deltaTime;

            PlayAnm(Actor.AnimationActor.ANI_CNT);

            if (fElasedTime <= 0.5f)
            {
                Vector3 vPos = transform.position;
                if (MyTeam)
                    vPos.x += 0.1f;
                else
                    vPos.x -= 0.1f;
                transform.position = vPos;
            }
            else if (fElasedTime > 0.5f)
            {
                PlayAnm(Actor.AnimationActor.ANI_ATK);
                mHeroState = eHeroState.HEROSTATE_IDLE;
                Debug.Log("HEROSTATE_CNT_ATK");
            }
        }
        else if (mHeroState == eHeroState.HEROSTATE_FAKE_ATK)
        {            
            fElasedTime += Time.deltaTime;

            PlayAnm(Actor.AnimationActor.ANI_FAKE);

            if (fElasedTime <= 0.5f)
            {
                Vector3 vPos = transform.position;
                if (MyTeam)
                    vPos.x += 0.1f;
                else
                    vPos.x -= 0.1f;
                transform.position = vPos;
            }
            else if (fElasedTime > 0.5f)
            {
                PlayAnm(Actor.AnimationActor.ANI_ATK);
                mHeroState = eHeroState.HEROSTATE_IDLE;
                Debug.Log("HEROSTATE_FAKE_ATK");
            }
        }
        else if (mHeroState == eHeroState.HEROSTATE_BREAK_DEFEAT)
        {            
            fElasedTime += Time.deltaTime;

            PlayAnm(Actor.AnimationActor.ANI_BREAK);

            if (fElasedTime > 0.5f)
            {
                PlayAnm(Actor.AnimationActor.ANI_DEFEAT);
                mHeroState = eHeroState.HEROSTATE_IDLE;
                Debug.Log("HEROSTATE_BREAK_DEFEAT");
            }
        }
        else if (mHeroState == eHeroState.HEROSTATE_CNT_DEFEAT)
        {            
            fElasedTime += Time.deltaTime;

            PlayAnm(Actor.AnimationActor.ANI_CNT);

            if (fElasedTime > 0.5f)
            {
                PlayAnm(Actor.AnimationActor.ANI_DEFEAT);
                mHeroState = eHeroState.HEROSTATE_IDLE;
                Debug.Log("HEROSTATE_CNT_DEFEAT");
            }
        }
        else if (mHeroState == eHeroState.HEROSTATE_FAKE_DEFEAT)
        {            
            fElasedTime += Time.deltaTime;

            PlayAnm(Actor.AnimationActor.ANI_FAKE);

            if (fElasedTime > 0.5f)
            {
                PlayAnm(Actor.AnimationActor.ANI_DEFEAT);
                mHeroState = eHeroState.HEROSTATE_IDLE;
                Debug.Log("HEROSTATE_FAKE_DEFEAT");
            }
        }
        else if (mHeroState == eHeroState.HEROSTATE_DRAW)
        {
            PlayAnm(Actor.AnimationActor.ANI_ATK);
            mHeroState = eHeroState.HEROSTATE_IDLE;
            Debug.Log("DRAW");
        }
    }

    IEnumerator HeroDeathAlphaFade()
    {
        for (int iFadeCount = 7; iFadeCount >= 0; --iFadeCount)
        {
            for (int iAlpha = 10; iAlpha >= 0; --iAlpha)
            {
                SR.color = new Color(1f, 1f, 1f, ((float)iAlpha) * 0.1f);
                yield return null;
            }

            float fWaitSeconds = (float)iFadeCount * 0.001f;
            yield return new WaitForSeconds(fWaitSeconds);
        }
    }

    bool DamagedHero(Hero_Control atthero, int iSkillNo)
    {
        // if(immune) return false

        HP -= atthero.Atk;
		BattleUI_Control bcUI = UIManager.Instance().GetUI() as BattleUI_Control;
        if (bcUI == null) return false;

        float amount =  (float)HP / (float)MaxHP;
        bcUI.UpdateHPGauge(mHeroUid, amount);

        GameObject goEfc = EffectManager.Instance().GetEffect(EffectManager.eEffectType.EFFECT_BATTLE_HIT); 
        if (goEfc != null)
        {
            Transform tCen = HeroObj.transform.Find("ef_Center");
            if( tCen != null )
            {
                Battle_Control bc = GameMain.Instance().BattleControl;
                Transform tEffect = bc.transform.Find("Effect");
                
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
        SR.color = new Color(1f, 142f/255f, 54f/255f);

        yield return new WaitForSeconds(fDelay);

        SR.color = Color.white;
    }

    void HPGaugePosUpdate()
    {
        if (IsDie) return;

		BattleUI_Control bcUI = UIManager.Instance().GetUI() as BattleUI_Control;
        if (bcUI == null) return;
        bcUI.UpdatePosHPGauge(mHeroUid, mEf_HP);
    }

    public Actor GetActor()
    {
        return mActor;
    }

    public Outline GetOutline()
    {
        return mOutline;
    }

    public void PlayAnm(Actor.AnimationActor anim)
    {
        mActor.PlayAnimation(anim);
    }

    public void ChangeState(eHeroState state, Vector3 battleStartPos)
    {
        HeroState = state;
    }
}