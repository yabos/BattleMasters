using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateWin : BattleState
{
    public override void Initialize(BattleScene owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart();

        m_Owner.TurnUI.TurnPause = true;
        m_Owner.ActiveOutline(false);

        var battleUI = Global.UIMgr.GetUIBattle();
        if (battleUI != null)
        {
            battleUI.ActiveUI(eBattleUI.Win, true);
        }
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }
}
