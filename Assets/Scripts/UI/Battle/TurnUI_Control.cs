using System.Collections.Generic;
using UnityEngine;

public class TurnUI_Control : MonoBehaviour
{
    private List<TurnIcon> listTurnIcons = new List<TurnIcon>();
    public List<TurnIcon> ListTurnIcons
    {
        get { return listTurnIcons; }
    }

    [System.NonSerialized]
    public bool TurnPause = true;

    float m_fTimeElapsed = 0;
    
	void Update ()
    {
        m_fTimeElapsed += Time.deltaTime;

        if (TurnPause == false)
        {
            if (m_fTimeElapsed >= 0.1f)
            {
                UpdateTurnSpeed();
                int heroNo = CheckActiveTurnHero();
                if (heroNo > 0)
                {
                    int place = 0;
                    byte[] data = new byte[128];
                    System.Buffer.BlockCopy(System.BitConverter.GetBytes(heroNo), 0, data, place, sizeof(int));
                    BattleManager.Instance.BattleStateManager.ChangeState(EBattleState.BattleState_Normal, data);

                    TurnPause = true;
                }

                m_fTimeElapsed -= 0.1f;
            }
        }
    }

    public void CreateTurnIcon()
    {
        CreateTurnIcon(BattleManager.Instance.ListMyHeroes);
        CreateTurnIcon(BattleManager.Instance.ListEnemyHeroes);
    }

    void CreateTurnIcon(List<Hero_Control> listHero)
    {
        for (int i = 0; i < listHero.Count; ++i)
        {
            GameObject goIcon = Instantiate(VResources.Load<GameObject>("UI/Battle/TurnIcon")) as GameObject;
            if (goIcon != null)
            {
                goIcon.transform.parent = transform;
                goIcon.name = listHero[i].HeroNo.ToString();

                goIcon.transform.localPosition = new Vector3(Define.TURNICON_START_POS_X, 0, 0);
                goIcon.transform.localRotation = Quaternion.identity;
                goIcon.transform.localScale = new Vector3(6, 6, 0);

                var icon = goIcon.GetComponent<TurnIcon>();
                if (icon != null)
                {
                    icon.SetTurnIcon(listHero[i].HeroNo);

                    ListTurnIcons.Add(icon);
                }
            }
        }
    }

    int CheckActiveTurnHero()
    {
        for (int i = 0; i < listTurnIcons.Count; ++i)
        {
            if (listTurnIcons[i].NotifyActiveTurn == true)
            {
                return listTurnIcons[i].HeroNo;
            }
        }

        return 0;
    }
        
    public void UpdateTurnSpeed()
    {
        List<Hero_Control> listTemp = new List<Hero_Control>();
        listTemp.AddRange(BattleManager.Instance.ListMyHeroes);
        listTemp.AddRange(BattleManager.Instance.ListEnemyHeroes);

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
            turnicon.InitTurn();
        }
    }
}
