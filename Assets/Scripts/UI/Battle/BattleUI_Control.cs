using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleUI_Control : BaseUI
{
	Transform BattleLoading = null;
    Transform HeroHp = null;
    Transform DamageRoot = null;

    BattleProfile[] Profiles = new BattleProfile[2];
    GameObject GoTurnTimer;
    TurnTimer TurnTime;

	// Use this for initialization
	void Awake ()
    {
        HeroHp = transform.Find("Anchor/HeroHP");
		BattleLoading = transform.Find ("Anchor/Loading");
        DamageRoot = transform.Find("Anchor/DamageRoot");

        var Tran = transform.Find("Anchor_BL/Profile");
        if (Tran != null)
        {
            Profiles[0] = Tran.GetComponent<BattleProfile>();
        }
        
        Tran = transform.Find("Anchor_BR/Profile");
        if (Tran != null)
        {
            Profiles[1] = Tran.GetComponent<BattleProfile>();
        }

        GoTurnTimer = transform.Find("Anchor/Timer").gameObject;
        if (GoTurnTimer != null)
        {
            TurnTime = GoTurnTimer.GetComponent<TurnTimer>();
        }
    }

    public override void SendEvent(EBattleEvent uIEvent)
    {
        if (uIEvent == EBattleEvent.UIEVENT_SELECT_TARGET)
        {
            SetBattleSelActionType();
        }
        else if (uIEvent == EBattleEvent.UIEVENT_ACTION_ATK)
        {
            SetHeroActionType(EAtionType.ACTION_ATK);            
        }
        else if (uIEvent == EBattleEvent.UIEVENT_ACTION_COUNT)
        {
            SetHeroActionType(EAtionType.ACTION_COUNT);
        }
        else if (uIEvent == EBattleEvent.UIEVENT_ACTION_FAKE)
        {
            SetHeroActionType(EAtionType.ACTION_FAKE);
        }
    }

    public void SetBattleSelActionType()
    {
        var profile = GetProfile(BattleManager.Instance.ActiveTargetHeroNo);
        if (profile != null)
        {
            profile.TweenPosSpriteProfile(true);            
            ActiveSelActionType(true, true);
            SetTurnTimer(Define.SELECT_ACTIONTYPE_LIMITTIME, ETurnTimeType.TURNTIME_SEL_ACTIONTYPE);

            BattleManager.Instance.OnlyActionInput = true;
        }
    }

    public void SetHeroActionType(EAtionType eAtionType)
    {
        int heroNo = BattleManager.Instance.ActiveTurnHeroNo;
        if (BattleManager.Instance.GetActiveHeroTeam() == false)
        {
            heroNo = BattleManager.Instance.ActiveTargetHeroNo;
        }
        
        var heroCont = BattleManager.Instance.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            heroCont.ActionType = eAtionType;
        }

        // 원래는 상대방의 입력 정보를 알아와야되는데
        // 지금은 AI로 대체 . 랜덤으로 타입을 정해준다.
        heroNo = BattleManager.Instance.ActiveTargetHeroNo;
        if (BattleManager.Instance.GetActiveHeroTeam() == false)
        {
            heroNo = BattleManager.Instance.ActiveTurnHeroNo;
        }
        BattleManager.Instance.BattleAIManager.SetRandomActionType(heroNo);

        BattleManager.Instance.BattleStateManager.ChangeState(EBattleState.BattleState_Action);
        BattleManager.Instance.OnlyActionInput = false;
    }

	public void ActiveLoadingIMG(bool bActive)
	{
		BattleLoading.gameObject.SetActive (bActive);
	}

    public void CreateHeroHp(System.Guid uid, bool bMyTeam)
    {
		GameObject goHPRes = VResources.Load<GameObject>("UI/Common/Prefabs/HPGauge");
        if (goHPRes == null) return;

        GameObject goHP = Instantiate(goHPRes) as GameObject;
        if (goHP != null)
        {
            goHP.transform.parent = HeroHp.transform;
            goHP.transform.name = uid.ToString();

            goHP.transform.position = Vector3.zero;
            goHP.transform.rotation = Quaternion.identity;
            goHP.transform.localScale = new Vector3(0.8f, 0.8f, 1);

            goHP.SetActive(true);
        }
    }

    public void UpdateHPGauge(System.Guid uid, float fFillAmountHp)
    {
        if (HeroHp == null) return;

        for (int i = 0; i < HeroHp.childCount; ++i)
        {
            Transform tChild = HeroHp.GetChild(i);
            if (tChild == null) continue;

            if (tChild.name.Equals(uid.ToString()))
            {
                Transform tSlider = tChild.Find("SpriteSlider");
                if (tSlider == null) continue;
                UISprite sprite = tSlider.GetComponent<UISprite>();
                if (sprite == null) continue;
                sprite.fillAmount = fFillAmountHp;
            }
        }
    }

    public void UpdatePosHPGauge(System.Guid uid, Transform tEf_HP)
    {
        if (HeroHp == null) return;

        for (int i = 0; i < HeroHp.childCount; ++i)
        {
            Transform tChild = HeroHp.GetChild(i);
            if (tChild == null) continue;

            if (tChild.name.Equals(uid.ToString()))
            {
                Vector3 vScreenPos = Camera.main.WorldToScreenPoint(tEf_HP.position);
                tChild.position = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(vScreenPos.x, vScreenPos.y, 0));
            }
        }
    }

    public void DestroyHPGauge(System.Guid uid)
    {
        if (HeroHp == null) return;

        for (int i = 0; i < HeroHp.childCount; ++i)
        {
            Transform tChild = HeroHp.GetChild(i);
            if (tChild == null) continue;

            if (tChild.name.Equals(uid.ToString()))
            {
                NGUITools.Destroy(tChild.gameObject);
            }
        }
    }

    BattleProfile GetProfile(int heroNo)
    {
        var heroCont = BattleManager.Instance.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            BattleProfile bp = null;
            if (heroCont.IsMyTeam)
            {
                bp = Profiles[0];

            }
            else
            {
                bp = Profiles[1];
            }

            return bp;
        }

        return null;
    }

    // 실제 공격 타입을 선택하는 UI 
    public void ActiveSelActionType(bool active, bool myTurn = false)
    {
        Profiles[0].ActiveSelActionType(active, myTurn);
    }

    public void SetTurnTimer(float fTime, ETurnTimeType type)
    {
        TurnTime.SetTimer(fTime, type);
        ActiveTurnTimer(true);
    }

    public void ActiveHUDUI(bool active)
    {
        BattleManager.Instance.TurnUI.ActiveTurnUI(active);
        ActiveHPUI(active);
    }

    public void SetProfileUI(int heroNo, bool isActiveHero)
    {
        var bp = GetProfile(heroNo);
        if (bp != null)
        {
            var heroCont = BattleManager.Instance.GetHeroControl(heroNo);
            if (heroCont != null)
            {
                bp.SetProfile(heroCont, isActiveHero);
            }
        }
    }

    public void SetReadyStateProfileUI(Hero_Control heroCont)
    {
        var bp = GetProfile(heroCont.HeroNo);
        if (bp != null)
        {
            bp.BattleStateReadyOnlyProfile(heroCont);
        }
    }

    void ActiveHPUI(bool active)
    {
        HeroHp.gameObject.SetActive(active);
    }

    public void ActiveTurnTimer(bool active)
    {
        GoTurnTimer.SetActive(active);
    }

    public void ActiveAllBattleProfile(bool active)
    {
        Profiles[0].ActiveProfile(active);
        Profiles[1].ActiveProfile(active);
    }

    public void ActiveBattleProfile(bool active, bool myTeam)
    {
        if (myTeam)
        {
            Profiles[0].gameObject.SetActive(active);
        }
        else
        {
            Profiles[1].gameObject.SetActive(active);
        }
    }

    public void CreateDamage(int iDamage, Vector3 vPos, bool bMyTeam)
    {
        GameObject goDamageRes = VResources.Load<GameObject>("UI/Common/Prefabs/HeroDamage");
        if (goDamageRes != null)
        {
            GameObject goDamage = Instantiate(goDamageRes) as GameObject;
            if (goDamage != null)
            {
                goDamage.transform.parent = DamageRoot;

                Vector2 vScreenPos = Camera.main.WorldToScreenPoint(new Vector2(vPos.x, vPos.y));
                goDamage.transform.position = UICamera.currentCamera.ScreenToWorldPoint(new Vector2(vScreenPos.x, vScreenPos.y));
                goDamage.transform.localPosition = new Vector3(goDamage.transform.localPosition.x, goDamage.transform.localPosition.y, 0);
                goDamage.transform.localRotation = Quaternion.identity;
                goDamage.transform.localScale = Vector3.one;
                Damage hd = goDamage.GetComponent<Damage>();
                if (hd != null)
                {
                    hd.m_LabelDamage.text = iDamage.ToString();                   
                }
            }
        }
    }
}