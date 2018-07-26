using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProfile : MonoBehaviour
{
    public GameObject mProfile;

    public UISprite mSpriteProfile;

    public HeroStatus mStatus;
    public Transform mTargetBtn;
    public Transform mSelectActionType;

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

    void SetActiveProfile(bool myTeam, bool isActiveHero)
    {
        ActiveProfile(true);
        ActiveTargetBtn(myTeam == false);

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

    public void ActiveSelActionType(bool active)
    {
        if (mSelectActionType != null)
        {
            mSelectActionType.gameObject.SetActive(active);
        }
    }
}
