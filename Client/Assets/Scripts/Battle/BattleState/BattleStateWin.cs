﻿using System.Collections;
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
