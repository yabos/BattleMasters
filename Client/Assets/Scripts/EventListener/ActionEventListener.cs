using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEventListener : MonoBehaviour
{
    public void OnAtk()
    {
        var attackHero = transform.GetComponentInParent<Hero>();
        if (attackHero != null)
        {
            TB_Hero tbHero;
            if (Global.TBMgr.cont_Hero.TryGetValue(attackHero.HeroNo, out tbHero))
            {
                Global.SoundMgr.PlaySoundOnce(tbHero.mBaseAtkSound);
            }

            if (attackHero.BattleTargetHero != null)
            {
                attackHero.BattleTargetHero.BeHit(attackHero);
            }
        }
    }
}
