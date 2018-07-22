using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleUI_Control : BaseUI
{
	Transform mBattleLoading = null;
    Transform mHeroHp = null;
    Transform mTrunIconRoot;

    public List<TurnIcon> mListTurnIcons = new List<TurnIcon>();

    Battle_Control mBattle_Control;

    BattleProfile[] mProfiles = new BattleProfile[2];
    GameObject mGoTurnTimer;
    TurnTimer mTurnTime;

	// Use this for initialization
	void Start ()
    {
        mHeroHp = transform.Find("Anchor/HeroHP");
		mBattleLoading = transform.Find ("Anchor/Loading");
        mTrunIconRoot = transform.Find("Anchor/Turn");
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

    // Update is called once per frame
    float ElapsedTime = 0;
    void Update ()
    {
        ElapsedTime += Time.deltaTime;

        if (GetBC() != null && GetBC().ActiveTurnHero == 0)
        {
            if (ElapsedTime >= 0.1f)
            {
                UpdateTurnCount();
                ElapsedTime -= 0.1f;
            }
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
            SetHeroActionType(Hero_Control.EAtionType.ACTION_ATK);            
        }
        else if (uIEvent == EBattleEvent.UIEVENT_ACTION_COUNT)
        {
            SetHeroActionType(Hero_Control.EAtionType.ACTION_COUNT);
        }
        else if (uIEvent == EBattleEvent.UIEVENT_ACTION_FAKE)
        {
            SetHeroActionType(Hero_Control.EAtionType.ACTION_FAKE);
        }
    }

    void SetHeroActionType(Hero_Control.EAtionType eAtionType)
    {
        int heroNo = GetBC().ActiveTurnHero;
        var heroCont = mBattle_Control.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            heroCont.ActionType = eAtionType;
        }

        // 원래는 상대방의 입력 정보를 알아와야되는데
        // 지금은 AI로 대체 . 랜덤으로 타입을 정해준다.
        heroNo = GetBC().ActiveTargetHero;
        heroCont = mBattle_Control.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            heroCont.ActionType = (Hero_Control.EAtionType)Random.Range(0, (int)Hero_Control.EAtionType.ACTION_MAX);
        }

        GetBC().SetBattleStateAction();
    }

    public void SetBattleSelActionType()
    {
        var profile = GetProfile(GetBC().ActiveTargetHero);
        if (profile != null)
        {
            profile.TweenPosSpriteProfile(true);
            GetBC().SetBattleStateSelActionType();
        }
    }

    Battle_Control GetBC()
    {
        if (mBattle_Control == null)
        {
            mBattle_Control = GameMain.Instance().BattleControl;
            if (mBattle_Control == null)
                return null;
            else
                return mBattle_Control;
        }
        else
        {
            return mBattle_Control;
        }
    }

	public void ActiveLoadingIMG(bool bActive)
	{
		mBattleLoading.gameObject.SetActive (bActive);
	}

    public void CreateHeroHp(System.Guid uid, bool bMyTeam)
    {
		GameObject goHPRes = VResources.Load<GameObject>("UI/Common/Prefabs/HPGauge");
        if (goHPRes == null) return;

        GameObject goHP = GameObject.Instantiate(goHPRes) as GameObject;
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

    public void CreateTurnIcon()
    {
        CreateTurnIcon(GetBC().ListMyHeroes);
        CreateTurnIcon(GetBC().ListEnemyHeroes);
    }

    void CreateTurnIcon(List<Hero_Control> listHero)
    {
        for (int i = 0; i < listHero.Count; ++i)
        {
            GameObject goIcon = GameObject.Instantiate(VResources.Load<GameObject>("UI/Battle/TurnIcon")) as GameObject;
            if (goIcon != null)
            {
                goIcon.transform.parent = mTrunIconRoot;
                goIcon.name = listHero[i].HeroNo.ToString();

                goIcon.transform.localPosition = new Vector3(Define.TURNICON_START_POS_X, 0, 0); 
                goIcon.transform.localRotation = Quaternion.identity;
                goIcon.transform.localScale = new Vector3(6, 6, 0);

                var icon = goIcon.GetComponent<TurnIcon>();
                if (icon != null)
                {
                    icon.SetTurnIcon(listHero[i].HeroNo);

                    mListTurnIcons.Add(icon);
                }
            }
        }
    }
    
    void UpdateTurnCount()
    {
        Battle_Control bc = GetBC();
        if (bc != null)
        {
            if (bc.BattleState == Battle_Control.eBattleState.eBattle_Ing)
            {
                List<Hero_Control> listTemp = new List<Hero_Control>();
                listTemp.AddRange(bc.ListMyHeroes);
                listTemp.AddRange(bc.ListEnemyHeroes);

                for (int i = 0; i < listTemp.Count; ++i)
                {
                    UpdateTurnIconSpeed(listTemp[i].HeroNo, listTemp[i].Speed);
                }

                UpdateTurnIconDepth();
            }
        }
    }

    void UpdateTurnIconSpeed(int heroNo, float speed)
    {
        if (mListTurnIcons == null || mListTurnIcons.Count == 0) return;

        for (int i = 0; i < mListTurnIcons.Count; ++i)
        {
            if (mListTurnIcons[i].name.Equals(heroNo.ToString()))
            {
                mListTurnIcons[i].AddMoveSpeed(speed);
            }
        }
    }

    void UpdateTurnIconDepth()
    {
        if (mListTurnIcons == null || mListTurnIcons.Count == 0) return;

        mListTurnIcons.Sort(ComparerDepth);

        for (int i = 0; i < mListTurnIcons.Count; ++i)
        {
            mListTurnIcons[i].SetDepth((mListTurnIcons.Count - i) + 1); // depth 가 최소 1 이상으로 하기 위해서 1 더함
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

    public void SetActiveTurnHeroUI(int heroNo)
    {
        var bp = GetProfile(heroNo);
        if (bp != null)
        {
            var heroCont = mBattle_Control.GetHeroControl(heroNo);
            if (heroCont != null)
            {
                bp.SetProfile(heroCont);
            }
        }
    }

    BattleProfile GetProfile(int heroNo)
    {
        var heroCont = mBattle_Control.GetHeroControl(heroNo);
        if (heroCont != null)
        {
            BattleProfile bp = new BattleProfile();
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
        var profile = GetProfile(GetBC().ActiveTurnHero);
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
        ActiveTurnUI(active);
        ActiveHPUI(active);
        ActiveTurnTimer(active);        
    }

    void ActiveTurnUI(bool active)
    {
        mTrunIconRoot.gameObject.SetActive(active);
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