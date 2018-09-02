using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateNormal : BattleState
{
    int BeforeHeroNo = 0;

    public override void Initialize(BattleScene owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override IEnumerator DoStart(byte[] data = null)
    {
        yield return base.DoStart();

        int place = 0;

        int heroNo = System.BitConverter.ToInt32(data, place);
        BattleHeroManager.Instance.ActiveHeroOutline(false);
        BattleHeroManager.Instance.SetHeroOutline(heroNo);
        m_Owner.SetActiveTurnHero(heroNo);

        m_Owner.TurnUI.TurnPause = true;

        TimeElapsed = 0;
        BeforeHeroNo = 0;

        if (m_Owner.GetActiveHeroTeam() == false)
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

        if (m_Owner.GetActiveHeroTeam())
        {
            if (Input.GetMouseButtonDown(0) && m_Owner.OnlyActionInput == false)
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray2D ray = new Ray2D(wp, Vector2.zero);
                RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

                foreach (var hit in hits)
                {
                    var heroCont = hit.collider.GetComponentInParent<BattleHero>();
                    if (heroCont == null) continue;
                    if (heroCont.IsMyTeam) continue;
                    if (BeforeHeroNo.Equals(heroCont.HeroNo)) continue;

                    m_Owner.ActiveTargetHeroNo = heroCont.HeroNo;
                    BattleHeroManager.Instance.SetHeroOutline(heroCont.HeroNo);

                    var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
                    if (battleUI != null)
                    {
                        battleUI.SetProfileUI(heroCont.HeroNo, false);
                        battleUI.ActiveBattleProfile(true, false);
                    }

                    BeforeHeroNo = heroCont.HeroNo;
                }
            }
        }       
    }
}
