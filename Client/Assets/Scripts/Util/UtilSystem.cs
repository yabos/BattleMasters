using UnityEngine;
using System;

public static class UtilSystem
{
    public static Hero CreateHero(GameObject goHero, Guid uid, int iHeroNo, bool MyTeam, int sortingOrder)
    {
        TB_Hero tbHero;
        if (TBManager.Instance.cont_Hero.TryGetValue(iHeroNo, out tbHero))
        {
            GameObject goRes = Global.ResourceMgr.Load<GameObject>(tbHero.mResPath);
            if (goRes == null) return null;

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

                Hero hero = go.GetComponent<Hero>();
                if (hero == null) return null;
                hero.InitHero(tbHero, uid, iHeroNo, MyTeam, sortingOrder, go);

                return hero;
            }
        }

        return null;
    }
}