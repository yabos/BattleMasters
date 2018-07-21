using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProfile : MonoBehaviour
{
    public GameObject mProfile;

    public UISprite mSpriteProfile;

    public HeroStatus mStatus;

    public void SetProfile(Hero_Control heroCont)
    {
        mSpriteProfile.spriteName = heroCont.HeroNo.ToString();

        mStatus.InitStatus(heroCont);
        if (heroCont.MyTeam == false)
        {
            mStatus.SetPos(heroCont.transform.position);
        }

        ActiveProfile(true, heroCont.MyTeam);
    }

    void ActiveProfile(bool active, bool myTeam)
    {
        mProfile.SetActive(active);

        if (myTeam == true)
        {
            mSpriteProfile.gameObject.SetActive(active);            
        }
        else
        {
            mSpriteProfile.gameObject.SetActive(false);
        }

        TweenPosProfile(true);
    }

    public void TweenPosProfile(bool isPlay)
    {
        var tp = mProfile.GetComponentInChildren<TweenPosition>();
        if (tp != null)
        {
            if (isPlay)
            {
                tp.PlayForward();
            }
            else
            {
                tp.ResetToBeginning();
            }
        }
    }
}
