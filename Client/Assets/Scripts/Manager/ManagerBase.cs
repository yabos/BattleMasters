using UnityEngine;
using System.Collections;

public abstract class ManagerBase
{
    #region Propeties

    protected string m_name = string.Empty;
    public string Name
    {
        get { return m_name; }
    }

    // You must allocate value for RootObject in a derived class.
    protected GameObject m_rootObject = null;
    public GameObject RootObject
    {
        get { return m_rootObject; }
    }

    protected void CreateRootObject(Transform transform, string name)
    {
        if (m_rootObject == null)
        {
            m_rootObject = new GameObject(name);
        }

        m_rootObject.transform.SetParent(transform, true);
    }

    protected void DestroyRootObject()
    {
        if (null != m_rootObject)
        {
            GameObjectFactory.Destroy(m_rootObject);
            m_rootObject = null;
        }
    }

    #endregion Properties

    #region IGraphUpdatable
    public abstract void BhvOnEnter();

    public abstract void BhvOnLeave();

    public abstract void BhvFixedUpdate(float dt);

    public abstract void BhvLateFixedUpdate(float dt);

    public abstract void BhvUpdate(float dt);

    public abstract void BhvLateUpdate(float dt);

    //public abstract bool OnMessage(IMessage message);

    #endregion IGraphUpdatable


    #region Abstract Methods

    // called when Global.Awake().
    public abstract void OnAppStart(ManagerSettingBase managerSetting);

    // called when Global.OnDestroy().
    public abstract void OnAppEnd();

    // called when Global.OnApplicationFocus().
    public abstract void OnAppFocus(bool focused);

    // called when Global.OnApplicationPause().
    public abstract void OnAppPause(bool paused);

    #endregion Abstract Methods

    #region virtual Methods

    // called when Global.OnAppPreload().
    public virtual IEnumerator OnAppPreload()
    {
        yield return new WaitForEndOfFrame();
    }


    //called LoadSceneAsync
    public abstract void OnPageEnter(string pageName);
    //called LoadSceneAsync
    public abstract IEnumerator OnPageExit();

    #endregion virtual Methods

    #region Log Methods

    public abstract void Log(string msg);

    public abstract void LogWarning(string msg);

    public abstract void LogError(string msg);

    #endregion Log Methods
}