using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateWin : BattleState
{
    public override void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart();

        m_Owner.TurnUI.TurnPause = true;
        m_Owner.ActiveOutline(false);
        m_Owner.BattleUI.ActiveUI(eBattleUI.Win, true);
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }
}
