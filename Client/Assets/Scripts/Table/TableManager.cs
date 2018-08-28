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

    public Dictionary<int, TB_Hero> cont_Hero = null;

    void LoadHeroTable()
    {
        if (cont_Hero != null) return;

        cont_Hero = new Dictionary<int, TB_Hero>();

        List<Dictionary<string, object>> data = CSVReader.Read("Table/TB_Hero");

        for (var i = 0; i < data.Count; i++)
        {
            TB_Hero tbHero = new TB_Hero();

            tbHero.mHeroNo = System.Convert.ToInt32(data[i]["HeroNo"]);
            tbHero.mHeroName = System.Convert.ToString(data[i]["Name"]);
            tbHero.mHP = System.Convert.ToInt32(data[i]["HP"]);
            tbHero.mAtk = System.Convert.ToInt32(data[i]["Atk"]);
            tbHero.mSpeed = System.Convert.ToInt32(data[i]["Speed"]);
            tbHero.mBaseAtkEfc = System.Convert.ToString(data[i]["BaseAtkEfc"]);
            tbHero.mBaseAtkSound = System.Convert.ToString(data[i]["BaseAtkSound"]);
            tbHero.mResPath = System.Convert.ToString(data[i]["ResPath"]);

            int key = tbHero.mHeroNo;
            if (cont_Hero.ContainsKey(key))
            {
                Debug.LogError("Already exist key. " + key.ToString());
            }

            cont_Hero.Add(key, tbHero);
        }
    }

    void LoadTableAll()
    {
        LoadHeroTable();
    }

    public int GetHeroNoByName(string name)
    {
        foreach (var elem in cont_Hero)
        {
            if (elem.Value.mHeroName.Equals(name))
            {
                return elem.Value.mHeroNo;
            }
        }

        return 0;
    }
}
