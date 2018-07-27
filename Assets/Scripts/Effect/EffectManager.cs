using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;
    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(EffectManager)) as EffectManager;
                if (_instance == null)
                {
                    GameObject manaer = new GameObject("EffectManager", typeof(EffectManager));
                    _instance = manaer.GetComponent<EffectManager>();
                }
            }

            return _instance;
        }
    }

    private Dictionary<string, GameObject> mDicEffectPool = new Dictionary<string, GameObject>();
    
    public void PreLoadEffect()
    {
        AddEffectPool("Effect/Hero/", "Blade");
        AddEffectPool("Effect/Hero/", "Blow1");
        AddEffectPool("Effect/Hero/", "Blow2");
        AddEffectPool("Effect/Hero/", "Blow3");
        AddEffectPool("Effect/Hero/", "Blow4");
    }

    void AddEffectPool(string path, string name)
    {
        GameObject efc = VResources.Load<GameObject>(path + name);
        if (efc != null)
        {
            GameObject goEfc = Instantiate(efc);
            if (goEfc != null)
            {
                mDicEffectPool.Add(name, goEfc);
            }
        }
    }

    public GameObject GetEffect(string name)
    {
        if (mDicEffectPool.ContainsKey(name))
        {
            return mDicEffectPool[name];
        }

        return null;
    }

    void SetEfc(string name)
    {
        var goEfc = GetEffect(name);

        //Transform tCen = HeroObj.transform.Find("ef_Center");
        //if(tCen != null )
        //{
        //    Transform tEffect = BattleManager.Instance.transform.Find("Effect");

        //    goEfc.transform.parent = tEffect; 
        //    goEfc.transform.position = tCen.position;
                
        //    ParticleSystem[] pcs = goEfc.GetComponentsInChildren<ParticleSystem>();
        //    if (pcs != null)
        //    {
        //        for (int i = 0; i<pcs.Length; ++i)
        //        {
        //            Renderer render = pcs[i].GetComponent<Renderer>();
        //            if (render != null)
        //            { 
        //                render.sortingOrder = 1000;
        //                render.sortingLayerName = "Hero";
        //            }
        //        }
        //    }
        //}
    }
}
