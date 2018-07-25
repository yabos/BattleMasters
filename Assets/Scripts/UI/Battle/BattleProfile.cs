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

    public void SetProfile(Hero_Control heroCont)
    {
        mSpriteProfile.spriteName = heroCont.HeroNo.ToString();

        mStatus.InitStatus(heroCont);
        if (heroCont.IsMyTeam == false)
        {
            mTargetBtn.position = heroCont.transform.position;
        }

        SetActiveProfile(true, heroCont.IsMyTeam);
    }

    void SetActiveProfile(bool active, bool myTeam)
    {
        ActiveProfile(active);
        ActiveTargetBtn(active);

        if (myTeam == true)
        {
            mSpriteProfile.gameObject.SetActive(active);            
        }
        else
        {
            mSpriteProfile.gameObject.SetActive(false);
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
        mSelectActionType.gameObject.SetActive(active);
    }
}
