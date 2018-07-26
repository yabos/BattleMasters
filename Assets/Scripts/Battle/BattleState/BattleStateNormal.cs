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
        int place = 0;

        int heroNo = System.BitConverter.ToInt32(data, place);
        m_Owner.SetActiveTurnHero(heroNo);
        m_Owner.TurnUI.TurnPause = true;

        TimeElapsed = 0;
        BeforeHeroNo = 0;
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
        else
        {
            ///// 적이 사용하는 임시 AI 패턴///////
            if (BeforeHeroNo == 0)
            {
                // 살아있는 상대방 적 1인 랜덤으로 선택 해줌.
                int targetHeroNo = m_Owner.GetRandomHeroTeam();
                m_Owner.ActiveTargetHeroNo = targetHeroNo;
                m_Owner.SetOutlineHero(targetHeroNo);
                m_Owner.BattleUI.SetProfileUI(targetHeroNo, false);
                m_Owner.BattleUI.ActiveBattleProfile(true, true);

                // 한가지 공격 타입을 설정
                m_Owner.SetRandomActionType(m_Owner.ActiveTurnHeroNo);

                BeforeHeroNo = targetHeroNo;

                // 우리팀 방어측 선택 UI 색을 바꾼 파랑 ui를 띄워주게 작업하자!!
                m_Owner.BattleUI.ActiveSelActionType(true);
            }

            // 느낌 주기 위해서 3초 이후 엑션을 실행한다.
            if (TimeElapsed >= 3)
            {
                BattleManager.Instance.BattleStateManager.ChangeState(EBattleState.BattleState_Action);
            }
        }
    }
}
