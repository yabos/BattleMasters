using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateWin : BattleState
{
    public override void Initialize(BattleScene owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override IEnumerator DoStart(byte[] data = null)
    {
        yield return base.DoStart();

        m_Owner.TurnUI.TurnPause = true;

        BattleHeroManager.Instance.ActiveHeroOutline(false);

        var btlWin = Global.UIMgr.GetUI<UIBattleWin>(UIManager.eUIType.eUI_BattleWin);
        if (btlWin != null)
        {
            btlWin.Show();
        }
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }
}
