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
    
    public UILabel mLavelTurnTimer;

    float mTime = 0;

    ETurnTimeType mType;

    public void SetTimer(float time, ETurnTimeType type)
    {
        mType = type;
        mTime = time;

        mLavelTurnTimer.text = time.ToString() + " Sec";
        gameObject.SetActive(true);
    }

    float fElapsedTime;
	void Update ()
    {
        fElapsedTime += Time.deltaTime;
        if (fElapsedTime >= 1f)
        {
            mTime -= fElapsedTime;            
            mLavelTurnTimer.text = string.Format("{0:N0} Sec", mTime);
            fElapsedTime -= 1f;

            int iTime = Mathf.CeilToInt(mTime);
            if (iTime == 0)
            {
                gameObject.SetActive(false);

                if (mType == ETurnTimeType.TURNTIME_SEL_TARGET)
                {
                    if (GameMain.Instance().BattleControl.ActiveTargetHero > 0)
                    {
                        GameMain.Instance().BattleControl.BattleUI.SetBattleSelActionType();
                    }
                    else
                    {
                        // turn out
                    }
                }
                else
                {
                    // random att type
                }
            }
        }
    }
}
