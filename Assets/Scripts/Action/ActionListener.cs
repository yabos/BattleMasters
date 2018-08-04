using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListener : MonoBehaviour
{
    public void OnAtk()
    {
        var activeHero = BattleManager.Instance.GetHeroControl(BattleManager.Instance.ActiveTurnHeroNo);

    }
}
