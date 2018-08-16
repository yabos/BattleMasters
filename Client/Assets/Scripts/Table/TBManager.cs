using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TBManager : MonoBehaviour
{
    private static TBManager _instance;
    public static TBManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(TBManager)) as TBManager;
                if (_instance == null)
                {
                    GameObject dataManaer = new GameObject("TBManager", typeof(TBManager));
                    _instance = dataManaer.GetComponent<TBManager>();
                }
            }

            return _instance;
        }
    }


    // ------------------------------------//

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

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

    public void LoadTableAll()
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
