using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearPath
{
    public bool mIsEntered = false;
    public Transform mTran = null;
}

public class Battle_Control : MonoBehaviour
{
    public enum eBattleState
    {
        eBattle_Ready,
        eBattle_Ing,
        eBattle_Win,
        eBattle_Lose,
        eBattle_End,
    }

    public static readonly string stMapLoadPath = "Map/";

    eBattleState mBattleState = eBattleState.eBattle_Ready;

    List<Hero_Control> mListMyHeroes = new List<Hero_Control>();
    List<Hero_Control> mListEnemyHeroes = new List<Hero_Control>();
    BattleUI_Control mBattleUI;

    SpriteRenderer mLoading = null;
    int m_iLoadingState = 0;

    public int ActiveTurnHero
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
        mBattleUI = UIManager.Instance().GetUI() as BattleUI_Control;
    }

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

                    SetEnemyOutline(heroCont.HeroNo);
                    mBattleUI.SetActiveTurnHeroUI(heroCont.HeroNo);
                }
            }
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
                mBattleUI.ActiveLoadingIMG (false);
                mBattleUI.CreateTurnIcon();
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
        mBattleUI.SetActiveTurnHeroUI(heroNo);
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
}