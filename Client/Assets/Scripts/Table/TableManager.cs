using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : GlobalManagerBase<ManagerSettingBase>
{

    #region Events
    public override void OnAppStart(ManagerSettingBase managerSetting)
    {
        LoadTableAll();
    }

    public override void OnAppEnd()
    {
    }

    public override void OnAppFocus(bool focused)
    {

    }

    public override void OnAppPause(bool paused)
    {

    }

    public override void OnPageEnter(string pageName)
    {
    }

    public override IEnumerator OnPageExit()
    {
        yield return new WaitForEndOfFrame();
    }

    #endregion Events

    #region IBhvUpdatable

    public override void BhvOnEnter()
    {

    }

    public override void BhvOnLeave()
    {

    }

    public override void BhvFixedUpdate(float dt)
    {

    }

    public override void BhvLateFixedUpdate(float dt)
    {

    }

    public override void BhvUpdate(float dt)
    {
    }

    public override void BhvLateUpdate(float dt)
    {

    }


    //public override bool OnMessage(IMessage message)
    //{
    //    return false;
    //}

    #endregion IBhvUpdatable

    public Dictionary<int, TB_Hero> DicHero = null;
    public Dictionary<eConstType, TB_Const> DicConst = null;

    void LoadHeroTable()
    {
        if (DicHero != null) return;

        DicHero = new Dictionary<int, TB_Hero>();

        List<Dictionary<string, object>> data = CSVReader.Read("Table/Hero");

        for (var i = 0; i < data.Count; i++)
        {
            TB_Hero tbHero = new TB_Hero();

            tbHero.mHeroNo = System.Convert.ToInt32(data[i]["char_ID"]);
            tbHero.mHeroName = System.Convert.ToString(data[i]["char_Name"]);
            tbHero.mElemental = System.Convert.ToString(data[i]["elemental"]);
            tbHero.mHP = System.Convert.ToInt32(data[i]["maxHP"]);
            tbHero.mAtk = System.Convert.ToInt32(data[i]["ATK"]);
            tbHero.mDef = System.Convert.ToInt32(data[i]["DEF"]);
            tbHero.mSpeed = System.Convert.ToInt32(data[i]["speed"]);

            tbHero.mCritical_Rate = System.Convert.ToInt32(data[i]["critical_Rate"]);
            tbHero.mCriticalDamage_Rate = System.Convert.ToInt32(data[i]["criticalDamage_Rate"]);
            tbHero.mGrowUp_TableGroupID = System.Convert.ToInt32(data[i]["growUp_TableGroupID"]);
            tbHero.mPassive_SkillID = System.Convert.ToInt32(data[i]["passive_SkillID"]);
            tbHero.mAtk_SkillID = System.Convert.ToInt32(data[i]["atk_SkillID"]);
            tbHero.mCut_SkillID = System.Convert.ToInt32(data[i]["cut_SkillID"]);
            tbHero.mDod_SkillID = System.Convert.ToInt32(data[i]["dod_SkillID"]);
            tbHero.mActive_SkillID = System.Convert.ToInt32(data[i]["active_SkillID"]);

            tbHero.mResPath = System.Convert.ToString(data[i]["resource_Prefab"]);
            tbHero.mChar_Illust = System.Convert.ToString(data[i]["char_Illust"]);
            tbHero.mChar_Icon = System.Convert.ToString(data[i]["char_Icon"]);
            tbHero.mBaseAtkEfc = System.Convert.ToString(data[i]["BaseAtkEfc"]);
            tbHero.mBaseAtkSound = System.Convert.ToString(data[i]["BaseAtkSound"]);            

            int key = tbHero.mHeroNo;
            if (DicHero.ContainsKey(key))
            {
                Debug.LogError("Already exist key. " + key.ToString());
            }

            DicHero.Add(key, tbHero);
        }
    }

    void LoadContTable()
    {
        if (DicConst != null) return;

        DicConst = new Dictionary<eConstType, TB_Const>();

        List<Dictionary<string, object>> data = CSVReader.Read("Table/Const");

        for (var i = 0; i < data.Count; i++)
        {
            TB_Const tbConst = new TB_Const();

            tbConst.mNameTag = System.Convert.ToString(data[i]["nameTag"]);
            tbConst.mValuel = System.Convert.ToSingle(data[i]["value"]);
            eConstType key = (eConstType)i;
            if (DicConst.ContainsKey(key))
            {
                Debug.LogError("Already exist key. " + key.ToString());
            }

            DicConst.Add(key, tbConst);
        }
    }

    void LoadTableAll()
    {
        LoadHeroTable();
        LoadContTable();
    }

    public int GetHeroNoByName(string name)
    {
        foreach (var elem in DicHero)
        {
            if (elem.Value.mHeroName.Equals(name))
            {
                return elem.Value.mHeroNo;
            }
        }

        return 0;
    }

    public float GetConstValue(eConstType type)
    {
        if (DicConst.ContainsKey(type))
        {
            return DicConst[type].mValuel;
        }

        return 0;
    }
}
