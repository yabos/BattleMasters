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

	SpriteRenderer mLoading = null;
    int m_iLoadingState = 0;

    Transform mBattleStartTo = null;
    List<NearPath> mListBattleEndPos = new List<NearPath>();

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

    public Transform BattleStartTo
    {
        get { return mBattleStartTo; }
    }

    public List<NearPath> ListBattleEndPos
    {
        get { return mListBattleEndPos; }
    }

    void Start()
    {
		mLoading = GetComponent<SpriteRenderer> ();

        mBattleState = eBattleState.eBattle_Ready;
    }

    void Update()
    {
        LoadingProcess();
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
                //AggroInit();
                m_iLoadingState++;
                break;

            case 4:
                InitBattlePos();
                break;

			case 5:
				BattleUI_Control ui = UIManager.Instance ().GetUI () as BattleUI_Control;
				ui.ActiveLoadingIMG (false);
                ui.CreateTurnIcon();
                m_iLoadingState++;
				break;
        }
    }

    IEnumerator CreateMap( int iMapNo )
    {
		GameObject goMap = VResources.Load<GameObject>(stMapLoadPath + iMapNo.ToString());
        if (goMap != null)
        { 
            GameObject Map = GameObject.Instantiate( goMap ) as GameObject;
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

    //void AggroInit()
    //{
    //    foreach (var nodeMy in mListMyHeroes)
    //    {
    //        foreach (var nodeEnemy in mListEnemyHeroes)
    //        {
    //            nodeMy.DicAggro.Add(nodeEnemy, 1);
    //        }
    //    }

    //    foreach (var nodeEnemy in mListEnemyHeroes)
    //    {
    //        foreach (var nodeMy in mListMyHeroes)
    //        {
    //            nodeEnemy.DicAggro.Add(nodeMy, 1);
    //        }
    //    }

    //    m_iLoadingState++;
    //}

    void InitBattlePos()
    {
        Transform tStartTo = transform.Find("Map/RegenPos/MyTeam/StartTo");
        if (tStartTo != null)
        {
            mBattleStartTo = tStartTo;
        }

        for (int i = 0; i < 6; ++i)
        {
            Transform tEndPath = transform.Find("Map/RegenPos/MyTeam/EndPath" + i.ToString());
            if (tEndPath == null) continue;
            NearPath np = new NearPath();
            np.mIsEntered = false;
            np.mTran = tEndPath;
            mListBattleEndPos.Add(np);
        }

        m_iLoadingState++;
    }

    public void CheckEndBattle()
    {
        bool bAliveEnemy = false;
        for (int i = 0; i < mListEnemyHeroes.Count; ++i)
        {
            if (!mListEnemyHeroes[i].IsDie)
            {
                bAliveEnemy = true;
            }
        }

        if (!bAliveEnemy)
        {
            mBattleState = eBattleState.eBattle_Win;
        }
    }
}