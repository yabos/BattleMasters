using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIcon : MonoBehaviour
{
    UISprite mSprite;

	// Use this for initialization
	void Awake ()
    {
        mSprite = GetComponent<UISprite>();
    }

    public void SetTurnIcon(int heroNo)
    {
        mSprite.spriteName = heroNo.ToString();
    }
}
