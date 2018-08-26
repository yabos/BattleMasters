using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Global : SingletonMonoBehaviour<Global>
{
    protected bool m_isInitialized = false;

    private bool m_hasSetOriginalScreenResolution = false;

    private const int m_highMaxResolutionWidth = 1280;
    private int m_originalScreenWidth = 1280;
    private int m_originalScreenHeight = 720;

    private bool UseDebugLog = true;

    public static GameObject GameObject
    {
        get
        {
            if (m_instance != null)
            {
                return m_instance.gameObject;
            }
            else
            {
                return null;
            }
        }
    }

    private SceneManager m_sceneManager = null;
    public static SceneManager SceneMgr
    {
        get
        {
            return m_instance.m_sceneManager;
        }
    }

    private FirebaseAuthManager m_AuthManager = null;
    public static FirebaseAuthManager AuthMgr
    {
        get
        {
            return m_instance.m_AuthManager;
        }
    }

    private ResourceManager m_resourceManager = null;
    public static ResourceManager ResourceMgr
    {
        get
        {
            return m_instance.m_resourceManager;
        }
    }

    private SoundManager m_soundManager = null;
    public static SoundManager SoundMgr
    {
        get
        {
            return m_instance.m_soundManager;
        }
    }

    void Awake()
    {
        Log("Awake()");

        if (null != m_instance)
        {
            FinalizeManager();
            //GameObjectFactory.Destroy(gameObject);
            return;
        }
        else
        {
            m_instance = this;
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Application.targetFrameRate = 30;

        if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        SetResoultion();

        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this);
        }

        InitializeManager();
    }

    void Start()
    {
        Log("Start()");

        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                //if (m_managers[i].Value is WidgetManager) continue;

                m_managers[i].Value.BhvOnEnter();
            }
        }
    }

    void OnApplicationQuit()
    {
        Log("OnApplicationQuit()");

#if UNITY_EDITOR
        QualitySettings.shadowProjection = ShadowProjection.CloseFit;
#endif // UNITY_EDITOR
    }

    void OnApplicationFocus(bool focusStatus)
    {
        if (!m_isInitialized)
        {
            return;
        }

        Log(StringUtil.Format("OnApplicationFocus({0})", focusStatus));

        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                m_managers[i].Value.OnAppFocus(focusStatus);
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!m_isInitialized)
        {
            return;
        }

        Log(StringUtil.Format("OnApplicationPause({0})", pauseStatus));

        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                m_managers[i].Value.OnAppPause(pauseStatus);
            }
        }
    }

    void Update()
    {
        if (!m_isInitialized)
        {
            return;
        }

        float delta = Time.deltaTime;
        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                m_managers[i].Value.BhvUpdate(delta);
            }
        }
    }

    void LateUpdate()
    {
        if (!m_isInitialized)
        {
            return;
        }
        float delta = Time.deltaTime;

        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                m_managers[i].Value.BhvLateUpdate(delta);
            }
        }
    }

    void FixedUpdate()
    {
        if (!m_isInitialized)
        {
            return;
        }

        float delta = Time.fixedDeltaTime;


        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                m_managers[i].Value.BhvFixedUpdate(delta);
            }
        }

        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                m_managers[i].Value.BhvLateFixedUpdate(delta);
            }
        }
    }

    private void OnEnable()
    {
        Log("OnEnable()");

#if UNITY_EDITOR
        if (gameObject.name.Equals("EditorGlobal"))
        {
            Log("OnEnable() = -> m_instance = this");
            m_instance = this;
        }
#endif // UNITY_EDITOR
    }

    private void OnDisable()
    {
        Log("OnDisable()");

#if UNITY_EDITOR
        if (gameObject.name.Equals("EditorGlobal") && m_instance == this)
        {
            Log("OnDisable() = -> m_instance = null");
            m_instance = null;
        }
#endif // UNITY_EDITOR
    }

    void OnDestroy()
    {
        Log("OnDestroy()");

        FinalizeManager();
    }

    #region Methods

    public IEnumerator OnAppPreload(System.Action completed)
    {
        for (int i = 0; i < m_managers.Count; ++i)
        {
            if (m_managers[i].Value != null)
            {
                yield return m_managers[i].Value.OnAppPreload();
            }
        }

        yield return new WaitForEndOfFrame();

        if (completed != null)
        {
            completed();
        }
    }

    public void AppPreload(System.Action completed)
    {
        StartCoroutine(OnAppPreload(completed));
    }   

    protected List<KeyValuePair<string, ManagerBase>> m_managers = new List<KeyValuePair<string, ManagerBase>>();

    protected bool CreateManager<T1, T2>(ref T1 manager) where T1 : ManagerBase, new() where T2 : ManagerSettingBase
    {
        if (manager != null)
        {
            // error message
            return false;
        }

        T2 setting = ComponentFactory.GetChildComponent<T2>(GameObject, IfNotExist.ReturnNull);
        if (setting == null)
        {
            setting = ComponentFactory.GetComponent<T2>(GameObject, IfNotExist.AddNew);
            if (Application.isPlaying)
            {
                LogWarning(StringUtil.Format("{0} component is automatically created", typeof(T2).ToString()));
            }
        }

        manager = new T1();
        manager.OnAppStart(setting);
        return (manager != null) ? true : false;
    }

    protected void DestroyManager<T>(ref T manager) where T : ManagerBase
    {
        if (manager == null)
        {
            // error message
            return;
        }

        manager.OnAppEnd();
        RemoveManager(manager);
        manager = null;
    }

    protected void AddManager(ManagerBase manager)
    {
        KeyValuePair<string, ManagerBase> keyValuePair = m_managers.FirstOrDefault(c => (c.Key.IndexOf(manager.Name, System.StringComparison.OrdinalIgnoreCase) >= 0));
        if (keyValuePair.Value != null)
        {
            return;
        }

        m_managers.Add(new KeyValuePair<string, ManagerBase>(manager.Name, manager));
    }

    protected void RemoveManager(ManagerBase manager)
    {
        RemoveManager(manager.Name);
    }

    protected void RemoveManager(string name)
    {
        KeyValuePair<string, ManagerBase> keyValuePair = m_managers.FirstOrDefault(c => (c.Key.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0));
        if (keyValuePair.Value == null)
        {
            return;
        }

        m_managers.Remove(keyValuePair);
    }

