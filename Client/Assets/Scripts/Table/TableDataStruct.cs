using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_Hero
{
    public int mHeroNo;
    public string mHeroName;
    public string mElemental;
    public int mHP;
    public int mAtk;
    public int mDef;
    public int mSpeed;
    public int mCritical_Rate;
    public int mCriticalDamage_Rate;
    public int mGrowUp_TableGroupID;
    public int mPassive_SkillID;
    public int mAtk_SkillID;
    public int mCut_SkillID;
    public int mDod_SkillID;
    public int mActive_SkillID;
    
    public string mResPath;
    public string mChar_Illust;
    public string mChar_Icon;

    public string mBaseAtkEfc;
    public string mBaseAtkSound;
}

public class TB_Const
{
    public string mNameTag;
    public float mValuel;
}

public enum eConstType
{
    TurnPoint,
    MaxSkillGage,
    Def_Const,
    TargetSelectTime,
    AttackSelectTIme,
    AffectTargetSelectTime,
    Clean_DEF_Const,
    Crash_DEF_Const,
}