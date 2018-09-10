using UnityEngine;

public class PrefabResource : IResource
{
    protected GameObject m_gameObject;
    protected TextAsset m_texObject;

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

    public TextAsset ResourceTextObject
    {
        get { return m_texObject; }
    }

    protected override bool InitUpdate()
    {
        if (ResourceData != null && ResourceData is GameObject)
        {
            m_gameObject = (GameObject)ResourceData;
        }

        if (ResourceData != null && ResourceData is TextAsset)
        {
            m_texObject = ResourceData as TextAsset;
        }

        return true;
    }

    public override void UnLoad(bool unloadAllLoadedObjects)
    {
        if (m_gameObject != null)
        {
            m_gameObject = null;
            TextAsset.Destroy(m_texObject);
            GameObject.Destroy(m_texObject);
        }
    }
}