#if UNITY_EDITOR

    public void InitializeForEditor()
    {
        Log("InitializeForEditor()");

        m_instance = this;

        InitializeManager();
    }

    public void FinalizeForEditor()
    {
        Log("FinalizeForEditor()");

        FinalizeManager();

        GameObjectFactory.Destroy(gameObject);
        m_instance = null;
    }

#endif // UNITY_EDITOR

    void InitializeManager()
    {
        Log("InitializeManager()");

        if (m_managers != null)
        {
            m_managers.Clear();
        }

        if (CreateManager<SceneManager, SceneManagerSetting > (ref m_sceneManager))
        {
            AddManager(m_sceneManager);
        }

        if (CreateManager<FirebaseAuthManager, ManagerSettingBase>(ref m_AuthManager))
        {
            AddManager(m_AuthManager);
        }

        if (CreateManager<ResourceManager, ManagerSettingBase>(ref m_resourceManager))
        {
            AddManager(m_resourceManager);
        }

        if (CreateManager<SoundManager, ManagerSettingBase>(ref m_soundManager))
        {
            AddManager(m_soundManager);
        }

        //DataManager.Instance.LoadData();

        m_isInitialized = true;
    }

    void FinalizeManager()
    {
        Log("FinalizeManager()");

        for (int i = m_managers.Count - 1; i >= 0; --i)
        {
            if (m_managers[i].Value != null)
            {
                m_managers[i].Value.BhvOnLeave();
            }
        }


        DestroyManager<SceneManager>(ref m_sceneManager);
        DestroyManager<FirebaseAuthManager>(ref m_AuthManager);
        DestroyManager<ResourceManager>(ref m_resourceManager);

        m_isInitialized = false;
    }

    void SetResoultion()
    {
        if (!m_hasSetOriginalScreenResolution)
        {
            m_hasSetOriginalScreenResolution = true;
            m_originalScreenWidth = Screen.width;
            m_originalScreenHeight = Screen.height;
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int maxResolutionWidth = m_highMaxResolutionWidth;
            int modwidth = m_originalScreenWidth;
            int modheight = m_originalScreenHeight;
            if (m_originalScreenWidth > maxResolutionWidth)
            {
                modwidth = maxResolutionWidth;
                modheight = (int)(m_originalScreenHeight * ((float)modwidth / (float)m_originalScreenWidth));
            }
            else
            {
                modwidth = m_originalScreenWidth;
                modheight = m_originalScreenHeight;
            }

            Screen.SetResolution(modwidth, modheight, Screen.fullScreen);
        }

        Log(StringUtil.Format("SetResoultion({0}, {1}, {2})", Screen.width, Screen.height, Screen.fullScreen));
    }

    #endregion // Methods

    #region Log Methods
    public void Log(string msg)
    {
        msg = StringUtil.Format("<color=#ffffffff>[Global] {0}</color>", msg);
        if (UseDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public void LogWarning(string msg)
    {
        msg = StringUtil.Format("<color=#ffff00ff>[Global] {0}</color>", msg);
        if (UseDebugLog)
        {
            Debug.LogWarning(msg);
        }
    }

    public void LogError(string msg)
    {
        msg = StringUtil.Format("<color=#ff0000ff>[Global] {0}</color>", msg);
        if (UseDebugLog)
        {
            Debug.LogError(msg);
        }
    }

    #endregion //Log Methods


}

