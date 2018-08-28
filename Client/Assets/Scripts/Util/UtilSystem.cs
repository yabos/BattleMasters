using UnityEngine;
using System;

public static class UtilSystem
{
    static MonoBehaviour mono = new MonoBehaviour();

    public static Hero CreateHero(GameObject goHero, Guid uid, int iHeroNo, bool MyTeam, int sortingOrder)
    {
        TB_Hero tbHero;
        if (Global.TBMgr.cont_Hero.TryGetValue(iHeroNo, out tbHero))
        {
            Hero hero = new Hero();

            var resourceAsync = mono.StartCoroutine(Global.ResourceMgr.CreateResourceAsync(eResourceType.Prefab, tbHero.mResPath, (resource) =>
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

                        hero = go.GetComponent<Hero>();
                        if (hero != null)
                        {
                            hero.InitHero(tbHero, uid, iHeroNo, MyTeam, sortingOrder, go);
                        }                        
                    }
                }
            }));

            return hero;
        }

        return null;
    }
}