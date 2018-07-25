using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleUI_Control : BaseUI
{
	Transform mBattleLoading = null;
    Transform mHeroHp = null;    

    BattleProfile[] mProfiles = new BattleProfile[2];
    GameObject mGoTurnTimer;
    TurnTimer mTurnTime;

	// Use this for initialization
	void Awake ()
    {
        mHeroHp = transform.Find("Anchor/HeroHP");
		mBattleLoading = transform.Find ("Anchor/Loading");        
        var Tran = transform.Find("Anchor_BL/Profile");
        if (Tran != null)
        {
            mProfiles[0] = Tran.GetComponent<BattleProfile>();
        }
        
        Tran = transform.Find("Anchor_BR/Profile");
        if (Tran != null)
        {
            mProfiles[1] = Tran.GetComponent<BattleProfile>();
        }

        mGoTurnTimer = transform.Find("Anchor/Timer").gameObject;
        if (mGoTurnTimer != null)
        {
            mTurnTime = mGoTurnTimer.GetComponent<TurnTimer>();
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
        var profile = GetProfile(BattleManager.Instance.ActiveTargetHero);
        if (profile != null)
        {
            profile.TweenPosSpriteProfile(true);
            ActiveSelActionType(true);
            SetTurnTimer(Define.SELECT_ACTIONTYPE_LIMITTIME, ETurnTimeType.TURNTIME_SEL_ACTIONTYPE);
        }
    }

    void SetHeroActionType(EAtionType eAtionType)
    {
        int heroNo = BattleManager.Instance.ActiveTurnHero;
        var heroCont = BattleManager.Instance.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            heroCont.ActionType = eAtionType;
        }

        // 원래는 상대방의 입력 정보를 알아와야되는데
        // 지금은 AI로 대체 . 랜덤으로 타입을 정해준다.
        heroNo = BattleManager.Instance.ActiveTargetHero;
        heroCont = BattleManager.Instance.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            heroCont.ActionType = EAtionType.ACTION_ATK;
            //heroCont.ActionType = (EAtionType)Random.Range(0, (int)EAtionType.ACTION_MAX);
        }

        BattleManager.Instance.BattleStateManager.ChangeState(EBattleState.BattleState_Action);        
    }

	public void ActiveLoadingIMG(bool bActive)
	{
		mBattleLoading.gameObject.SetActive (bActive);
	}

    public void CreateHeroHp(System.Guid uid, bool bMyTeam)
    {
		GameObject goHPRes = VResources.Load<GameObject>("UI/Common/Prefabs/HPGauge");
        if (goHPRes == null) return;

        GameObject goHP = Instantiate(goHPRes) as GameObject;
        if (goHP != null)
        {
            goHP.transform.parent = mHeroHp.transform;
            goHP.transform.name = uid.ToString();

            goHP.transform.position = Vector3.zero;
            goHP.transform.rotation = Quaternion.identity;
            goHP.transform.localScale = new Vector3(3,3,1);

            goHP.SetActive(true);
        }
    }

    public void UpdateHPGauge(System.Guid uid, float fFillAmountHp)
    {
        if (mHeroHp == null) return;

        for (int i = 0; i < mHeroHp.childCount; ++i)
        {
            Transform tChild = mHeroHp.GetChild(i);
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
        if (mHeroHp == null) return;

        for (int i = 0; i < mHeroHp.childCount; ++i)
        {
            Transform tChild = mHeroHp.GetChild(i);
            if (tChild == null) continue;

            if (tChild.name.Equals(uid.ToString()))
            {
                tChild.position = tEf_HP.position;
            }
        }
    }

    public void DestroyHPGauge(System.Guid uid)
    {
        if (mHeroHp == null) return;

        for (int i = 0; i < mHeroHp.childCount; ++i)
        {
            Transform tChild = mHeroHp.GetChild(i);
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
            if (heroCont.MyTeam)
            {
                bp = mProfiles[0];

            }
            else
            {
                bp = mProfiles[1];
            }

            return bp;
        }

        return null;
    }

    public void ActiveSelActionType(bool active)
    {
        var profile = GetProfile(BattleManager.Instance.ActiveTurnHero);
        if (profile != null)
        {
            profile.ActiveSelActionType(active);
        }
    }

    public void SetTurnTimer(float fTime, ETurnTimeType type)
    {
        mTurnTime.SetTimer(fTime, type);
        ActiveTurnTimer(true);
    }

    public void ActiveHUDUI(bool active)
    {
        BattleManager.Instance.TurnUI.ActiveTurnUI(active);
        ActiveHPUI(active);
        ActiveTurnTimer(active);        
    }

    public void SetProfileUI(int heroNo)
    {
        var bp = GetProfile(heroNo);
        if (bp != null)
        {
            var heroCont = BattleManager.Instance.GetHeroControl(heroNo);
            if (heroCont != null)
            {
                bp.SetProfile(heroCont);
            }
        }
    }

    void ActiveHPUI(bool active)
    {
        mHeroHp.gameObject.SetActive(active);
    }

    void ActiveTurnTimer(bool active)
    {
        mGoTurnTimer.SetActive(active);
    }

    public void ActiveBattleProfile(bool active)
    {
        mProfiles[0].gameObject.SetActive(active);
        mProfiles[1].gameObject.SetActive(active);
    }
}