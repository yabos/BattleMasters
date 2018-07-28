using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionDrawAtkDefeat : HeroBattleAction
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
        yield return MoveForward(2.0f, Define.MOVE_ATK_SPEED_X, Actor.AniType.ANI_ATK);

        yield return MoveBackward(2.0f, Define.MOVE_BACK_DEFEAT_SPEED_X, Actor.AniType.ANI_DEFEAT);

        m_Owner.ChangeState(EHeroBattleAction.HeroAction_Idle);
    }
}
