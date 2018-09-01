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
        var btlWin = Global.UIMgr.GetUI<UIBattleEnd>(UIManager.eUIType.eUI_BattleEnd);
        if (btlWin != null)
        {
            btlWin.Show(0.5f);
        }
        
        m_Owner.TurnUI.DestroyTurnIcon();

        yield return base.DoStart();
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }
}
