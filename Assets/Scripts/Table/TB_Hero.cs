using UnityEngine;
using System.Collections;

public class TB_Hero 
{
    public int mHeroNo;
    public int mHP;
    public int mAtk;
    public int mDef;
    public int mSpeed;
    public EffectType mBaseAtkEfc;
    public string mBaseAtkSound;

    public string mResPath;

    public EffectType GetEffectType(int iType)
    {
        return (EffectType)(iType + 1);
    }
}
