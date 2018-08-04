using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEventListener : MonoBehaviour
{
    public void OnAtk()
    {
        var attackHero = transform.GetComponentInParent<Hero_Control>();
        if (attackHero != null)
        {
            TB_Hero tbHero;
            if (TBManager.Instance.cont_Hero.TryGetValue(attackHero.HeroNo, out tbHero))
            {
                SoundManager.Instance.PlaySoundOnce(tbHero.mBaseAtkSound);
            }

            if (attackHero.TargetHero != null)
            {
                attackHero.TargetHero.BeHit(attackHero);
            }
        }
    }
}
