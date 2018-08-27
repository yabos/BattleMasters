using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class SceneManager : GlobalManagerBase<SceneManagerSetting>
{
    private List<KeyValuePair<string, SceneBase>> m_pages = new List<KeyValuePair<string, SceneBase>>();

    private SceneBase m_currentScene = null;
    public SceneBase CurrentScene
    {
        get { return m_currentScene; }
    }

    private string m_priviousResourceName = string.Empty;
    private string m_priviousPageTypeName = string.Empty;

    #region Events
    public override void OnAppStart(ManagerSettingBase managerSetting)
    {
        m_name = typeof(SceneManager).ToString();

        if (string.IsNullOrEmpty(m_name))
        {
            throw new System.Exception("manager name is empty");
        }

        m_setting = managerSetting as SceneManagerSetting;

        if (null == m_setting)
        {
            throw new System.Exception("manager setting is null");
        }

        CreateRootObject(m_setting.transform, "SceneManager");

        string pageType = "TitleScene";

        if (string.IsNullOrEmpty(pageType) == false)
        {
            Transition(new SceneTransition(pageType, string.Empty, 0.5f, 0.3f, (code) =>
            {
                LogWarning(StringUtil.Format("Scene Transition -> {0}", pageType));
            }));
        }
    }

    public override void OnAppEnd()
    {
        DestroyRootObject();

        if (m_setting != null)
        {
            GameObjectFactory.DestroyComponent(m_setting);
            m_setting = null;
        }
    }

    public override void OnAppFocus(bool focused)
    {

    }

    public override void OnAppPause(bool paused)
    {

    }

    public override void OnPageEnter(string pageName)
    {
    }

    public override IEnumerator OnPageExit()
    {
        yield return new WaitForEndOfFrame();
    }

    #endregion Events

    #region IBhvUpdatable

    public override void BhvOnEnter()
    {

    }

    public override void BhvOnLeave()
    {

    }

    public override void BhvFixedUpdate(float dt)
    {

    }

    public override void BhvLateFixedUpdate(float dt)
    {

    }

    public override void BhvUpdate(float dt)
    {
    }

    public override void BhvLateUpdate(float dt)
    {

    }


    //public override bool OnMessage(IMessage message)
    //{
    //    return false;
    //}

    #endregion IBhvUpdatable

    #region Methods

#if UNITY_EDITOR
    public List<KeyValuePair<string, SceneBase>> GetPages()
    {
        return m_pages;
    }
#endif //UNITY_EDITOR

    protected void AddPage(SceneBase scene)
    {
        if (scene == null)
        {
            return;
        }

        m_pages.Add(new KeyValuePair<string, SceneBase>(scene.GetType().ToString(), scene));
    }

    public SceneBase FindPage(string key)
    {
        if (m_pages == null || m_pages.Any() == false)
        {
            return null;
        }

        return m_pages.FirstOrDefault(c => (c.Key.IndexOf(key, System.StringComparison.OrdinalIgnoreCase) >= 0)).Value;
    }

    public bool IsInGamePage()
    {
        if (CurrentScene == null)
        {
            return false;
        }

        return CurrentScene.IsInGamePage();
    }

    public void TransitionWithBackPage(float fadeInDuration, float fadeOutDuration, System.Action<eSceneTransitionErrorCode> completed)
    {
        if (string.IsNullOrEmpty(m_priviousPageTypeName))
        {
            return;
        }

        Transition(new SceneTransition(m_priviousPageTypeName, m_priviousResourceName, fadeInDuration, fadeOutDuration, completed));
    }

    public void Transition(SceneTransition data)
    {
        if (data == null)
        {
            // error code
            return;
        }

        MethodInfo method = this.GetType().GetMethod("Transition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, System.Type.DefaultBinder, data.GetTransitionDataTypes(), null);
        if (method != null)
        {
            method = method.MakeGenericMethod(System.Type.GetType(data.PageType));
            method.Invoke(this, data.GetTransitionDataArgs());
        }
    }

    private Coroutine m_transitionCoroutine = null;

    public void Transition<T>(string resourceName, float fadeInDuration, float fadeOutDuration, System.Action<eSceneTransitionErrorCode> completed) where T : SceneBase
    {
        if (m_transitionCoroutine != null)
        {
            return;
        }

        // check resource
        UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name.Equals(resourceName, System.StringComparison.CurrentCultureIgnoreCase) == true)
        {
            resourceName = string.Empty;
        }

        // check page
        if (m_currentScene != null)
        {
            if (m_currentScene.GetType() == typeof(T))
            {
                completed(eSceneTransitionErrorCode.Failure);
                return;
            }
        }

        m_transitionCoroutine = Setting.StartCoroutine(OnTransitionCoroutine<T>(resourceName, fadeInDuration, fadeOutDuration, completed));
    }

    private IEnumerator OnTransitionCoroutine<T>(string resourceName, float fadeInDuration, float fadeOutDuration, System.Action<eSceneTransitionErrorCode> completed) where T : SceneBase
    {
        string currentPageType = m_currentScene ? m_currentScene.GetType().ToString() : string.Empty;
        string nextPageType = typeof(T).ToString();

        Global.UIMgr.ShowLoadingWidget(fadeInDuration, currentPageType, nextPageType);

        yield return new WaitForEndOfFrame();

        if (m_currentScene != null)
        {
            m_priviousPageTypeName = currentPageType;

            m_currentScene.OnFinalize();
            m_currentScene.OnExit();
        }

        yield return new WaitForEndOfFrame();

        float currentProgress = 0.0f;
        const float sceneLoadingProgressRate = 0.4f;

        if (string.IsNullOrEmpty(resourceName) == false)
        {
            UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene != null)
            {
                m_priviousResourceName = activeScene.name;
            }

            yield return Global.ResourceMgr.LoadSceneAsync(resourceName,
                (progress) =>
                {
                    // scene loading..
                    currentProgress = progress * sceneLoadingProgressRate;
                    Global.UIMgr.SetLoadingProgressInfo(currentProgress);

                    LogWarning(StringUtil.Format("OnTransitionCoroutine -> LoadScene {0} %", (int)(currentProgress * 100)));
                },
                () =>
                {
                    // completed scene load
                    currentProgress = sceneLoadingProgressRate;
                    Global.UIMgr.SetLoadingProgressInfo(currentProgress);

                    LogWarning("OnTransitionCoroutine -> Completed LoadScene.");
                });
        }
        else
        {
            currentProgress = sceneLoadingProgressRate;
        }

        m_currentScene = FindPage(typeof(T).ToString());

        if (m_currentScene == null)
        {
            m_currentScene = ComponentFactory.GetChildComponent<T>(RootObject != null ? RootObject : Setting.gameObject, IfNotExist.AddNew);
            AddPage(m_currentScene);
        }

        if (m_currentScene != null)
        {
            yield return m_currentScene.OnEnter(currentProgress);
            m_currentScene.OnInitialize();
        }

        yield return new WaitForEndOfFrame();

        if (completed != null)
        {
            Global.UIMgr.HideLoadingWidget(fadeOutDuration);
            completed(eSceneTransitionErrorCode.Success);
        }

        m_transitionCoroutine = null;
    }

    #endregion Methods
}
