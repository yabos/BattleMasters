using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBattleEvent
{
    UIEVENT_NONE,
    UIEVENT_SELECT_TARGET,
    UIEVENT_ACTION_ATK,
    UIEVENT_ACTION_COUNT,
    UIEVENT_ACTION_FAKE,
}

public class BattleEvent : MonoBehaviour
{
    public EBattleEvent uIEvent;

    public void SendEvent()
    {
        BattleManager.Instance.BattleUI.SendEvent(uIEvent);
    }
}
