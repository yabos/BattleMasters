using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIcon : MonoBehaviour
{    
    UISprite mSprite;
    int mHeroNo;

    public float MoveSpeedCount
    {
        get; set;
    }

	// Use this for initialization
	void Awake ()
    {
        mSprite = GetComponent<UISprite>();
        MoveSpeedCount = 0;
        mHeroNo = 0;
    }

    void Update()
    {
        if (MoveSpeedCount > 0 && GameMain.Instance().BattleControl.ActiveTurnHero == 0)
        {
            float ratioMax = MoveSpeedCount / Define.TURN_MAX;
            var pos = transform.localPosition;
            float ratioPos = (pos.x + Define.TURNICON_END_POS_X) / Define.TURNICON_POS_X_LENGTH;

            if (ratioMax > ratioPos)
            {
                pos.x = (Define.TURNICON_POS_X_LENGTH * ratioMax) + Define.TURNICON_START_POS_X;              

                if (pos.x > Define.TURNICON_END_POS_X)
                {
                    pos.x = Define.TURNICON_END_POS_X;
                    GameMain.Instance().BattleControl.SetActiveTurnHero(mHeroNo);
                }

                transform.localPosition = pos;
            }
        }
    }

    public void SetTurnIcon(int heroNo)
    {
        mSprite.spriteName = heroNo.ToString();
        mHeroNo = heroNo;
    }

    public void SetDepth(int depth, bool equal = false)
    {
        mSprite.depth = depth;

        if (equal)
        {
            var pos = transform.localPosition;
            pos.y = 1000;
            transform.localPosition = pos;
        }
    }

    public void AddMoveSpeed(float speed)
    {
        MoveSpeedCount += speed;
    }
}
