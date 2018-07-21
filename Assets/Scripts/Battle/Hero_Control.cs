using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hero_Control : MonoBehaviour
{
    public enum eHeroState
    {
        HEROSTATE_IDLE = 0,
        HEROSTATE_ATK,
        HEROSTATE_CNT,
        HEROSTATE_FAKE,
        HEROSTATE_DEFEAT,
    }

    public enum eAttPos
    {
        ATTPOS_NONE = 0,
        ATTPOS_LEFT,
        ATTPOS_RIGHT,
    }

    public readonly int ATTPOS_MAXCOUNT = 2;

    Actor mActor = null;
    Outline mOutline = null;

    eHeroState mHeroState = eHeroState.HEROSTATE_IDLE;

    Guid mHeroUid = new Guid();
    int mHeroNo = 0;
    int mHP = 0;
    int mMaxHP = 0;
    int mAtk = 0;
    int mDef = 0;    
    float mSpeed = 0;
    float mBlowPower = 0;
    float mBlowTolerance = 0;
    string mStResPath = null;

    bool mMyTeam = false;
    bool mIsDie = false;

    SpriteRenderer mSR = new SpriteRenderer();
    Transform mEf_HP = null;
    Hero_Control mTarget = null;    
    GameObject mHeroObj = null;

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

    public float BlowPower
    {
        set { mBlowPower = value; }
        get { return mBlowPower; }
    }

    public float BlowTolerance
    {
        set { mBlowTolerance = value; }
        get { return mBlowTolerance; }
    }

    public string StResPath
    {
        set { mStResPath = value; }
        get { return mStResPath; }
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
        //MyStateControl();
        HPGaugePosUpdate();
    }

    void MyStateControl()
    {
        switch ((int)mHeroState)
        {
            case (int)eHeroState.HEROSTATE_IDLE:
                Battle_Control bc = GameMain.Instance().BattleControl;
                if (bc != null)
                {
                    if (bc.BattleState == Battle_Control.eBattleState.eBattle_Ready)
                    {
                        // 전투 준비 중일 때 시작 위치로 아군이동
                    }
                    else if (bc.BattleState == Battle_Control.eBattleState.eBattle_Ing)
                    {
                        // 피아 모두 적을 탐색
                    }
                    else if (bc.BattleState == Battle_Control.eBattleState.eBattle_Win)
                    {
                        // 아군 이겼을 때 포즈 후 다음 스테이지로
                    }
                    else if (bc.BattleState == Battle_Control.eBattleState.eBattle_Lose)
                    {
                        // 적이 이겼을 때 포즈
                    }
                }                
                break;
           
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
}