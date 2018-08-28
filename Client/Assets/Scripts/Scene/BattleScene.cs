using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleScene : SceneBase
{
    readonly List<Hero> mListMyHeroes = new List<Hero>();
    readonly List<Hero> mListEnemyHeroes = new List<Hero>();

    public GameObject BattleRoot { get; private set; }
    public BattleStateManager BattleStateManager { get; private set; }
    public BattleAIManager BattleAIManager { get; private set; }
    public Transform EffectRoot { get; private set; }
    public Battleground Battleground
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

    public List<Hero> ListMyHeroes
    {
        get { return mListMyHeroes; }
    }

    public List<Hero> ListEnemyHeroes
    {
        get { return mListEnemyHeroes; }
    }

    public bool OnlyActionInput
    {
        get; set;
    }

    public override IEnumerator OnEnter(float progress)
    {
        yield return base.OnEnter(progress);

        yield return Global.UIMgr.OnCreateWidgetAsync<UIBattle>(ResourcePath.UIBattle, widget =>
        {
            if (widget != null)
            {
                Global.SoundMgr.PlayBGM(SoundManager.eBGMType.eBGM_Battle);

                widget.Show();
                SetEnterPageProgressInfo(0.5f);
            }
        });

        EffectRoot = BattleRoot.transform.Find("Effect");

        if (BattleStateManager == null)
        {
            BattleStateManager = new BattleStateManager();
            BattleStateManager.Initialize(this);
        }

        if (BattleAIManager == null)
        {
            BattleAIManager = new BattleAIManager();
            BattleAIManager.Initialize();
        }

        yield return new WaitForSeconds(1.0f);
    }

    public override void OnExit()
    {
        base.OnExit();

        Global.UIMgr.HideAllWidgets(0.3f);
    }

    public override void OnInitialize()
    {

    }

    public override void OnFinalize()
    {
    }

    public override void OnRequestEvent(string netClentTypeName, string requestPackets)
    {
        Global.UIMgr.ShowLoadingWidget(0.3f);
    }

    public override void OnReceivedEvent(string netClentTypeName, string receivePackets)
    {
        Global.UIMgr.HideLoadingWidget(0.1f);
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);

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


    //private void ShowMessageBoxWithPluginNotifyInfo(string message, eMessageBoxType boxType = eMessageBoxType.OK, System.Action<bool> completed = null)
    //{
    //    string title = StringUtil.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(Color.blue), "System Info Message");
    //    Global.WidgetMgr.ShowMessageBox(title, message, boxType, completed);
    //}

    public override void OnNotify(INotify notify)
    {
        base.OnNotify(notify);
    }


    public void CreateBattleHero(Transform tParant, int iHeroNo, bool MyTeam, int sortingOrder, int spawnPos)
    {
        GameObject goHero = new GameObject();

        Guid uid = Guid.NewGuid();
        goHero.transform.parent = tParant;
        goHero.transform.name = uid.ToString();

        var hero = UtilSystem.CreateHero(goHero, uid, iHeroNo, MyTeam, sortingOrder);
        if (hero != null)
        {
            SetBattleSpawnPos(goHero, MyTeam, spawnPos);

            var battleUI = Global.UIMgr.GetUIBattle();
            // create hero hp
            if (battleUI != null)
            {
                battleUI.CreateHeroHp(hero.HeroUid, MyTeam);
            }

            if (MyTeam)
            {
                ListMyHeroes.Add(hero);
            }
            else
            {
                ListEnemyHeroes.Add(hero);
            }
        }
        else
        {
            Debug.LogError("CreateBattleHero Fail : " + iHeroNo.ToString());
        }
    }

    void SetBattleSpawnPos(GameObject goHero, bool myTeam, int spawnPos)
    {
        Transform tSPos = myTeam ? Battleground.BattleRegenPosMyTeam[spawnPos] : Battleground.BattleRegenPosEnemy[spawnPos];
        if (tSPos != null)
        {
            goHero.transform.position = tSPos.position;
            goHero.transform.rotation = Quaternion.identity;
            goHero.transform.localScale = Vector3.one;
        }
    }

    public void SetBattleStateActionStart(bool isTurnOut)
    {
        ActiveBlur(!isTurnOut);
        ActiveOutline(false);

        var battleUI = Global.UIMgr.GetUIBattle();
        if (battleUI != null)
        {
            battleUI.ActiveAllBattleProfile(false);
            battleUI.ActiveHUDUI(isTurnOut);
            battleUI.ActiveTurnTimer(false);
        }

        if (isTurnOut == false)
        {
            ExcuteHeroAction();
        }
    }

    public void SetBattleStateActionEnd()
    {
        InitHeroTween();
        TurnUI.InitActiveTurnMember(ActiveTurnHeroNo);

        var battleUI = Global.UIMgr.GetUIBattle();
        if (battleUI != null)
        {
            battleUI.ActiveSelActionType(false);
            battleUI.ActiveHUDUI(true);
        }

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

        bool isWinner = false;
        EHeroBattleAction ActiveAction = ResultBattleAction(ActiveHero, TargetHero, ref isWinner);
        Vector3 vPos = Battleground.GetTeamPos(ActiveAction, ActiveHero.IsMyTeam);
        ActiveHero.ExcuteAction(ActiveAction, vPos, TargetHero, isWinner);

        EHeroBattleAction TargetHeroAction = ResultBattleAction(TargetHero, ActiveHero, ref isWinner);
        vPos = Battleground.GetTeamPos(TargetHeroAction, TargetHero.IsMyTeam);
        TargetHero.ExcuteAction(TargetHeroAction, vPos, ActiveHero, isWinner);
    }

    EHeroBattleAction ResultBattleAction(Hero mine, Hero yours, ref bool isWinner)
    {
        isWinner = false;

        // Win
        if (mine.ActionType == Hero.EAtionType.ACTION_ATK && yours.ActionType == Hero.EAtionType.ACTION_FAKE)
        {
            isWinner = true;
            return EHeroBattleAction.HeroAction_AtkWin;
        }
        else if (mine.ActionType == Hero.EAtionType.ACTION_COUNT && yours.ActionType == Hero.EAtionType.ACTION_ATK)
        {
            isWinner = true;
            return EHeroBattleAction.HeroAction_CntWin;
        }
        else if (mine.ActionType == Hero.EAtionType.ACTION_FAKE && yours.ActionType == Hero.EAtionType.ACTION_COUNT)
        {
            isWinner = true;
            return EHeroBattleAction.HeroAction_FakeWin;
        }
        // Defeat
        else if (mine.ActionType == Hero.EAtionType.ACTION_FAKE && yours.ActionType == Hero.EAtionType.ACTION_ATK)
        {
            return EHeroBattleAction.HeroAction_FakeDefeat;
        }
        else if (mine.ActionType == Hero.EAtionType.ACTION_ATK && yours.ActionType == Hero.EAtionType.ACTION_COUNT)
        {
            return EHeroBattleAction.HeroAction_AtkDefeat;
        }
        else if (mine.ActionType == Hero.EAtionType.ACTION_COUNT && yours.ActionType == Hero.EAtionType.ACTION_FAKE)
        {
            return EHeroBattleAction.HeroAction_CntDefeat;
        }
        else
        {
            // draw
            if (mine.IsMyTurn)
            {
                isWinner = true;
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

        var battleUI = Global.UIMgr.GetUIBattle();
        if (battleUI != null)
        {
            battleUI.ActiveBattleProfile(true, hero.IsMyTeam);
            battleUI.SetProfileUI(heroNo, true);
            battleUI.SetTurnTimer(Define.SELECT_TARGET_LIMITTIME, ETurnTimeType.TURNTIME_SEL_TARGET);
        }
    }

    public Hero GetHeroControl(int heroNo)
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
        int Idx = UnityEngine.Random.Range(0, mListMyHeroes.Count);
        Hero randomHero = mListMyHeroes[Idx];
        while (randomHero.IsDie)
        {
            Idx = UnityEngine.Random.Range(0, mListMyHeroes.Count);
            if (mListMyHeroes[Idx].IsDie == false)
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
