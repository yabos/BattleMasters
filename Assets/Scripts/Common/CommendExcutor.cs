using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommendExcutor : MonoBehaviour
{
    private static CommendExcutor _instance;
    public static CommendExcutor Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(CommendExcutor)) as CommendExcutor;
                if (_instance == null)
                {
                    GameObject manaer = new GameObject("CommendExcutor", typeof(CommendExcutor));
                    _instance = manaer.GetComponent<CommendExcutor>();
                }
            }

            return _instance;
        }
    }

    delegate IEnumerator Func(params object[] list);

    Dictionary<string, Func> DicCommand = new Dictionary<string, Func>();  

    public static readonly string AnimDelay = "AnimationDelay";
    public static readonly string MoveF = "MoveForward";
    public static readonly string MoveFM = "MoveForwardMoment";
    public static readonly string MoveB = "MoveBackward";
    public static readonly string MoveBM = "MoveBackwardMoment";

    public string[] ClipName = new string[]
    {
        "AnimationDelay",
        "MoveForward",
        "MoveForwardMoment",
        "MoveBackward",
        "MoveBackwardMoment",
    };

    private void Awake()
    {
        for (int i = 0; i < ClipName.Length; ++i)
        {
            AddCommend(ClipName[i]);
        }
    }

    public void AddCommend(string commend)
    {
        //if (commend.Equals(AnimDelay))
        //{
        //    DicCommand.Add(commend, new Func(BattleManager.Instance.AnimationDeley));
        //}
        //else if (commend.Equals(MoveF))
        //{
        //    DicCommand.Add(commend, new Func(BattleManager.Instance.PlayAnim));
        //}
        //else if (commend.Equals(MoveFM))
        //{
        //    DicCommand.Add(commend, new Func(BattleManager.Instance.PlayAnim));
        //}
        //else if (commend.Equals(MoveB))
        //{
        //    DicCommand.Add(commend, new Func(BattleManager.Instance.PlayAnim));
        //}
        //else if (commend.Equals(MoveBM))
        //{
        //    DicCommand.Add(commend, new Func(BattleManager.Instance.PlayAnim));
        //}
    }

    public IEnumerator Excute(string commend, params object[] list)
    {
        if (DicCommand.ContainsKey(commend))
        {
            yield return DicCommand[commend](list);
        }
    }   
}
