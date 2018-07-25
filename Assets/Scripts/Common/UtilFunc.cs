using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UtilFunc 
{
    public static Hero_Control CreateHero( Transform tParant, int iHeroNo, int iLv, bool bMyTeam )
    {
        GameObject goHero = new GameObject();
        Hero_Control hero = goHero.AddComponent<Hero_Control>();
        if (hero == null) return null;

        Guid uid = Guid.NewGuid();
        goHero.transform.parent = tParant;
        goHero.transform.name = uid.ToString();
        
        TB_Hero tbHero;
        if (TBManager.Instance.cont_Hero.TryGetValue(iHeroNo, out tbHero))
        {
            hero.HeroUid = uid;
            hero.HeroNo = iHeroNo;
            hero.HP = tbHero.mHP + Mathf.CeilToInt(((iLv - 1) * (tbHero.mHP * 0.1f)));
            hero.MaxHP = hero.HP;
            hero.Atk = tbHero.mAtk + Mathf.CeilToInt(((iLv - 1) * (tbHero.mAtk * 0.1f)));
            hero.Def = tbHero.mDef + Mathf.CeilToInt(((iLv - 1) * (tbHero.mDef * 0.1f)));
            hero.Speed = tbHero.mSpeed;
            hero.StResPath = tbHero.stResPath;
            hero.IsMyTeam = bMyTeam;

			GameObject goRes = VResources.Load<GameObject> (hero.StResPath);
            if (goRes == null) return null;

            GameObject go = GameObject.Instantiate(goRes) as GameObject;
            if (go != null)
            {
                go.transform.parent = goHero.transform;
                go.transform.name = "Obj";

                go.transform.position = Vector3.zero;
                if (bMyTeam)
                {
                    go.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                }
                else
                {
                    go.transform.rotation = Quaternion.identity;
                }

                go.transform.localScale = Vector3.one;

                hero.HeroObj = go;
            }

            // create hero hp
            if (BattleManager.Instance.BattleUI != null)
            {
                BattleManager.Instance.BattleUI.CreateHeroHp(uid, bMyTeam);
            }
        }

        return hero;
    }

    public static void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
            ChangeLayersRecursively(child, name);
        }
    }
}
