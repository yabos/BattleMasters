using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Battle_Control : MonoBehaviour
{
    public enum eBattleState
    {
        eBattle_Ready,
        eBattle_Ing,
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

    public static readonly string stMapLoadPath = "Map/";

    eBattleState mBattleState = eBattleState.eBattle_Ready;

    List<Hero_Control> mListMyHeroes = new List<Hero_Control>();
    List<Hero_Control> mListEnemyHeroes = new List<Hero_Control>();

    Transform [] mBattlePosMyTeam = new Transform[4];
    Transform [] mBattlePosEnemy = new Transform[4];

    public BattleUI_Control BattleUI
    {
        get; set;
    }

    public GameObject mBlur;

    SpriteRenderer mLoading = null;
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

    void Start()
    {
		mLoading = GetComponent<SpriteRenderer> ();

        mBattleState = eBattleState.eBattle_Ready;
        BattleUI = UIManager.Instance().GetUI() as BattleUI_Control;
    }

    int beforeHeroNo = 0;
    void Update()
    {
        LoadingProcess();

       if (mBattleState == eBattleState.eBattle_Ing)
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

    public void SetBattleStateAction()
    {
        BattleState = eBattleState.eBattle_Action;

        mBlur.SetActive(true);
        BattleUI.ActiveBattleProfile(false);
        BattleUI.ActiveHUDUI(false);        
        EnemyOutlineOff();
        HeroAction();
    }

    void HeroAction()
    {
        var MyTeamHero = GetHeroControl(ActiveTurnHero);
        if (MyTeamHero != null)
        {
            UtilFunc.ChangeLayersRecursively(MyTeamHero.transform, "UI");
            Debug.LogError("Player : " + MyTeamHero.ActionType);
        }

        var EnemyHero = GetHeroControl(ActiveTargetHero);
        if (EnemyHero != null)
        {
            UtilFunc.ChangeLayersRecursively(EnemyHero.transform, "UI");
            Debug.LogError("Enemy : " + EnemyHero.ActionType);
        }

        eHeroState myHeroAction = GetActionState(MyTeamHero.ActionType, EnemyHero.ActionType);
        Vector3 vPos = GetTeamPos(myHeroAction, true);
        MyTeamHero.ChangeState(myHeroAction, vPos);

        eHeroState enemyHeroAction = GetActionState(EnemyHero.ActionType, MyTeamHero.ActionType);
        vPos = GetTeamPos(enemyHeroAction, false);
        EnemyHero.ChangeState(enemyHeroAction, vPos);
    }

    Vector3 GetTeamPos(eHeroState stateAction, bool myTeam)
    {
        if (stateAction == eHeroState.HEROSTATE_TRACE_ATK || stateAction == eHeroState.HEROSTATE_BREAK_DEFEAT)
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
        else if (stateAction == eHeroState.HEROSTATE_CNT_ATK || stateAction == eHeroState.HEROSTATE_CNT_DEFEAT)
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
        else if (stateAction == eHeroState.HEROSTATE_FAKE_ATK || stateAction == eHeroState.HEROSTATE_FAKE_DEFEAT)
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

    eHeroState GetActionState(EAtionType me, EAtionType your)
    {
        // Win
        if (me == EAtionType.ACTION_ATK && your == EAtionType.ACTION_FAKE)
        {
            return eHeroState.HEROSTATE_TRACE_ATK;
        }
        else if (me == EAtionType.ACTION_COUNT && your == EAtionType.ACTION_ATK)
        {
            return eHeroState.HEROSTATE_CNT_ATK;
        }
        else if (me == EAtionType.ACTION_FAKE && your == EAtionType.ACTION_COUNT)
        {
            return eHeroState.HEROSTATE_FAKE_ATK;
        }
        // Defeat
        else if (me == EAtionType.ACTION_FAKE && your == EAtionType.ACTION_ATK)
        {
            return eHeroState.HEROSTATE_FAKE_DEFEAT;
        }
        else if (me == EAtionType.ACTION_ATK && your == EAtionType.ACTION_COUNT)
        {
            return eHeroState.HEROSTATE_BREAK_DEFEAT;
        }
        else if (me == EAtionType.ACTION_COUNT && your == EAtionType.ACTION_FAKE)
        {
            return eHeroState.HEROSTATE_CNT_DEFEAT;
        }
        else
        {
            return eHeroState.HEROSTATE_DRAW;
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
                mBattleState = eBattleState.eBattle_Ing;
                m_iLoadingState++;
				break;
        }
    }

    IEnumerator CreateMap( int iMapNo )
    {
		GameObject goMap = VResources.Load<GameObject>(stMapLoadPath + iMapNo.ToString());
        if (goMap != null)
        { 
            GameObject Map = Instantiate( goMap ) as GameObject;
            if (Map != null)
            {
                Map.transform.parent = transform;
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
        Transform tTeam = transform.Find("Team/MyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                Hero_Control hero = UtilFunc.CreateHero(tTeam, 1001 + i, 1, true);
                if (hero != null)
                {
                    Transform tSPos = transform.Find("Map/RegenPos/MyTeam/" + i.ToString());
                    if (tSPos != null)
                    {
                        hero.transform.position = tSPos.position;
                        hero.transform.rotation = Quaternion.identity;
                        hero.transform.localScale = Vector3.one;
                    }

                    hero.InitHero();
                    hero.SR.sortingOrder = i + 1;

                    mListMyHeroes.Add(hero);
                }
            }
        }

        yield return new WaitForEndOfFrame();

        m_iLoadingState++;
    }

    IEnumerator SetEnemyTeamHero()
    {
        Transform tTeam = transform.Find("Team/EnemyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                Hero_Control hero = UtilFunc.CreateHero(tTeam, 2001 + i, 1, false);
                if (hero != null)
                {
                    Transform tSPos = transform.Find("Map/RegenPos/EnemyTeam/" + i.ToString());
                    if (tSPos != null)
                    {
                        hero.transform.position = tSPos.position;
                        hero.transform.rotation = Quaternion.identity;
                        hero.transform.localScale = Vector3.one;
                    }

                    hero.InitHero();
                    hero.SR.sortingOrder = i + 1;

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
            elem.GetOutline().eraseRenderer = !elem.HeroNo.Equals(heroNo);
        }
    }

    void EnemyOutlineOff()
    {
        foreach (var elem in mListEnemyHeroes)
        {
            elem.GetOutline().eraseRenderer = true;
        }
    }
}