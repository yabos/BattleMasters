using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(BattleManager)) as BattleManager;
                if (_instance == null)
                {
                    GameObject manaer = new GameObject("BattleManager", typeof(BattleManager));
                    _instance = manaer.GetComponent<BattleManager>();
                }
            }

            return _instance;
        }
    }

    readonly List<Hero_Control> mListMyHeroes = new List<Hero_Control>();
    readonly List<Hero_Control> mListEnemyHeroes = new List<Hero_Control>();

    public GameObject BattleRoot;

    public Battleground Battleground
    {
        get; set;
    }

    public BattleStateManager BattleStateManager
    {
        get; set;
    }

    public BattleAIManager BattleAIManager
    {
        get; set;
    }

    public BattleUI_Control BattleUI
    {
        get; set;
    }

    public TurnUI_Control TurnUI
    {
        get; set;
    }

    public GameObject Blur;

    public int ActiveTurnHeroNo
    {
        get; set;
    }

    public int ActiveTargetHeroNo
    {
        get; set;
    }

    public List<Hero_Control> ListMyHeroes
    {
        get { return mListMyHeroes; }
    }

    public List<Hero_Control> ListEnemyHeroes
    {
        get { return mListEnemyHeroes; }
    }

    public bool OnlyActionInput
    {
        get; set;
    }

    public Transform EffectRoot
    {
        get; private set;
    }

    void Awake()
    {
        EffectRoot = BattleRoot.transform.Find("Effect");

        TBManager.Instance.LoadTableAll();

        if (BattleStateManager == null)
        {
            BattleStateManager = new BattleStateManager();
            BattleStateManager.Initialize(this);
        }

        if (BattleAIManager == null)
        {
            BattleAIManager = new BattleAIManager();
            BattleAIManager.Initialize(this);
        }
    }

    void Update()
    {
        float fDeltaTime = Time.deltaTime;

        if (BattleStateManager != null)
        {
            BattleStateManager.Update(fDeltaTime);
        }

        if (BattleAIManager != null)
        {
            BattleAIManager.Update(fDeltaTime);
        }
    }

    public void SetBattleStateActionStart(bool isTurnOut)
    {
        ActiveBlur(!isTurnOut);
        ActiveOutline(false);

        BattleUI.ActiveAllBattleProfile(false);
        BattleUI.ActiveHUDUI(isTurnOut);
        BattleUI.ActiveTurnTimer(false);

        if (isTurnOut == false)
        {
            ExcuteHeroAction();
        }
    }

    public void SetBattleStateActionEnd()
    {
        InitHeroTween();
        TurnUI.InitActiveTurnMember(ActiveTurnHeroNo);

        BattleUI.ActiveSelActionType(false);
        BattleUI.ActiveHUDUI(true);

        ActiveBlur(false);
        ActiveTurnHeroNo = 0;
        ActiveTargetHeroNo = 0;
    }

    public void InitHeroTween()
    {
        var hero = GetHeroControl(ActiveTurnHeroNo);
        if (hero != null)
        {
            hero.InitHeroTween();
            hero.IsMyTurn = false;

            UtilFunc.ChangeLayersRecursively(hero.transform, Define.DEFAULT_LAYER);
        }

        hero = GetHeroControl(ActiveTargetHeroNo);
        if (hero != null)
        {
            hero.InitHeroTween();

            UtilFunc.ChangeLayersRecursively(hero.transform, Define.DEFAULT_LAYER);
        }
    }

    void ExcuteHeroAction()
    {
        var ActiveHero = GetHeroControl(ActiveTurnHeroNo);
        if (ActiveHero != null)
        {
            Debug.Log("ActiveHero Action : " + ActiveHero.ActionType);
        }

        var TargetHero = GetHeroControl(ActiveTargetHeroNo);
        if (TargetHero != null)
        {
            Debug.Log("TargetHero Action : " + TargetHero.ActionType);
        }

        EHeroBattleAction ActiveAction = ResultBattleAction(ActiveHero, TargetHero);
        Vector3 vPos = Battleground.GetTeamPos(ActiveAction, ActiveHero.IsMyTeam);
        ActiveHero.ExcuteAction(ActiveAction, vPos, TargetHero);        

        EHeroBattleAction TargetHeroAction = ResultBattleAction(TargetHero, ActiveHero);
        vPos = Battleground.GetTeamPos(TargetHeroAction, TargetHero.IsMyTeam);
        TargetHero.ExcuteAction(TargetHeroAction, vPos, ActiveHero);
    }
    
    EHeroBattleAction ResultBattleAction(Hero_Control mine, Hero_Control yours)
    {
        // Win
        if (mine.ActionType == EAtionType.ACTION_ATK && yours.ActionType == EAtionType.ACTION_FAKE)
        {
            return EHeroBattleAction.HeroAction_AtkWin;
        }
        else if (mine.ActionType == EAtionType.ACTION_COUNT && yours.ActionType == EAtionType.ACTION_ATK)
        {
            return EHeroBattleAction.HeroAction_CntWin;
        }
        else if (mine.ActionType == EAtionType.ACTION_FAKE && yours.ActionType == EAtionType.ACTION_COUNT)
        {
            return EHeroBattleAction.HeroAction_FakeWin;
        }
        // Defeat
        else if (mine.ActionType == EAtionType.ACTION_FAKE && yours.ActionType == EAtionType.ACTION_ATK)
        {
            return EHeroBattleAction.HeroAction_FakeDefeat;
        }
        else if (mine.ActionType == EAtionType.ACTION_ATK && yours.ActionType == EAtionType.ACTION_COUNT)
        {
            return EHeroBattleAction.HeroAction_AtkDefeat;
        }
        else if (mine.ActionType == EAtionType.ACTION_COUNT && yours.ActionType == EAtionType.ACTION_FAKE)
        {
            return EHeroBattleAction.HeroAction_CntDefeat;
        }
        else
        {
            // draw
            if (mine.IsMyTurn)
            {
                return EHeroBattleAction.HeroAction_DrawAtkDefeat;
            }
            else
            {
                return EHeroBattleAction.HeroAction_DrawDefeatAtk;
            }            
        }
    }
    
    public void SetActiveTurnHero(int heroNo)
    {
        ActiveTurnHeroNo = heroNo;

        var hero = GetHeroControl(heroNo);
        if (hero != null)
        {
            hero.IsMyTurn = true;
        }

        BattleUI.ActiveBattleProfile(true, hero.IsMyTeam);
        BattleUI.SetProfileUI(heroNo, true);
        BattleUI.SetTurnTimer(Define.SELECT_TARGET_LIMITTIME, ETurnTimeType.TURNTIME_SEL_TARGET);        
    }

    public Hero_Control GetHeroControl(int heroNo)
    {
        var hero = ListMyHeroes.Find(x => x.HeroNo.Equals(heroNo));
        if (hero != null)
        {
            return hero;
        }

        hero = ListEnemyHeroes.Find(x => x.HeroNo.Equals(heroNo));
        if (hero != null)
        {
            return hero;
        }

        return null;
    }

    public void SetOutlineHero(int heroNo)
    {
        var heroCont = GetHeroControl(heroNo);
        if (heroCont == null) return;

        if (heroCont.IsMyTeam)
        {
            foreach (var elem in mListMyHeroes)
            {
                elem.Outline.eraseRenderer = !elem.HeroNo.Equals(heroNo);
            }
        }
        else
        {
            foreach (var elem in mListEnemyHeroes)
            {
                elem.Outline.eraseRenderer = !elem.HeroNo.Equals(heroNo);
            }
        }
    }

    public void ActiveOutline(bool active)
    {
        foreach (var elem in mListMyHeroes)
        {
            elem.Outline.eraseRenderer = !active;
        }

        foreach (var elem in mListEnemyHeroes)
        {
            elem.Outline.eraseRenderer = !active;
        }
    }

    public void ActiveBlur(bool active)
    {
        Blur.SetActive(active);
    }

    public bool CheckAction()
    {
        bool myHeroAction = false;
        foreach (var elem in mListMyHeroes)
        {
            //if (elem.IsDie) continue;

            if (elem.IsAction)
            {
                myHeroAction = true;
            }
        }

        bool enemyHeroAction = false;
        foreach (var elem in mListEnemyHeroes)
        {
            //if (elem.IsDie) continue;

            if (elem.IsAction)
            {
                enemyHeroAction = true;
            }
        }

        return myHeroAction || enemyHeroAction;
    }

    public bool GetActiveHeroTeam()
    {
        var hero = GetHeroControl(ActiveTurnHeroNo);
        if (hero != null)
        {
            return hero.IsMyTeam;
        }

        return false;
    }

    // AI 용도. 상대팀 살아있는 한명 랜덤으로 넘겨줌
    // 나중에 조건을 검색해서 넘겨줄수도 있음
    public int GetRandomHeroTeam()
    {
        int Idx = Random.Range(0, mListMyHeroes.Count);
        Hero_Control randomHero = mListMyHeroes[Idx];
        while (randomHero.IsDie)
        {
            Idx = Random.Range(0, mListMyHeroes.Count);
            if(mListMyHeroes[Idx].IsDie == false)
            {
                randomHero = mListMyHeroes[Idx];
            }
        }

        return randomHero.HeroNo;
    }

    public bool IsMyTeamAllDie()
    {
        for (int i = 0; i < mListMyHeroes.Count; ++i)
        {
            if (mListMyHeroes[i].IsDie == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsEnemyAllDie()
    {
        for (int i = 0; i < mListEnemyHeroes.Count; ++i)
        {
            if (mListEnemyHeroes[i].IsDie == false)
            {
                return false;
            }
        }

        return true;
    }
}