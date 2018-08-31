using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateLoad : BattleState
{
    GameObject goBattlegound;

    public override void Initialize(BattleScene owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override IEnumerator DoStart(byte[] data = null)
    {
        yield return base.DoStart();

        LoadBattleground(10101);
        yield return LoadBattleHero();
        yield return LoadBattleEnemy();
        LoadEffects();

        foreach (Outline vCurOutline in (Outline[])Object.FindObjectsOfType(typeof(Outline)))
        {
            vCurOutline.Initialise();
        }

        m_Owner.TurnUI.CreateTurnIcon();
        Global.SoundMgr.PlayBGM(SoundManager.eBGMType.eBGM_Battle);

        m_StateManager.ChangeState(EBattleState.BattleState_Ready);
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }    

    protected void LoadBattleground(int iMapNo)
    {
        if (goBattlegound != null)
        {
            Object.Destroy(goBattlegound);
        }

        var goMap = Global.ResourceMgr.CreatePrefabResource(ResourcePath.MapLoadPath + iMapNo.ToString());
        if (goMap != null)
        {
            goBattlegound = Object.Instantiate(goMap.ResourceData) as GameObject;
            if (goBattlegound != null)
            {
                goBattlegound.name = "Map";

                goBattlegound.transform.position = Vector3.zero;
                goBattlegound.transform.rotation = Quaternion.identity;
                goBattlegound.transform.localScale = Vector3.one;
            }
        }

        m_Owner.Battleground = goBattlegound.GetComponent<Battleground>();
    }

    protected IEnumerator LoadBattleHero()
    {
        Transform tTeam = m_Owner.BattleRoot.transform.Find("Team/MyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return m_Owner.CreateBattleHero(tTeam, 1001 + i, true, (i + 1) * 10,  i);
            }
        }
    }

    protected IEnumerator LoadBattleEnemy()
    {
        Transform tTeam = m_Owner.BattleRoot.transform.Find("Team/EnemyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return m_Owner.CreateBattleHero(tTeam, 2001 + i, false, (i + 1) * 10, i);
            }
        }
    }

    protected void LoadEffects()
    {
        EffectManager.Instance.PreLoadEffect();
    }
}
