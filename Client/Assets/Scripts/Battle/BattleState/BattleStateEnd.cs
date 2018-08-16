using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateEnd : BattleState
{
    public override void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart();
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }
}
