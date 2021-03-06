﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStatus : MonoBehaviour
{
    public UILabel mName;
    public UILabel mHP;
    public UILabel mSpeed;
    public UILabel mAtk;
    public UILabel mCri;
    public UILabel mDef;
    public UILabel mResist;

    public UILabel[] mPassive = new UILabel[3];

    public void InitStatus(BattleHero heroCont)
    {
        mName.text = heroCont.HeroName;
        mHP.text = heroCont.HP + "/" + heroCont.MaxHP;
        mSpeed.text = heroCont.Speed.ToString();
        mAtk.text = heroCont.Atk.ToString();
        mCri.text = heroCont.Def.ToString();

        var tw = GetComponent<TweenPosition>();
        if (tw != null)
        {
            tw.enabled = true;
            tw.ResetToBeginning();
        }
    }

    public void SetPos(Vector3 vWorldPos)
    {
        transform.position = vWorldPos;
    }
}
