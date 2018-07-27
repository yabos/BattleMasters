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
                goEfc.SetActive(false);
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
        else
        {
            Debug.LogError("Do not Regist Effect : " + name);
        }

        return null;
    }    
}
