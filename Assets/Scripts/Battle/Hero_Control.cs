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

public enum EActionCommend
{
    COMMEND_ATK_WIN,
    COMMEND_ATK_DEFEAT,
    COMMEND_CNT_WIN,
    COMMEND_CNT_DEFEAT,
    COMMEND_FAKE_WIN,
    COMMEND_FAKE_DEFEAT,
    COMMEND_DRAW_ATK_DEFEAT,
    COMMEND_DRAW_DEFEAT_ATK,
    COMMEND_MAX,
}

public class Hero_Control : MonoBehaviour
{
    public string[] _ActionCommend = new string[] 
    {
        "_AtkWin",
        "_AtkDefeat",
        "_CntWin",
        "_CntDefeat",
        "_FakeWin",
        "_FakeDefeat",
        "_DrawAtkDefeat",
        "_DrawDefeatAtk",
    };

    HeroBattleActionManager mActionManager;
    HeroBattleActionCommendExcutor mActionCommendExcutor;
    Dictionary<EActionCommend, TextAsset> mActionCommend = new Dictionary<EActionCommend, TextAsset>();

    public Guid HeroUid { get; set; }
    public int HeroNo { get; set; }
    public string HeroName { get; set; }
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public float Speed { get; set; }
    public string StResPath { get; set; }   

    public bool IsMyTeam { get; set; }      
    public bool IsMyTurn { get; set; }
    public bool IsAction { get; set; }   //  현재 치고받는 엑션을 하고있는지 여부
    public bool IsDie { get; set; }

    public EAtionType ActionType { get; set; }    
    public GameObject HeroObj { get; set; }
    public Actor Actor { get; set; }
    public Outline Outline { get; set; }
    public Transform Ef_HP { get; set; }
    public Transform Ef_Effect { get; set; }
    public Vector3 InitPos { get; set; }
    public Hero_Control TargetHero { get; set; }

    public void InitHero(int sortingOrder)
    {
        InitPos = transform.position;

        mActionManager = new HeroBattleActionManager();
        mActionManager.Initialize(this);

        mActionCommendExcutor = new HeroBattleActionCommendExcutor();
        mActionCommendExcutor.Initialize(mActionManager);

        Transform tObj = transform.Find("Obj");
        if (tObj != null)
        {
            Actor = tObj.GetComponent<Actor>();
            for (int i = 0; i < Actor.ListSR.Count; ++i)
            {
                Actor.ListSR[i].sortingOrder = sortingOrder + i;
            }

            Outline = tObj.GetComponent<Outline>();

            Ef_HP = tObj.Find("ef_HP");
            if (Ef_HP == null)
            {
                Debug.LogError("Not Find ef_HP!");
            }

            Ef_Effect = tObj.Find("ef_Center");
            if (Ef_Effect == null)
            {
                Debug.LogError("Not Find Ef_Effect!");
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

    public void BeHit(Hero_Control atthero)
    {
        // if(immune) return false

        HP -= atthero.Atk;
        BattleUIManager bcUI = BattleManager.Instance.BattleUI;
        if (bcUI == null) return;

        float amount = (float)HP / MaxHP;
        bcUI.UpdateHPGauge(HeroUid, amount);
        bcUI.CreateDamage(atthero.Atk, Ef_HP.position, IsMyTeam);

        // Effect
        CreateDamageEfc(atthero.HeroNo);

        if (HP <= 0)
        {
            IsDie = true;
        }
    }

    void CreateDamageEfc(int heroNo)
    {
        TB_Hero tbHero;
        if (TBManager.Instance.cont_Hero.TryGetValue(heroNo, out tbHero))
        {
            var goEfc = Instantiate(EffectManager.Instance.GetEffect(tbHero.mBaseAtkEfc)) as GameObject;
            if (goEfc != null)
            {
                goEfc.transform.parent = BattleManager.Instance.EffectRoot;
                goEfc.transform.position = Ef_Effect.position;

                ParticleSystem[] pcs = goEfc.GetComponentsInChildren<ParticleSystem>();
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

                var efcData = goEfc.GetComponent<EffectData>();
                if (efcData != null)
                {
                    Destroy(goEfc, efcData.LifeTime);
                }
            }
        }
    }

    // 실제 전투 행동
    public void ExcuteAction(EHeroBattleAction heroAction, Vector3 vPos, Hero_Control targetHero)
    {
        this.TargetHero = targetHero;

        SetPosition(vPos);
        SetScale(new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE));
        ChangeState(heroAction);
    }

    // 전투 애니메이션 읽어와서 실행
    public IEnumerator BattleActionCommendExcution(string key, params object [] list)
    {
        yield return mActionCommendExcutor.Excute(key, list);
    }

    public void SetActionCommend()
    {
        for (int i = 0; i < _ActionCommend.Length; ++i)
        {
            string resPath = ResourcePath.CommendPath + HeroNo + "/" + HeroNo + _ActionCommend[i];
            var excution = VResources.Load<TextAsset>(resPath);
            if (excution != null)
            {
                mActionCommend.Add((EActionCommend)i, excution);                
            }
            else
            {
                Debug.LogError("Load Fail ActionTxt : " + HeroNo + _ActionCommend[i]);
            }
        }
    }

    public TextAsset GetActionCommend(EActionCommend actionCommend)
    {
        if (mActionCommend.ContainsKey(actionCommend))
        {
            return mActionCommend[actionCommend];
        }

        return null;
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

    public void HeroDie()
    {
        BattleUIManager bcUI = BattleManager.Instance.BattleUI;
        if (bcUI == null) return;
        {
            bcUI.DestroyHPGauge(HeroUid);
            BattleManager.Instance.TurnUI.DestroyTurnIcon(HeroNo);
        }

        StartCoroutine(HeroDieAlphaFade(() => 
        {
            IsAction = false;
        }));
            
    }

    IEnumerator HeroDieAlphaFade(Action endFade)
    {
        float fAlpha = 1f;
        while (fAlpha >= 0)
        {
            for (int i = 0; i < Actor.ListSR.Count; ++i)
            {
                Actor.ListSR[i].color = new Color(1f, 1f, 1f, fAlpha);
            }

            fAlpha -= 0.1f;

            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 0; i < Actor.ListSR.Count; ++i)
        {
            Actor.ListSR[i].color = new Color(1f, 1f, 1f, 0);
        }

        endFade();
    }
}