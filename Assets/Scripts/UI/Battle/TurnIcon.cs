using UnityEngine;

public class TurnIcon : MonoBehaviour
{    
    UISprite mSprite;

    public int HeroNo
    {
        get; set;
    }

    public float MoveSpeedCount
    {
        get; set;
    }
    
    public bool NotifyActiveTurn
    {
        get; set;
    }

    // Use this for initialization
    void Awake ()
    {
        mSprite = GetComponent<UISprite>();

        InitTurn();
    }

    public void InitTurn()
    {
        MoveSpeedCount = 0;
        NotifyActiveTurn = false;
        SetStartPos();
    }

    public void SetTurnIcon(int heroNo)
    {
        mSprite.spriteName = heroNo.ToString();
        HeroNo = heroNo;
    }

    public void SetDepth(int depth)
    {
        mSprite.depth = depth;
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
                NotifyActiveTurn = true;

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
