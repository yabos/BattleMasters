using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProfile : MonoBehaviour
{
    public GameObject mProfile;

    public UISprite mSpriteProfile;

    public void SetProfile(Hero_Control heroCont)
    {
        mSpriteProfile.spriteName = heroCont.HeroNo.ToString();
    }

    public void ActiveProfile(bool active)
    {
        mProfile.SetActive(active);

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
