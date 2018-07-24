using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateReady : BattleState
{
    int BeforeHeroNo = 0;

    public override void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        m_fTimeElapsed = 0;
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }

    public override void Update(float fTimeDelta)
    {
        base.Update(fTimeDelta);
        m_fTimeElapsed += fTimeDelta;

        if (Input.GetMouseButtonDown(0) && m_Owner.ActiveTurnHero > 0)
        {
            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(wp, Vector2.zero);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

            foreach (var hit in hits)
            {
                var heroCont = hit.collider.GetComponentInParent<Hero_Control>();
                if (heroCont == null) continue;
                if (heroCont.MyTeam) continue;
                if (BeforeHeroNo.Equals(heroCont.HeroNo)) continue;

                m_Owner.SetEnemyOutline(heroCont.HeroNo);
                m_Owner.BattleUI.SetActiveTurnHeroUI(heroCont.HeroNo);
                m_Owner.ActiveTargetHero = heroCont.HeroNo;
                BeforeHeroNo = heroCont.HeroNo;
            }
        }

        if (BattleManager.Instance.ActiveTurnHero == 0)
        {
            if (m_fTimeElapsed >= 0.1f)
            {
                m_Owner.BattleUI.UpdateTurnCount();
                m_fTimeElapsed -= 0.1f;
            }
        }
    }
}
