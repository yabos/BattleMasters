using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETurnTimeType
{
    TURNTIME_SEL_TARGET,
    TURNTIME_SEL_ACTIONTYPE,
}

public class TurnTimer : MonoBehaviour
{    
    public UILabel LabelTurnTimer;

    float Time = 0;
    float TimeElapsed;
    ETurnTimeType Type;

    public void SetTimer(float time, ETurnTimeType type)
    {
        Type = type;
        Time = time;
        TimeElapsed = 0;

        LabelTurnTimer.text = time.ToString() + " Sec";
        gameObject.SetActive(true);
    }
    
	void Update ()
    {
        TimeElapsed += UnityEngine.Time.deltaTime;
        if (TimeElapsed >= 1f)
        {
            Time -= TimeElapsed;            
            LabelTurnTimer.text = string.Format("{0:N0} Sec", Time);
            TimeElapsed -= 1f;

            int iTime = Mathf.CeilToInt(Time);
            if (iTime == 0)
            {
                if (Type == ETurnTimeType.TURNTIME_SEL_TARGET)
                {
                    if (BattleManager.Instance.ActiveTargetHeroNo > 0)
                    {
                        BattleManager.Instance.BattleUI.SetBattleSelActionType();
                    }
                    else
                    {
                        // turn out
                        int place = 0;
                        byte[] data = new byte[128];
                        System.Buffer.BlockCopy(System.BitConverter.GetBytes(true), 0, data, place, sizeof(bool));
                        BattleManager.Instance.BattleStateManager.ChangeState(EBattleState.BattleState_Action, data);
                    }
                }
                else
                {
                    // random att type
                    if (BattleManager.Instance.ActiveTargetHeroNo > 0)
                    {
                        EAtionType actionType = (EAtionType)Random.Range(0, (int)EAtionType.ACTION_MAX);
                        BattleManager.Instance.BattleUI.SetHeroActionType(actionType);
                    }
                }
            }
        }
    }
}
