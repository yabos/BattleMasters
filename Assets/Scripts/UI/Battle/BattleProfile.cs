using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProfile : MonoBehaviour
{
    public GameObject mProfile;

    public UISprite mSpriteProfile;

    public HeroStatus mStatus;
    public Transform mTargetBtn;
    public Transform mSelectActionTypeMyTurn;
    public Transform mSelectActionTypeEnemyTurn;

    public void SetProfile(Hero_Control heroCont, bool isActiveHero)
    {
        mSpriteProfile.spriteName = heroCont.HeroNo.ToString();
        mStatus.InitStatus(heroCont);        
        SetActiveProfile(heroCont.IsMyTeam, isActiveHero);
        if (heroCont.IsMyTeam == false && isActiveHero == false)
        {
            //mStatus.SetPos(heroCont.transform.position);
            mTargetBtn.position = heroCont.transform.position;
        }
    }

    // BattleStateReady 상태에서 클릭으로 스테이터스 볼 때 사용
    public void BattleStateReadyOnlyProfile(Hero_Control heroCont)
    {
        ActiveProfile(true);
        ActiveTargetBtn(false);
        ActiveSpriteProfile(false);
        ActiveSelActionType(false, false);
        mStatus.InitStatus(heroCont);
    }

    void SetActiveProfile(bool myTeam, bool isActiveHero)
    {
        ActiveProfile(true);

        // 적 액티브 턴에는 꺼야된다.
        // 아군 엑티브만 켜야 됨
        ActiveTargetBtn(myTeam == false && isActiveHero == false);

        if (myTeam)
        {
            ActiveSpriteProfile(true);
        }
        else
        {
            ActiveSpriteProfile(isActiveHero);
        }

        TweenPosProfile();
    }

    public void TweenPosProfile()
    {
        var tp = mProfile.GetComponentInChildren<TweenPosition>();
        if (tp != null)
        {
            tp.enabled = true;
            tp.ResetToBeginning();            
        }
    }

    public void TweenPosSpriteProfile(bool isPlay)
    {
        ActiveSpriteProfile(true);

        var tp = mSpriteProfile.GetComponent<TweenPosition>();
        if (tp != null)
        {
            tp.enabled = true;
            tp.ResetToBeginning();
        }
    }

    public void OnTweenPosSpriteProfileFinish()
    {
        ActiveTargetBtn(false);
    }

    public void ActiveProfile(bool active)
    {
        mProfile.SetActive(active);
    }

    void ActiveTargetBtn(bool active)
    {
        if (mTargetBtn != null)
        {
            mTargetBtn.gameObject.SetActive(active);
        }
    }

    public void ActiveSpriteProfile(bool active)
    {
        mSpriteProfile.gameObject.SetActive(active);
    }

    public void ActiveSelActionType(bool active, bool myTurn)
    {
        if (active == false)
        {
            if (mSelectActionTypeMyTurn != null)
            {
                mSelectActionTypeMyTurn.gameObject.SetActive(false);
            }

            if (mSelectActionTypeEnemyTurn != null)
            {
                mSelectActionTypeEnemyTurn.gameObject.SetActive(false);
            }
        }
        else
        {            
            if (mSelectActionTypeMyTurn != null)
            {
                mSelectActionTypeMyTurn.gameObject.SetActive(myTurn);
            }

            if (mSelectActionTypeEnemyTurn != null)
            {
                mSelectActionTypeEnemyTurn.gameObject.SetActive(!myTurn);
            }
        }
    }
}
