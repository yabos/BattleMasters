using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTimer : MonoBehaviour
{
    public UILabel mLavelTurnTimer;


    public void SetTimer(float time)
    {
        mLavelTurnTimer.text = time.ToString() + " Sec";            
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
