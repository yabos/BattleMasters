using UnityEngine;
using System.Collections;

public abstract class SceneBase : NotifyHanlderBehaviour
{
    public int m_widgetLayer = 0;
    public int WidgetLayer
    {
        get { return m_widgetLayer; }
        set { m_widgetLayer = value; }
    }

    public UnityEngine.Camera UICamera = null;

    protected float m_baseProgress = 0.0f;
    protected float m_currentProgress = 0.0f;
    protected float m_remainProgress = 0.0f;

    #region IEventHandler

    public override eNotifyHandler GetHandlerType()
    {
        return eNotifyHandler.Page;
    }

    public virtual bool IsInGamePage()
    {
        return false;
    }

    public override void OnNotify(INotify notify)
    {

    }

    #endregion IEventHandler


    #region Methods

    public virtual IEnumerator OnEnter(float progress)
    {
        m_widgetLayer = 0;

        m_baseProgress = progress;
        m_currentProgress = m_baseProgress;
        m_remainProgress = 1.0f - m_baseProgress;

        gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();
    }

    public virtual void OnExit()
    {
        this.StopAllCoroutines();

        gameObject.SetActive(false);
    }

    public override void ConnectHandler()
    {
        //Global.NotificationMgr.ConnectHandler(this);
    }

    public override void DisconnectHandler()
    {
        //Global.NotificationMgr.DisconnectHandler(this);
    }

    public abstract void OnInitialize();

    public abstract void OnFinalize();

    public virtual void OnTransitionPage() { }

    public virtual void OnRequestEvent(string netClentTypeName, string requestPackets) { }

    public virtual void OnReceivedEvent(string netClentTypeName, string receivePackets) { }

    public virtual void OnUpdate(float dt)
    {

    }

    public void SetEnterPageProgressInfo(float progress)
    {
        m_currentProgress = m_baseProgress + (progress * m_remainProgress);
        //Global.ResourceMgr.LogWarning(StringUtil.Format("OnTransitionCoroutine -> Enter Page {0} %", (int)(m_currentProgress * 100)));
        Global.UIMgr.SetLoadingProgressInfo(m_currentProgress);
    }

    public void TransitionPage<T>(string pageResourceName, float fadeInDuration, float fadeOutDuration) where T : SceneBase
    {
        string pageName = typeof(T).ToString();
        Global.SceneMgr.Transition(new SceneTransition(pageName, pageResourceName, fadeInDuration, fadeOutDuration, (code) =>
        {
            Global.SceneMgr.LogWarning(StringUtil.Format("Page Transition -> {0}", pageName));
        }));

        OnTransitionPage();
    }

    #endregion Methods
}
