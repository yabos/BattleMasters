using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eBattleState
{
    eBattle_Ready,
    eBattle_TurnStart,
    eBattle_SelAtk,
    eBattle_Action,
    eBattle_Win,
    eBattle_Lose,
    eBattle_End,
}

public enum EBattlePosType
{
    BPT_TRACE_FAKE,
    BPT_CNT_BREAK,
    BPT_FAKE_CNT,
    BPT_DRAW,
}

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

    eBattleState mBattleState = eBattleState.eBattle_Ready;

    List<Hero_Control> mListMyHeroes = new List<Hero_Control>();
    List<Hero_Control> mListEnemyHeroes = new List<Hero_Control>();

    Transform [] mBattlePosMyTeam = new Transform[4];
    Transform [] mBattlePosEnemy = new Transform[4];

    GameObject mBattleRoot;

    public BattleUI_Control BattleUI
    {
        get; set;
    }

    GameObject mBlur;

    int m_iLoadingState = 0;

    public int ActiveTurnHero
    {
        get; set;
    }

    public int ActiveTargetHero
    {
        get; set;
    }

    public eBattleState BattleState
    {
        set { mBattleState = value; }
        get { return mBattleState; }
    }

    public List<Hero_Control> ListMyHeroes
    {
        get { return mListMyHeroes; }
    }

    public List<Hero_Control> ListEnemyHeroes
    {
        get { return mListEnemyHeroes; }
    }

    bool bLoadingDone = false;
    public void InitBattleManager()
    {
        StartCoroutine(LoadBattleRoot());
    }

    IEnumerator LoadBattleRoot()
    {
        yield return null;

        GameObject goBattle = VResources.Load<GameObject>(ResourcePath.BattleRootPath);
        if (goBattle != null)
        {
            mBattleRoot = Instantiate(goBattle);
            if (mBattleRoot != null)
            {
                mBattleRoot.transform.parent = transform;
                mBattleRoot.transform.name = "Battle_Root";

                mBattleRoot.transform.position = Vector3.zero;
                mBattleRoot.transform.rotation = Quaternion.identity;
                mBattleRoot.transform.localScale = Vector3.one;

                mBlur = mBattleRoot.transform.Find("Blur").gameObject;
            }
        }

        BattleUI = UIManager.Instance().GetUI() as BattleUI_Control;

        bLoadingDone = true;
    }

    void Awake()
    {
        mBattleState = eBattleState.eBattle_Ready;
    }

    int beforeHeroNo = 0;
    void Update()
    {
        if (bLoadingDone == false) return;

        LoadingProcess();

        if (mBattleState == eBattleState.eBattle_TurnStart)
        {
            if (Input.GetMouseButtonDown(0) && ActiveTurnHero > 0)
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray2D ray = new Ray2D(wp, Vector2.zero);
                RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

                foreach (var hit in hits)
                {
                    var heroCont = hit.collider.GetComponentInParent<Hero_Control>();
                    if (heroCont == null) continue;
                    if (heroCont.MyTeam) continue;
                    if (beforeHeroNo.Equals(heroCont.HeroNo)) continue;

                    SetEnemyOutline(heroCont.HeroNo);
                    BattleUI.SetActiveTurnHeroUI(heroCont.HeroNo);
                    ActiveTargetHero = heroCont.HeroNo;
                    beforeHeroNo = heroCont.HeroNo;
                }
            }
        }        
    }    

    public void SetBattleStateSelActionType()
    {
        BattleState = eBattleState.eBattle_SelAtk;

        BattleUI.ActiveSelActionType(true);
        BattleUI.SetTurnTimer(Define.SELECT_ACTIONTYPE_LIMITTIME, ETurnTimeType.TURNTIME_SEL_ACTIONTYPE);
    }

    public void SetBattleStateActionStart()
    {
        BattleState = eBattleState.eBattle_Action;

        ActiveBlur(true);
        BattleUI.ActiveBattleProfile(false);
        BattleUI.ActiveHUDUI(false);        
        EnemyOutlineOff();
        HeroAction();
    }

    public void SetBattleStateActionEnd()
    {
        BattleState = eBattleState.eBattle_TurnStart;

        ActiveBlur(false);
        ActiveTurnHero = 0;
        ActiveTargetHero = 0;
    }

    void HeroAction()
    {
        var MyTeamHero = GetHeroControl(ActiveTurnHero);
        if (MyTeamHero != null)
        {
            UtilFunc.ChangeLayersRecursively(MyTeamHero.transform, "UI");
            Debug.Log("Player : " + MyTeamHero.ActionType);
        }

        var EnemyHero = GetHeroControl(ActiveTargetHero);
        if (EnemyHero != null)
        {
            UtilFunc.ChangeLayersRecursively(EnemyHero.transform, "UI");
            Debug.Log("Enemy : " + EnemyHero.ActionType);
        }

        EHeroBattleAction myHeroAction = GetActionState(MyTeamHero.ActionType, EnemyHero.ActionType);
        SetActionMode(myHeroAction, MyTeamHero);

        EHeroBattleAction enemyHeroAction = GetActionState(EnemyHero.ActionType, MyTeamHero.ActionType);
        SetActionMode(enemyHeroAction, EnemyHero);
    }

    void SetActionMode(EHeroBattleAction action, Hero_Control hero)
    {
        Vector3 vPos = GetTeamPos(action, hero.MyTeam);
        hero.SetPosition(vPos);
        hero.SetScale(new Vector3(Define.BATTLE_MOD_SCALE, Define.BATTLE_MOD_SCALE, Define.BATTLE_MOD_SCALE));
        hero.ChangeState(action);
    }

    Vector3 GetTeamPos(EHeroBattleAction stateAction, bool myTeam)
    {
        if (stateAction == EHeroBattleAction.HeroAction_AtkWin || stateAction == EHeroBattleAction.HeroAction_CntDefeat)
        {
            if (myTeam)
            {
                return mBattlePosMyTeam[(int)EBattlePosType.BPT_TRACE_FAKE].position;
            }
            else
            {
                return mBattlePosEnemy[(int)EBattlePosType.BPT_TRACE_FAKE].position;
            }
        }
        else if (stateAction == EHeroBattleAction.HeroAction_CntWin || stateAction == EHeroBattleAction.HeroAction_FakeDefeat)
        {
            if (myTeam)
            {
                return mBattlePosMyTeam[(int)EBattlePosType.BPT_CNT_BREAK].position;
            }
            else
            {
                return mBattlePosEnemy[(int)EBattlePosType.BPT_CNT_BREAK].position;
            }
        }
        else if (stateAction == EHeroBattleAction.HeroAction_FakeWin || stateAction == EHeroBattleAction.HeroAction_AtkDefeat)
        {
            if (myTeam)
            {
                return mBattlePosMyTeam[(int)EBattlePosType.BPT_FAKE_CNT].position;
            }
            else
            {
                return mBattlePosEnemy[(int)EBattlePosType.BPT_FAKE_CNT].position;
            }
        }
        else
        {
            if (myTeam)
            {
                return mBattlePosMyTeam[(int)EBattlePosType.BPT_DRAW].position;
            }
            else
            {
                return mBattlePosEnemy[(int)EBattlePosType.BPT_DRAW].position;
            }
        }
    }

    EHeroBattleAction GetActionState(EAtionType me, EAtionType your)
    {
        // Win
        if (me == EAtionType.ACTION_ATK && your == EAtionType.ACTION_FAKE)
        {
            return EHeroBattleAction.HeroAction_AtkWin;
        }
        else if (me == EAtionType.ACTION_COUNT && your == EAtionType.ACTION_ATK)
        {
            return EHeroBattleAction.HeroAction_CntWin;
        }
        else if (me == EAtionType.ACTION_FAKE && your == EAtionType.ACTION_COUNT)
        {
            return EHeroBattleAction.HeroAction_FakeWin;
        }
        // Defeat
        else if (me == EAtionType.ACTION_FAKE && your == EAtionType.ACTION_ATK)
        {
            return EHeroBattleAction.HeroAction_AtkDefeat;
        }
        else if (me == EAtionType.ACTION_ATK && your == EAtionType.ACTION_COUNT)
        {
            return EHeroBattleAction.HeroAction_CntDefeat;
        }
        else if (me == EAtionType.ACTION_COUNT && your == EAtionType.ACTION_FAKE)
        {
            return EHeroBattleAction.HeroAction_FakeDefeat;
        }
        else
        {
            // draw
            return EHeroBattleAction.HeroAction_Max;
        }
    }

    void LoadingProcess()
    {
        switch (m_iLoadingState)
        {
            case 0:
                StartCoroutine(CreateMap(10101));
                break;

            case 1:
                StartCoroutine(SetMyTeamHero());
                break;

            case 2:
                StartCoroutine(SetEnemyTeamHero());
                break;

            case 3:
                //activate all Outline scripts in game
                foreach (Outline vCurOutline in (Outline[])FindObjectsOfType(typeof(Outline)))
                {
                    vCurOutline.Initialise();
                }
                BattleUI.ActiveLoadingIMG (false);
                BattleUI.CreateTurnIcon();
                SoundManager.Instance().PlayBattleBGM(SoundManager.eBattleBGM.eBattleBGM_Normal);
                mBattleState = eBattleState.eBattle_TurnStart;
                m_iLoadingState++;
				break;
        }
    }

    IEnumerator CreateMap( int iMapNo )
    {
		GameObject goMap = VResources.Load<GameObject>(ResourcePath.MapLoadPath + iMapNo.ToString());
        if (goMap != null)
        { 
            GameObject Map = Instantiate( goMap ) as GameObject;
            if (Map != null)
            {
                Map.transform.parent = mBattleRoot.transform;
                Map.name = "Map";

                Map.transform.position = Vector3.zero;
                Map.transform.rotation = Quaternion.identity;
                //Map.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                mBattlePosMyTeam[(int)EBattlePosType.BPT_TRACE_FAKE] = Map.transform.Find("BattlePos/TraceFake/MyTeam");
                mBattlePosEnemy[(int)EBattlePosType.BPT_TRACE_FAKE] = Map.transform.Find("BattlePos/TraceFake/Enemy");

                mBattlePosMyTeam[(int)EBattlePosType.BPT_CNT_BREAK] = Map.transform.Find("BattlePos/CntBreak/MyTeam");
                mBattlePosEnemy[(int)EBattlePosType.BPT_CNT_BREAK] = Map.transform.Find("BattlePos/CntBreak/Enemy");

                mBattlePosMyTeam[(int)EBattlePosType.BPT_FAKE_CNT] = Map.transform.Find("BattlePos/FakeCnt/MyTeam");
                mBattlePosEnemy[(int)EBattlePosType.BPT_FAKE_CNT] = Map.transform.Find("BattlePos/FakeCnt/Enemy");

                mBattlePosMyTeam[(int)EBattlePosType.BPT_DRAW] = Map.transform.Find("BattlePos/Draw/MyTeam");
                mBattlePosEnemy[(int)EBattlePosType.BPT_DRAW] = Map.transform.Find("BattlePos/Draw/Enemy");
            }
        }

        yield return new WaitForEndOfFrame();

        m_iLoadingState++;
    }

    IEnumerator SetMyTeamHero()
    {
        Transform tTeam = transform.Find("Battle_Root/Team/MyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                Hero_Control hero = UtilFunc.CreateHero(tTeam, 1001 + i, 1, true);
                if (hero != null)
                {
                    Transform tSPos = transform.Find("Battle_Root/Map/RegenPos/MyTeam/" + i.ToString());
                    if (tSPos != null)
                    {
                        hero.transform.position = tSPos.position;
                        hero.transform.rotation = Quaternion.identity;
                        hero.transform.localScale = Vector3.one;
                    }

                    hero.InitHero(i + 1);
                    mListMyHeroes.Add(hero);
                }
            }
        }

        yield return new WaitForEndOfFrame();

        m_iLoadingState++;
    }

    IEnumerator SetEnemyTeamHero()
    {
        Transform tTeam = transform.Find("Battle_Root/Team/EnemyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                Hero_Control hero = UtilFunc.CreateHero(tTeam, 2001 + i, 1, false);
                if (hero != null)
                {
                    Transform tSPos = transform.Find("Battle_Root/Map/RegenPos/EnemyTeam/" + i.ToString());
                    if (tSPos != null)
                    {
                        hero.transform.position = tSPos.position;
                        hero.transform.rotation = Quaternion.identity;
                        hero.transform.localScale = Vector3.one;
                    }

                    hero.InitHero(i + 1);
                    mListEnemyHeroes.Add(hero);
                }
            }
        }

        yield return new WaitForEndOfFrame();

        m_iLoadingState++;
    }

    public void SetActiveTurnHero(int heroNo)
    {
        ActiveTurnHero = heroNo;
        var hero = GetHeroControl(heroNo);
        if (hero != null)
        {
            hero.MyTurn = true;
        }

        BattleUI.SetActiveTurnHeroUI(heroNo);
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

    void SetEnemyOutline(int heroNo)
    {
        foreach (var elem in mListEnemyHeroes)
        {
            elem.Outline.eraseRenderer = !elem.HeroNo.Equals(heroNo);
        }
    }

    void EnemyOutlineOff()
    {
        foreach (var elem in mListEnemyHeroes)
        {
            elem.Outline.eraseRenderer = true;
        }
    }

    public void ActiveBlur(bool active)
    {
        mBlur.SetActive(active);
    }
}