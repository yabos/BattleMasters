using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EffectType
{    
    Effect_Blade,
    Effect_Blow1,
    Effect_Blow2,
    Effect_Blow3,
    Effect_Blow4,
}

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
    
    string[] effectName =
    {
        "Blade",
        "Blow1",
        "Blow2",
        "Blow3",
        "Blow4",
    };

    private Dictionary<EffectType, GameObject> mDicEffectPool = new Dictionary<EffectType, GameObject>();
    
    public void PreLoadEffect()
    {
        AddEffectPool(EffectType.Effect_Blade);
        AddEffectPool(EffectType.Effect_Blow1);
        AddEffectPool(EffectType.Effect_Blow2);
        AddEffectPool(EffectType.Effect_Blow3);
        AddEffectPool(EffectType.Effect_Blow4);
    }

    void AddEffectPool(EffectType type)
    {
        string path = "Effect/Hero/";
        var resource =Global.ResourceMgr.CreatePrefabResource(path + effectName[(int)type]);
        if (resource != null)
        {
            mDicEffectPool.Add(type, resource.ResourceGameObject);
        }
    }

    public GameObject GetEffect(EffectType type)
    {
        if (mDicEffectPool.ContainsKey(type))
        {
            return mDicEffectPool[type];
        }
        else
        {
            Debug.LogError("Do not Regist Effect : " + name);
        }

        return null;
    }

    public GameObject GetEffect(string type)
    {
        for (int i = 0; i < effectName.Length; ++i)
        {
            if (effectName[i].Equals(type))
            {
                return GetEffect((EffectType)i);
            }
        }

        return null;
    }
}
