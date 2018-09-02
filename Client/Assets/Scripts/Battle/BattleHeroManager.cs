using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleHeroManager : Singleton<BattleHeroManager>
{
    readonly List<BattleHero> mListMyHeroes = new List<BattleHero>();
    readonly List<BattleHero> mListEnemyHeroes = new List<BattleHero>();

    public List<BattleHero> ListMyHeroes
    {
        get { return mListMyHeroes; }
    }

    public List<BattleHero> ListEnemyHeroes
    {
        get { return mListEnemyHeroes; }
    }

    public void Init()
    {
        mListMyHeroes.Clear();
        mListEnemyHeroes.Clear();
    }

    public IEnumerator CreateHero(GameObject goHero, Guid uid, int iHeroNo, bool MyTeam, int sortingOrder)
    {
        TB_Hero tbHero;
        if (Global.TBMgr.cont_Hero.TryGetValue(iHeroNo, out tbHero))
        {
            yield return Global.ResourceMgr.CreateResourceAsync(eResourceType.Prefab, tbHero.mResPath, (resource) =>
            {
                GameObject goRes = resource.ResourceData as GameObject;
                if (goRes != null)
                {
                    GameObject go = UnityEngine.Object.Instantiate(goRes) as GameObject;
                    if (go != null)
                    {
                        go.transform.parent = goHero.transform;
                        go.transform.name = "Obj";

                        go.transform.position = Vector3.zero;
                        if (MyTeam)
                        {
                            go.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                            go.GetComponent<Outline>().color = 2;
                        }
                        else
                        {
                            go.transform.rotation = Quaternion.identity;
                            go.GetComponent<Outline>().color = 0;
                        }

                        go.transform.localScale = Vector3.one;

                        var hero = go.GetComponent<BattleHero>();
                        if (hero != null)
                        {
                            hero.InitHero(tbHero, uid, iHeroNo, MyTeam, sortingOrder, go);
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
                }
            });
        }
    }

    public BattleHero GetHeroControl(int heroNo)
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

    public void SetHeroOutline(int heroNo)
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

    public void ActiveHeroOutline(bool active)
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

    public bool CheckAction()
    {
        bool myHeroAction = false;
        foreach (var elem in mListMyHeroes)
        {
            if (elem.IsAction)
            {
                myHeroAction = true;
            }
        }

        bool enemyHeroAction = false;
        foreach (var elem in mListEnemyHeroes)
        {
            if (elem.IsAction)
            {
                enemyHeroAction = true;
            }
        }

        return myHeroAction || enemyHeroAction;
    }    

    // AI 용도. 상대팀 살아있는 한명 랜덤으로 넘겨줌
    // 나중에 조건을 검색해서 넘겨줄수도 있음
    public int GetRandomHeroTeam()
    {
        int Idx = UnityEngine.Random.Range(0, mListMyHeroes.Count);
        BattleHero randomHero = mListMyHeroes[Idx];
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