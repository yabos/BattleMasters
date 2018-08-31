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
    public UISprite SpriteBackImage;
    public UILabel LabelTurnTimer;

    float Time = 0;
    float TimeElapsed;
    ETurnTimeType Type;

    BattleScene mBattleScene;
    UIBattle Owner;

    public void SetTimer(UIBattle _owner, float time, ETurnTimeType type)
    {
        mBattleScene = Global.SceneMgr.CurrentScene as BattleScene;
        Owner = _owner;

        Type = type;
        Time = time;
        TimeElapsed = 0;

        LabelTurnTimer.text = time.ToString() + " Sec";
        if (type == ETurnTimeType.TURNTIME_SEL_ACTIONTYPE)
        {
            SpriteBackImage.spriteName = "stayToAttack";
        }
        else
        {
            SpriteBackImage.spriteName = "stayToTarget";
        }
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
                    if (mBattleScene.ActiveTargetHeroNo > 0)
                    {
                        Owner.SetBattleSelActionType();
                    }
                    else
                    {
                        // turn out
                        int place = 0;
                        byte[] data = new byte[128];
                        System.Buffer.BlockCopy(System.BitConverter.GetBytes(true), 0, data, place, sizeof(bool));
                        mBattleScene.BattleStateManager.ChangeState(EBattleState.BattleState_Action, data);
                    }
                }
                else
                {
                    // random att type
                    if (mBattleScene.ActiveTargetHeroNo > 0)
                    {
                        Hero.EAtionType actionType = (Hero.EAtionType)Random.Range(0, (int)Hero.EAtionType.ACTION_MAX);
                        Owner.SetHeroActionType(actionType);
                    }
                }
            }
        }
    }
}
