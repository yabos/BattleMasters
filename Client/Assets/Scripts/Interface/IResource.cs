using UnityEngine;


public abstract class IResource
{
    protected string m_name;
    protected string m_path;

    protected string m_hashname;
    protected int m_hashcode;

    protected readonly Object m_resourceData;
    protected readonly bool m_isAssetBundle = false;

    public abstract eResourceType Type { get; }

    public Object ResourceData
    {
        get { return m_resourceData; }
    }

    //public abstract GameObject UnityGameObject { get; }

    public string Name
    {
        get { return m_name; }
    }

    public void Release()
    {
        UnLoad(false);
        //m_resourceData = null;
        //m_isAssetBundle = false;
        //Resources.UnloadUnusedAssets();
    }

    protected abstract bool InitUpdate();


    public IResource(Object obj, bool isAssetBundle)
    {
        m_resourceData = obj;
        m_isAssetBundle = isAssetBundle;
    }

    public bool InitLoad(string name, string path)
    {
        m_name = name;
        m_path = path;

        m_hashcode = m_name.GetHashCode();

        return InitUpdate();
    }

    public override string ToString()
    {
        return m_name;
    }

    public override int GetHashCode()
    {
        return m_hashcode;
    }


    public virtual void UnLoad(bool unloadAllLoadedObjects)
    {

    }
}