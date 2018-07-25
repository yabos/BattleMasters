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
        m_Owner.TurnUI.TurnPause = false;

        TimeElapsed = 0;
    }

    public override void DoEnd()
    {
        base.DoEnd();

        // 스테이터스 UI 켜진거 꺼줘야 함
    }

    public override void Update(float fTimeDelta)
    {
        base.Update(fTimeDelta);
        TimeElapsed += fTimeDelta;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(wp, Vector2.zero);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

            foreach (var hit in hits)
            {
                var heroCont = hit.collider.GetComponentInParent<Hero_Control>();
                if (heroCont == null) continue;               
                if (BeforeHeroNo.Equals(heroCont.HeroNo)) continue;

                m_Owner.SetOutlineHero(heroCont.HeroNo);
                // 스테이터스 UI 만 나오게 나중에 수정해야함
                //m_Owner.BattleUI.SetProfileUI(heroCont.HeroNo);              
                BeforeHeroNo = heroCont.HeroNo;
            }
        }
    }
}
