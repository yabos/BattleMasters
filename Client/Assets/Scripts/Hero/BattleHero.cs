using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleHero : Hero
{
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

    private readonly string[] ActionCommend = new string[]
    {
        "AtkWin",
        "AtkDefeat",
        "CntWin",
        "CntDefeat",
        "FakeWin",
        "FakeDefeat",
        "DrawAtkDefeat",
        "DrawDefeatAtk",
    };

    HeroBattleActionManager mActionManager;
    HeroBattleActionCommendExcutor mActionCommendExcutor;
    Dictionary<EActionCommend, TextAsset> mActionCommend = new Dictionary<EActionCommend, TextAsset>();

    public bool IsMyTeam { get; private set; }
    public bool IsMyTurn { get; set; }
    public bool IsAction { get; set; }   //  현재 치고받는 엑션을 하고있는지 여부
    public bool IsDie { get; private set; }

    public EAtionType ActionType { get; set; }
    public BattleHero BattleTargetHero { get; set; }
    public int DefaultSortingOrder { get; protected set; }

    public void InitHero(TB_Hero tbHero, Guid uid, int iHeroNo, bool myTeam, int sortingOrder, GameObject heroObj)
    {
        base.InitHero(tbHero, uid, iHeroNo, heroObj);
        
        IsMyTeam = myTeam;

        SetActionCommend();

        InitHero(sortingOrder);
    }

    public void InitHero(int sortingOrder)
    {
        InitPos = HeroObj.transform.localPosition;

        mActionManager = new HeroBattleActionManager();
        mActionManager.Initialize(this);

        mActionCommendExcutor = new HeroBattleActionCommendExcutor();
        mActionCommendExcutor.Initialize(mActionManager);

        DefaultSortingOrder = sortingOrder;
        SetSortingOrder(sortingOrder);

        SetPosHPUI();
    }

    protected override void Update()
    {
        float fTimeDelta = Time.deltaTime;

        if (mActionManager != null)
        {
            mActionManager.Update(fTimeDelta);
        }
    }

    public void SetSortingOrder(int sortingOrder)
    {
        for (int i = 0; i < ListSR.Count; ++i)
        {
            ListSR[i].sortingOrder = sortingOrder + i;
        }
    }

    public void SetDefaultSortingOrder()
    {
        for (int i = 0; i < ListSR.Count; ++i)
        {
            ListSR[i].sortingOrder = DefaultSortingOrder + i;
        }
    }

    void SetPosHPUI()
    {
        if (Global.SceneMgr.IsBattleScene())
        {
            var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
            if (battleUI != null)
            {
                //battleUI.SetPosHPGauge(HeroUid, Ef_HP);
            }
        }
    }

    public void ChangeState(EHeroBattleAction eAction, byte[] data = null, bool bRefresh = false)
    {
        if (mActionManager != null)
        {
            mActionManager.ChangeAction(eAction, data, bRefresh);
        }
    }

    public void BeHit(BattleHero atthero)
    {
        // if(immune) return false

        if (Global.SceneMgr.IsBattleScene())
        {
            var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
            if (battleUI != null)
            {
                // 임시 데미지 연산 공식
                // 나중에 서버에서 주는 데미지로 감소. 지금은 기획테스트용 임시코드                
                float resultDamage = 0;
                float defConst = Global.TBMgr.GetConstValue(eConstType.Def_Const);
                float damage = atthero.Atk * (defConst / (defConst + Def));
                if (atthero.Atk * 1.3f <= Def)
                {
                    resultDamage = damage * Global.TBMgr.GetConstValue(eConstType.Clean_DEF_Const);
                }
                else if (atthero.Atk * 0.7f >= Def)
                {
                    resultDamage = damage * Global.TBMgr.GetConstValue(eConstType.Crash_DEF_Const);
                }
                else
                {
                    resultDamage = damage;
                }

                ///////////////////////
                int floorDamage = Mathf.FloorToInt(resultDamage);
                HP -= floorDamage;

                float amount = (float)HP / MaxHP;
                //battleUI.UpdateHPGauge(HeroUid, amount);
                battleUI.CreateDamage(floorDamage, Ef_HP.position, IsMyTeam);

                // Effect
                CreateDamageEfc(atthero.HeroNo);

                if (HP <= 0)
                {
                    IsDie = true;
                }
            }
        }
    }

    void CreateDamageEfc(int heroNo)
    {
        if (Global.SceneMgr.IsBattleScene() == false) return;

        var battleScene = Global.SceneMgr.CurrentScene as BattleScene;
        if (battleScene == null) return;

        TB_Hero tbHero;
        if (Global.TBMgr.DicHero.TryGetValue(heroNo, out tbHero))
        {
            var goEfc = Instantiate(EffectManager.Instance.GetEffect(tbHero.mBaseAtkEfc)) as GameObject;
            if (goEfc != null)
            {
                goEfc.transform.parent = battleScene.EffectRoot;
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
    public void ExcuteAction(EHeroBattleAction heroAction, Vector3 vPos, BattleHero targetHero, bool isWinner)
    {
        this.BattleTargetHero = targetHero;

        int sortingOrder = isWinner ? Define.BATTLE_WINNER_SORTINGORDER : Define.BATTLE_LOSER_SORTINGORDER;
        SetSortingOrder(sortingOrder);
        SetPosition(vPos);
        SetScale(new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE));
        ChangeState(heroAction);
    }

    // 전투 애니메이션 읽어와서 실행
    public IEnumerator BattleActionCommendExcution(string key, params object[] list)
    {
        yield return mActionCommendExcutor.Excute(key, list);
    }

    public void SetActionCommend()
    {
        for (int i = 0; i < ActionCommend.Length; ++i)
        {
            string resPath = ResourcePath.CommendPath + HeroNo + "/" + HeroNo + "_" + ActionCommend[i];
            var excution = Global.ResourceMgr.CreatePrefabResource(resPath);
            if (excution != null)
            {
                mActionCommend.Add((EActionCommend)i, (TextAsset)excution.ResourceData);
            }
            else
            {
                Debug.LogError("Load Fail ActionTxt : " + HeroNo + ActionCommend[i]);
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

    public void InitHeroTweenPosition(float duration, float delay = 0)
    {
        var tp = gameObject.AddComponent<TweenPosition>();
        if (tp != null)
        {
            tp.style = UITweener.Style.Once;
            tp.duration = duration;
            tp.delay = delay;
            tp.from = HeroObj.transform.localPosition;
            tp.to = InitPos;
            tp.animationCurve = BackStepCurve;
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
            ts.animationCurve = BackStepCurve;
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
        if (Global.SceneMgr.IsBattleScene())
        {
            var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
            if (battleUI != null)
            {
                //battleUI.DestroyHPGauge(HeroUid);

                var battleScene = Global.SceneMgr.CurrentScene as BattleScene;
                if (battleScene != null)
                {
                    battleScene.TurnUI.DestroyTurnIcon(HeroNo);

                    StartCoroutine(HeroDieAlphaFade(() =>
                    {
                        IsAction = false;
                    }));
                }
            }
        }
    }

    IEnumerator HeroDieAlphaFade(Action endFade)
    {
        float fAlpha = 1f;
        while (fAlpha >= 0)
        {
            for (int i = 0; i < ListSR.Count; ++i)
            {
                ListSR[i].color = new Color(1f, 1f, 1f, fAlpha);
            }

            fAlpha -= 0.1f;

            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 0; i < ListSR.Count; ++i)
        {
            ListSR[i].color = new Color(1f, 1f, 1f, 0);
        }

        endFade();
    }

    public IEnumerator HeroAlphaFade(float delay)
    {
        float ElapsedTime = delay;
        while (ElapsedTime >= 0)
        {
            ElapsedTime -= Time.deltaTime;

            for (int i = 0; i < ListSR.Count; ++i)
            {
                ListSR[i].color = new Color(1f, 1f, 1f, ElapsedTime / delay);
            }

            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < ListSR.Count; ++i)
        {
            ListSR[i].color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
