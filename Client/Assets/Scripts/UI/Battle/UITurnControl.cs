﻿using System.Collections.Generic;
using UnityEngine;

public class UITurnControl : MonoBehaviour
{
    [System.NonSerialized]
    public List<TurnIcon> ListTurnIcons = new List<TurnIcon>();

    [System.NonSerialized]
    public bool TurnPause = true;

    float m_fTimeElapsed = 0;
    
    public void CreateTurnIcon()
    {
        CreateTurnIcon(BattleHeroManager.Instance.ListMyHeroes);
        CreateTurnIcon(BattleHeroManager.Instance.ListEnemyHeroes);
    }

    void CreateTurnIcon(List<BattleHero> listHero)
    {
        for (int i = 0; i < listHero.Count; ++i)
        {
            var resource = Global.ResourceMgr.CreateUIResource(ResourcePath.TurnIconPath, true);
            GameObject goIcon = Instantiate(resource.ResourceData) as GameObject;
            if (goIcon != null)
            {
                goIcon.transform.parent = transform;
                goIcon.name = listHero[i].HeroNo.ToString();

                goIcon.transform.localPosition = new Vector3(Define.TURNICON_START_POS_X, 0, 0);
                goIcon.transform.localRotation = Quaternion.identity;
                goIcon.transform.localScale = Vector3.one;

                var icon = goIcon.GetComponent<TurnIcon>();
                if (icon != null)
                {
                    icon.InitTurn(this, listHero[i].HeroNo);

                    ListTurnIcons.Add(icon);
                }
            }
        }
    }

    public void DestroyTurnIcon()
    {
        foreach (var elem in ListTurnIcons)
        {
            GameObject.Destroy(elem.gameObject);
        }

        ListTurnIcons.Clear();
    }

    void Update()
    {
        m_fTimeElapsed += Time.deltaTime;

        if (TurnPause == false)
        {
            if (m_fTimeElapsed >= 0.1f)
            {
                UpdateTurnSpeed();
                m_fTimeElapsed = 0;
            }
        }
    }

    public void UpdateTurnSpeed()
    {
        List<BattleHero> listTemp = new List<BattleHero>();
        listTemp.AddRange(BattleHeroManager.Instance.ListMyHeroes);
        listTemp.AddRange(BattleHeroManager.Instance.ListEnemyHeroes);

        for (int i = 0; i < listTemp.Count; ++i)
        {
            UpdateTurnIconSpeed(listTemp[i].HeroNo, listTemp[i].Speed);
        }

        UpdateTurnIconDepth();
    }

    void UpdateTurnIconSpeed(int heroNo, float speed)
    {
        if (ListTurnIcons == null || ListTurnIcons.Count == 0) return;

        for (int i = 0; i < ListTurnIcons.Count; ++i)
        {
            if (ListTurnIcons[i].name.Equals(heroNo.ToString()))
            {
                ListTurnIcons[i].AddMoveSpeed(speed);
            }
        }
    }

    void UpdateTurnIconDepth()
    {
        if (ListTurnIcons == null || ListTurnIcons.Count == 0) return;

        ListTurnIcons.Sort(ComparerDepth);

        for (int i = 0; i < ListTurnIcons.Count; ++i)
        {
            ListTurnIcons[i].SetDepth((ListTurnIcons.Count - i) + 1); // depth 가 최소 1 이상으로 하기 위해서 1 더함
        }
    }

    int ComparerDepth(TurnIcon lhs, TurnIcon rhs)
    {
        int now = rhs.MoveSpeedCount.CompareTo(lhs.MoveSpeedCount);
        if (now == 0)
            return rhs.name.CompareTo(lhs.name);
        else
            return now;
    }
    
    public void ActiveTurnUI(bool active)
    {
        gameObject.SetActive(active);
    }

    public void InitActiveTurnMember(int heroNo)
    {
        var turnicon = ListTurnIcons.Find(x => x.HeroNo.Equals(heroNo));
        if (turnicon != null)
        {
            turnicon.InitTurn(this, heroNo);
        }
    }

    public void NotifyActiveTurn(int heroNo)
    {
        int place = 0;
        byte[] data = new byte[128];
        System.Buffer.BlockCopy(System.BitConverter.GetBytes(heroNo), 0, data, place, sizeof(int));

        var battleScene = Global.SceneMgr.CurrentScene as BattleScene;
        if (battleScene != null)
        {
            battleScene.BattleStateManager.ChangeState(EBattleState.BattleState_Normal, data);
        }
    }

    public void DestroyTurnIcon(int heroNo)
    {
        var turnIcon = ListTurnIcons.Find(x => x.HeroNo == heroNo);
        if (turnIcon != null)
        {
            NGUITools.Destroy(turnIcon.gameObject);
            ListTurnIcons.Remove(turnIcon);
        }        
    }
}
