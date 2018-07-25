using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateAction : BattleState
{
    public override void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {        
        BattleManager.Instance.SetBattleStateActionStart();
    }

    public override void DoEnd()
    {
        base.DoEnd();

        BattleManager.Instance.SetBattleStateActionEnd();
    }

    public override void Update(float fTimeDelta)
    {
      
    }
}
