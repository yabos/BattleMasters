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
            yield return m_Owner.CreateBattleHero(tTeam, 9, true, (0 + 1) * 10, 0);
            yield return m_Owner.CreateBattleHero(tTeam, 10, true, (1 + 1) * 10, 1);
            yield return m_Owner.CreateBattleHero(tTeam, 2, true, (2 + 1) * 10, 2);
            yield return m_Owner.CreateBattleHero(tTeam, 3, true, (3 + 1) * 10, 3);
        }
    }

    protected IEnumerator LoadBattleEnemy()
    {
        Transform tTeam = m_Owner.BattleRoot.transform.Find("Team/EnemyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return m_Owner.CreateBattleHero(tTeam, 5 + i, false, (i + 1) * 10, i);
            }
        }
    }

    protected void LoadEffects()
    {
        EffectManager.Instance.PreLoadEffect();
    }
}
