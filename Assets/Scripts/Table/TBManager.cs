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
        cont_Hero = new Dictionary<int, TB_Hero>();

        StringTable st = new StringTable();

        if (false == st.Build("Table/TB_Hero")) { return; }

        int iRowCount = st.row;

        for (int x = 0; x < iRowCount; ++x)
        {
            TB_Hero tbHero = new TB_Hero();

            tbHero.mHeroNo = st.GetValueAsInt(x, "HeroNo");
            tbHero.mHP = st.GetValueAsInt(x, "HP");
            tbHero.mAtk = st.GetValueAsInt(x, "Atk");
            tbHero.mSpeed = st.GetValueAsInt(x, "Speed");            
            //tbHero.mSpeed = (float)iSpeed * 0.001f;
            tbHero.stResPath = st.GetValue(x, "ResPath");

            int key = tbHero.mHeroNo;
            if (cont_Hero.ContainsKey(key))
            {
                Debug.LogError("Already exist key. " + key.ToString() );
            }

            cont_Hero.Add(key, tbHero);
        }
    }

    public void LoadTableAll()
    {
        LoadHeroTable();
    }
}
