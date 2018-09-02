using UnityEngine;
using System;
using System.Collections;

public class Hero : Actor
{
    public Guid HeroUid { get; protected set; }
    public int HeroNo { get; protected set; }
    public string HeroName { get; protected set; }
    public int HP { get; protected set; }
    public int MaxHP { get; protected set; }
    public int Atk { get; protected set; }
    public int Def { get; protected set; }
    public float Speed { get; protected set; }
   
    public GameObject HeroObj { get; protected set; }
    public Outline Outline { get; protected set; }
    public Transform Ef_HP { get; protected set; }
    public Transform Ef_Effect { get; protected set; }
    public Vector3 InitPos { get; protected set; }    

    public void InitHero(TB_Hero tbHero, Guid uid, int iHeroNo, GameObject heroObj)
    {
        HeroObj = heroObj;

        HeroUid = uid;
        HeroNo = iHeroNo;
        HeroName = tbHero.mHeroName;
        HP = tbHero.mHP;
        MaxHP = HP;
        Atk = tbHero.mAtk;
        Def = tbHero.mDef;
        Speed = tbHero.mSpeed;

        if (HeroObj != null)
        {
            Outline = HeroObj.GetComponent<Outline>();

            Ef_HP = HeroObj.transform.Find("ef_HP");
            if (Ef_HP == null)
            {
                Debug.LogError("Not Find ef_HP!");
            }

            Ef_Effect = HeroObj.transform.Find("ef_Center");
            if (Ef_Effect == null)
            {
                Debug.LogError("Not Find Ef_Effect!");
            }
        }
    }

    public void SetPosition(Vector3 vPos)
    {
        transform.position = vPos;
    }

    public void SetScale(Vector3 vScale)
    {
        transform.localScale = vScale;
    }
}