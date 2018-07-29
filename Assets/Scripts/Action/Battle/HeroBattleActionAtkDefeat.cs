using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionAtkDefeat : HeroBattleAction
{
    public override void Initialize(Hero_Control owner, HeroBattleActionManager action_manager)
    {
        base.Initialize(owner, action_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart(data);

        UtilFunc.ChangeLayersRecursively(m_Owner.transform, "UI");

        m_Owner.StartCoroutine(ActionProc());
    }

    public override void DoEnd(EHeroBattleAction eNextAction)
    {
        base.DoEnd(eNextAction);

        m_Owner.StopCoroutine(ActionProc());
    }

    public override IEnumerator ActionProc()
    {
        // 튕겨내지는 모션
        //yield return AnimationDeley(0.1f, Actor.AniType.ANI_BREAK);
        yield return MoveBackwardDistance(0.1f, 5, Actor.AniType.ANI_BREAK);

        // 처맞는 모션
        //yield return MoveBackward(0.5f, Define.MOVE_BACK_DEFEAT_SPEED_X, Actor.AniType.ANI_DEFEAT);

        m_Owner.ChangeState(EHeroBattleAction.HeroAction_Idle);
    }
}
