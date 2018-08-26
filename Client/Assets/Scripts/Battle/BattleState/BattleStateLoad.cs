using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateLoad : BattleState
{
    GameObject goBattlegound;

    public override void Initialize(BattleManager owner, BattleStateManager state_manager)
    {
        base.Initialize(owner, state_manager);
    }

    public override void DoStart(byte[] data = null)
    {
        base.DoStart();

        LoadBattleHUD();
        LoadBattleground(10101);
        LoadBattleHero();
        LoadBattleEnemy();
        LoadEffects();

        foreach (Outline vCurOutline in (Outline[])Object.FindObjectsOfType(typeof(Outline)))
        {
            vCurOutline.Initialise();
        }
        m_Owner.BattleUI.ActiveLoadingIMG(false);
        m_Owner.TurnUI.CreateTurnIcon();
        Global.SoundMgr.PlayBGM(SoundManager.eBGMType.eBGM_Battle);

        m_StateManager.ChangeState(EBattleState.BattleState_Ready);
    }

    public override void DoEnd()
    {
        base.DoEnd();
    }

    public void LoadBattleHUD()
    {
        GameObject goUI = Global.ResourceMgr.Load<GameObject>(ResourcePath.BattleUIPath);
        if (goUI != null)
        {
            GameObject uiRoot = Object.Instantiate(goUI) as GameObject;
            if (uiRoot != null)
            {
                uiRoot.transform.name = uiRoot.name;
                uiRoot.transform.parent = GameObject.FindGameObjectWithTag("UICamera").transform;

                uiRoot.transform.position = Vector3.zero;
                uiRoot.transform.rotation = Quaternion.identity;
                uiRoot.transform.localScale = Vector3.one;

                m_Owner.BattleUI = uiRoot.GetComponent<BattleUIManager>();
                m_Owner.TurnUI = uiRoot.GetComponentInChildren<TurnUI_Control>();
            }
        }
    }

    protected void LoadBattleground(int iMapNo)
    {
        if (goBattlegound != null)
        {
            Object.Destroy(goBattlegound);
        }

        GameObject goMap = Global.ResourceMgr.Load<GameObject>(ResourcePath.MapLoadPath + iMapNo.ToString());
        if (goMap != null)
        {
            goBattlegound = Object.Instantiate(goMap) as GameObject;
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

    protected void LoadBattleHero()
    {
        Transform tTeam = m_Owner.BattleRoot.transform.Find("Team/MyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                m_Owner.CreateBattleHero(tTeam, 1001 + i, true, (i + 1) * 10,  i);
            }
        }
    }

    protected void LoadBattleEnemy()
    {
        Transform tTeam = m_Owner.BattleRoot.transform.Find("Team/EnemyTeam");
        if (tTeam != null)
        {
            for (int i = 0; i < 4; ++i)
            {
                m_Owner.CreateBattleHero(tTeam, 2001 + i, false, (i + 1) * 10, i);
            }
        }
    }

    protected void LoadEffects()
    {
        EffectManager.Instance.PreLoadEffect();
    }
}
