using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleUI_Control : BaseUI
{
	Transform mBattleLoading = null;
    Transform mHeroHp = null;
    Transform mTrunIconRoot;    

	// Use this for initialization
	void Start ()
    {
        mHeroHp = transform.Find("Anchor/HeroHP");
		mBattleLoading = transform.Find ("Anchor/Loading");
        mTrunIconRoot = transform.Find("Anchor/Turn");
     }
	
	// Update is called once per frame
	void Update ()
    {
	
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
        CreateTurnIcon(GameMain.Instance().BattleControl.ListMyHeroes);
        CreateTurnIcon(GameMain.Instance().BattleControl.ListEnemyHeroes);

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

                goIcon.transform.localPosition = Vector3.zero;
                goIcon.transform.localRotation = Quaternion.identity;
                goIcon.transform.localScale = new Vector3(6, 6, 0);

                var icon = goIcon.GetComponent<TurnIcon>();
                if (icon != null)
                {
                    icon.SetTurnIcon(listHero[i].HeroNo);
                }
            }
        }
    }
}
