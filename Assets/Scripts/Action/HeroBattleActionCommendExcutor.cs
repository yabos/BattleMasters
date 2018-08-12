using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBattleActionCommendExcutor
{   
    delegate IEnumerator Func(params object[] list);

    Dictionary<string, Func> DicCommand = new Dictionary<string, Func>();  

    public static readonly string AnimDelay = "AnimationDelay";
    public static readonly string MoveF = "MoveForward";
    public static readonly string MoveFM = "MoveForwardMoment";
    public static readonly string MoveB = "MoveBackward";
    public static readonly string MoveBM = "MoveBackwardMoment";
    public static readonly string FadeOut = "FadeOut";

    public string[] ClipName = new string[]
    {
        "AnimationDelay",
        "MoveForward",
        "MoveForwardMoment",
        "MoveBackward",
        "MoveBackwardMoment",
        "FadeOut",
};

    public void Initialize(HeroBattleActionManager actionManager)
    {
        for (int i = 0; i < ClipName.Length; ++i)
        {
            AddCommend(ClipName[i], actionManager);
        }
    }

    void AddCommend(string commend, HeroBattleActionManager actionManager)
    {
        if (commend.Equals(AnimDelay))
        {
            DicCommand.Add(commend, new Func(actionManager.AnimationDelay));
        }
        else if (commend.Equals(MoveF))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveForward));
        }
        else if (commend.Equals(MoveFM))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveForwardMoment));
        }
        else if (commend.Equals(MoveB))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveBackward));
        }
        else if (commend.Equals(MoveBM))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveBackwardMoment));
        }
        else if (commend.Equals(FadeOut))
        {
            DicCommand.Add(commend, new Func(actionManager.FadeOut));
        }
    }

    public IEnumerator Excute(string commend, params object[] list)
    {
        if (DicCommand.ContainsKey(commend))
        {
            yield return DicCommand[commend](list);
        }
        else
        {
            string paramlist = null;
            foreach (var elem in list)
            {
                paramlist += elem + " ";
            }

            Debug.LogError("Do not find commendkey : " + commend);
            Debug.LogError("paramlist : " + paramlist);
        }
    }

    // ActionMaker Tools Only.
    #region ActionMaker Tool
    public void Initialize(ActionMaker actionMaker)
    {
        for (int i = 0; i < ClipName.Length; ++i)
        {
            AddCommend(ClipName[i], actionMaker);
        }
    }

    // ActionMaker Tools Only.
    void AddCommend(string commend, ActionMaker actionManager)
    {
        if (commend.Equals(AnimDelay))
        {
            DicCommand.Add(commend, new Func(actionManager.AnimationDelay));
        }
        else if (commend.Equals(MoveF))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveForward));
        }
        else if (commend.Equals(MoveFM))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveForwardMoment));
        }
        else if (commend.Equals(MoveB))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveBackward));
        }
        else if (commend.Equals(MoveBM))
        {
            DicCommand.Add(commend, new Func(actionManager.MoveBackwardMoment));
        }
        else if (commend.Equals(FadeOut))
        {
            DicCommand.Add(commend, new Func(actionManager.FadeOut));
        }
    }
    #endregion
}
