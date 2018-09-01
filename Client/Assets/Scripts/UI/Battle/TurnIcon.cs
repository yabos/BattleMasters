using UnityEngine;

public class TurnIcon : MonoBehaviour
{
    UITurnControl Owner;
    UISprite Sprite;

    public int HeroNo
    {
        get; set;
    }

    public float MoveSpeedCount
    {
        get; set;
    }
    
    // Use this for initialization
    void Awake ()
    {
        Sprite = GetComponent<UISprite>();
    }

    public void InitTurn(UITurnControl turnUI, int heroNo)
    {
        Owner = turnUI;
        MoveSpeedCount = 0;
        SetStartPos();
        SetTurnIcon(heroNo);
    }

    void SetTurnIcon(int heroNo)
    {
        Sprite.spriteName = heroNo.ToString();
        HeroNo = heroNo;
    }

    public void SetDepth(int depth)
    {
        Sprite.depth = depth;
    }

    public void AddMoveSpeed(float speed)
    {
        MoveSpeedCount += speed;

        UpdateTurnIconPos();
    }

    void UpdateTurnIconPos()
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

                //notify
                Owner.NotifyActiveTurn(HeroNo);
            }

            transform.localPosition = pos;
        }
    }

    void SetStartPos()
    {
        var pos = transform.localPosition;
        pos.x = Define.TURNICON_START_POS_X;
        transform.localPosition = pos;
    }
}
