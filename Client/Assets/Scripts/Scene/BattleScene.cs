using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleScene : SceneBase
{
    public GameObject BattleRoot { get; private set; }
    public BattleStateManager BattleStateManager { get; private set; }
    public BattleAIManager BattleAIManager { get; private set; }
    public Transform EffectRoot { get; private set; }
    public Battleground Battleground { get; set; }
    public UITurnControl TurnUI { get; set;}

    public int ActiveTurnHeroNo { get; set; }
    public int ActiveTargetHeroNo { get; set; }
    public bool OnlyActionInput { get; set; }

    private GameObject Blur;

    public override IEnumerator OnEnter(float progress)
    {
        yield return base.OnEnter(progress);

        Global.UIMgr.ShowLoadingWidget(999);

        yield return Global.UIMgr.OnCreateWidgetAsync<UIBattle>(UIManager.eUIType.eUI_Battle, widget =>
        {
            if (widget != null)
            {
                Global.SoundMgr.PlayBGM(SoundManager.eBGMType.eBGM_Battle);

                TurnUI = widget.GetComponentInChildren<UITurnControl>(true);

                widget.BattleScene = this;
                widget.Show();                
            }
        });

        yield return Global.UIMgr.OnCreateWidgetAsync<UIBattleWin>(UIManager.eUIType.eUI_BattleWin, widget => 
        {
            widget.Hide();
        });

        yield return Global.UIMgr.OnCreateWidgetAsync<UIBattleLose>(UIManager.eUIType.eUI_BattleLose, widget => 
        {
            widget.Hide();
        });

        yield return Global.UIMgr.OnCreateWidgetAsync<UIBattleEnd>(UIManager.eUIType.eUI_BattleEnd, widget =>
        {
            widget.Hide();
        });

        yield return Global.ResourceMgr.CreateResourceAsync( eResourceType.Prefab, "Battle/Prefabs/BattleRoot", (prefabResource) =>
        {
            BattleRoot = Instantiate(prefabResource.ResourceData) as GameObject;
            if (BattleRoot != null)
            {
                BattleRoot.name = "BattleRoot";

                BattleRoot.transform.position = Vector3.zero;
                BattleRoot.transform.rotation = Quaternion.identity;
                BattleRoot.transform.localScale = Vector3.one;

                Blur = ComponentFactory.FindInChildrenByName<Transform>(BattleRoot.transform, "Blur").gameObject;
            }
        });
        
        //BattleRoot
        EffectRoot = BattleRoot.transform.Find("Effect");

        BattleHeroManager.Instance.Init();

        if (BattleStateManager == null)
        {
            BattleStateManager = new BattleStateManager();            
        }

        yield return BattleStateManager.Initialize(this);

        if (BattleAIManager == null)
        {
            BattleAIManager = new BattleAIManager();            
        }

        BattleAIManager.Initialize();

        Global.UIMgr.HideLoadingWidget();
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


    public IEnumerator CreateBattleHero(Transform tParant, int iHeroNo, bool MyTeam, int sortingOrder, int spawnPos)
    {
        GameObject goHero = new GameObject();

        Guid uid = Guid.NewGuid();
        goHero.transform.parent = tParant;
        goHero.transform.name = uid.ToString();

        yield return BattleHeroManager.Instance.CreateHero(goHero, uid, iHeroNo, MyTeam, sortingOrder);

        SetBattleSpawnPos(goHero, MyTeam, spawnPos);

        // create hero hp
        var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
        if (battleUI != null)
        {
            battleUI.CreateHeroHp(uid, MyTeam);
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
        BattleHeroManager.Instance.ActiveHeroOutline(false);

        var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
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

        var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
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
        var hero = BattleHeroManager.Instance.GetHeroControl(ActiveTurnHeroNo);
        if (hero != null)
        {
            hero.InitHeroTween();
            hero.IsMyTurn = false;

            UtilFunc.ChangeLayersRecursively(hero.transform, Define.DEFAULT_LAYER);
        }

        hero = BattleHeroManager.Instance.GetHeroControl(ActiveTargetHeroNo);
        if (hero != null)
        {
            hero.InitHeroTween();

            UtilFunc.ChangeLayersRecursively(hero.transform, Define.DEFAULT_LAYER);
        }
    }

    void ExcuteHeroAction()
    {
        var ActiveHero = BattleHeroManager.Instance.GetHeroControl(ActiveTurnHeroNo);
        if (ActiveHero != null)
        {
            Debug.Log("ActiveHero Action : " + ActiveHero.ActionType);
        }

        var TargetHero = BattleHeroManager.Instance.GetHeroControl(ActiveTargetHeroNo);
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

        var hero = BattleHeroManager.Instance.GetHeroControl(heroNo);
        if (hero != null)
        {
            hero.IsMyTurn = true;
        }

        var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
        if (battleUI != null)
        {
            battleUI.ActiveBattleProfile(true, hero.IsMyTeam);
            battleUI.SetProfileUI(heroNo, true);
            battleUI.SetTurnTimer(Define.SELECT_TARGET_LIMITTIME, ETurnTimeType.TURNTIME_SEL_TARGET);
        }
    }    

    public void ActiveBlur(bool active)
    {
        Blur.SetActive(active);
    }

    public bool GetActiveHeroTeam()
    {
        var hero = BattleHeroManager.Instance.GetHeroControl(ActiveTurnHeroNo);
        if (hero != null)
        {
            return hero.IsMyTeam;
        }

        return false;
    }
}
