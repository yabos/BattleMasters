﻿using UnityEngine;
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
                    GameObject dataManaer = new GameObject("BattleManager", typeof(BattleManager));
                    _instance = dataManaer.GetComponent<BattleManager>();
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

    public BattleUI_Control BattleUI
    {
        get; set;
    }

    public TurnUI_Control TurnUI
    {
        get; set;
    }

    public GameObject Blur;

    public int ActiveTurnHero
    {
        get; set;
    }

    public int ActiveTargetHero
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

    void Awake()
    {
        TBManager.Instance.LoadTableAll();

        if (BattleStateManager == null)
        {
            BattleStateManager = new BattleStateManager();
            BattleStateManager.Initialize(this);
        }
    }
    
    void Update()
    {
        float fDeltaTime = Time.deltaTime;

        if (BattleStateManager != null)
        {
            BattleStateManager.Update(fDeltaTime);
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
        var hero = GetHeroControl(ActiveTurnHero);
        if (hero != null)
        {
            hero.IsMyTurn = false;
            TurnUI.InitActiveTurnMember(ActiveTurnHero);
            BattleUI.ActiveSelActionType(false);
        }

        ActiveBlur(false);
        ActiveTurnHero = 0;
        ActiveTargetHero = 0;
        
        BattleUI.ActiveHUDUI(true);        
    }

    void ExcuteHeroAction()
    {
        var MyTeamHero = GetHeroControl(ActiveTurnHero);
        if (MyTeamHero != null)
        {
            Debug.Log("Player : " + MyTeamHero.ActionType);
        }

        var EnemyHero = GetHeroControl(ActiveTargetHero);
        if (EnemyHero != null)
        {
            Debug.Log("Enemy : " + EnemyHero.ActionType);
        }

        EHeroBattleAction myHeroAction = ResultBattleAction(MyTeamHero, EnemyHero);
        Vector3 vPos = Battleground.GetTeamPos(myHeroAction, MyTeamHero.IsMyTeam);
        MyTeamHero.ExcuteAction(myHeroAction, vPos);        

        EHeroBattleAction enemyHeroAction = ResultBattleAction(EnemyHero, MyTeamHero);
        vPos = Battleground.GetTeamPos(enemyHeroAction, EnemyHero.IsMyTeam);
        EnemyHero.ExcuteAction(enemyHeroAction, vPos);
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
        ActiveTurnHero = heroNo;

        var hero = GetHeroControl(heroNo);
        if (hero != null)
        {
            hero.IsMyTurn = true;
        }

        BattleUI.ActiveBattleProfile(true, true);
        BattleUI.SetProfileUI(heroNo);
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
        foreach (var elem in mListMyHeroes)
        {
            elem.Outline.eraseRenderer = !elem.HeroNo.Equals(heroNo);
        }

        foreach (var elem in mListEnemyHeroes)
        {
            elem.Outline.eraseRenderer = !elem.HeroNo.Equals(heroNo);
        }
    }

    void ActiveOutline(bool active)
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

    public bool CheckActiveMoving()
    {
        foreach (var elem in mListMyHeroes)
        {
            if (elem.IsDie) continue;

            if (elem.IsActiveMoving)
            {
                return true;
            }
        }

        foreach (var elem in mListEnemyHeroes)
        {
            if (elem.IsDie) continue;

            if (elem.IsActiveMoving)
            {
                return true;
            }
        }

        return false;
    }
}