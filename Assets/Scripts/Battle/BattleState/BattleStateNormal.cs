using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateNormal : BattleState
{
    int BeforeHeroNo = 0;

    public override void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart();

        int place = 0;

        int heroNo = System.BitConverter.ToInt32(data, place);
        m_Owner.ActiveOutline(false);
        m_Owner.SetOutlineHero(heroNo);
        m_Owner.SetActiveTurnHero(heroNo);
        m_Owner.TurnUI.TurnPause = true;        

        TimeElapsed = 0;
        BeforeHeroNo = 0;

        if (BattleManager.Instance.GetActiveHeroTeam() == false)
        {
            m_Owner.BattleAIManager.ProceserAI();
        }
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }

    public override void Update(float fTimeDelta)
    {
        base.Update(fTimeDelta);
        TimeElapsed += fTimeDelta;

        if (BattleManager.Instance.GetActiveHeroTeam())
        {
            if (Input.GetMouseButtonDown(0) && m_Owner.OnlyActionInput == false)
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray2D ray = new Ray2D(wp, Vector2.zero);
                RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

                foreach (var hit in hits)
                {
                    var heroCont = hit.collider.GetComponentInParent<Hero_Control>();
                    if (heroCont == null) continue;
                    if (heroCont.IsMyTeam) continue;
                    if (BeforeHeroNo.Equals(heroCont.HeroNo)) continue;

                    m_Owner.ActiveTargetHeroNo = heroCont.HeroNo;
                    m_Owner.SetOutlineHero(heroCont.HeroNo);
                    m_Owner.BattleUI.SetProfileUI(heroCont.HeroNo, false);
                    m_Owner.BattleUI.ActiveBattleProfile(true, false);

                    BeforeHeroNo = heroCont.HeroNo;
                }
            }
        }       
    }
}
