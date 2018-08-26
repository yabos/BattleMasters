using UnityEngine;

public class PrefabResource : IResource
{
    protected GameObject m_gameObject;
    private eResourceType m_resourceType;

    public PrefabResource(Object obj, eResourceType resourceType, bool isAssetBundle)
        : base(obj, isAssetBundle)
    {
        m_resourceType = resourceType;
    }

    public override eResourceType Type
    {
        get { return m_resourceType; }
    }

    public GameObject ResourceGameObject
    {
        get { return m_gameObject; }
    }

    protected override bool InitUpdate()
    {
        if (ResourceData != null && ResourceData is GameObject)
        {
            m_gameObject = (GameObject)ResourceData;
        }
        return true;
    }

    public override void UnLoad(bool unloadAllLoadedObjects)
    {
        if (m_gameObject != null)
        {
            m_gameObject = null;
            //TextAsset.Destroy(m_texObject);
            //GameObject.Destroy(m_texObject);
        }
    }
}