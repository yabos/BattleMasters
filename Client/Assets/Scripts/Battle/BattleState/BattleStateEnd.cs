using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateEnd : BattleState
{
    public override void Initialize(BattleScene owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override IEnumerator DoStart(byte[] data = null)
    {
        yield return base.DoStart();
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }
}
