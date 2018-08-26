using System.Collections;
using UnityEngine;

public abstract class BaseUI : NotifyHanlderBehaviour
{
    public bool IsPopupType = false;

    public string UniqueName { get; private set; }
    public string WidgetName { get; private set; }

    protected bool m_isActive = false;

    protected UIPanel uIPanel;

    public bool IsActive
    {
        get { return m_isActive; }
    }

    public bool IsGameOjectActive
    {
        get { return gameObject.activeSelf; }
    }


    //ui getcomponent 하는 곳을 enter 종료할때는 leave로 사용하는 것이 좋을듯한데..
    void Awake()
    {
        string typeName = this.GetType().ToString();
        string[] split = typeName.Split('.');
        if (split.Length > 0)
        {
            WidgetName = split[split.Length - 1];
        }

        BhvOnEnter();
    }

    void OnDestroy()
    {
        BhvOnLeave();
    }

    #region IBhvUpdatable

    public abstract void BhvOnEnter();
    public abstract void BhvOnLeave();

    public virtual void BhvFixedUpdate(float dt)
    {
    }

    public virtual void BhvLateFixedUpdate(float dt)
    {
    }

    public virtual void BhvUpdate(float dt)
    {
    }

    public virtual void BhvLateUpdate(float dt)
    {
    }

    #endregion // "IBhvUpdatable"

    #region IEventHandler

    public override eNotifyHandler GetHandlerType()
    {
        return eNotifyHandler.Widget;
    }

    public override void ConnectHandler()
    {
        //Global.NotificationMgr.ConnectHandler(this);
    }

    public override void DisconnectHandler()
    {
        //Global.NotificationMgr.DisconnectHandler(this);
    }

    //public override void OnConnectHandler()
    //{
    //    base.OnConnectHandler();
    //}

    //public override void OnDisconnectHandler()
    //{
    //    base.OnDisconnectHandler();
    //}


    #endregion IEventHandler


    public void InitializeWidget(string name)
    {
        UniqueName = name;

        uIPanel = ComponentFactory.GetComponent<UIPanel>(gameObject);
    }

    public virtual void FinalizeWidget()
    {
        gameObject.SetActive(false);
    }

    private Coroutine FadeCoroutine = null;

    protected abstract void ShowWidget(params object [] data);
    protected abstract void HideWidget();

    public void Show(float activeTime = 0.0f, params object [] data)
    {
        if (FadeCoroutine != null)
        {
            StopCoroutine(FadeCoroutine);
            FadeCoroutine = null;
        }

        m_isActive = true;      

        gameObject.SetActive(IsActive);
        ShowWidget(data);

        if (activeTime != 0.0f)
        {
            if (uIPanel != null)
            {
                uIPanel.alpha = 0.0f;
            }

            if (IsGameOjectActive == true)
            {
                FadeCoroutine = StartCoroutine(CanvasFadeCoroutine(activeTime, false, () =>
                {
                    if (uIPanel != null)
                    {
                        uIPanel.alpha = 1.0f;
                    }
                    FadeCoroutine = null;
                }));
            }
            else
            {
                if (uIPanel != null)
                {
                    uIPanel.alpha = 1.0f;
                }
                FadeCoroutine = null;
            }
        }
        else
        {
            if (uIPanel != null)
            {
                uIPanel.alpha = 1.0f;
            }
        }
    }

    public void Hide(float deactiveTime = 0.0f)
    {
        if (FadeCoroutine != null)
        {
            StopCoroutine(FadeCoroutine);
            FadeCoroutine = null;
        }

        m_isActive = false;
       
        if (deactiveTime != 0.0f)
        {
            if (uIPanel != null)
            {
                uIPanel.alpha = 1.0f;
            }

            if (IsGameOjectActive == true)
            {
                FadeCoroutine = StartCoroutine(CanvasFadeCoroutine(deactiveTime, true, () =>
                {
                    gameObject.SetActive(IsActive);
                    FadeCoroutine = null;
                }));
            }
            else
            {
                if (uIPanel != null)
                {
                    uIPanel.alpha = 0.0f;
                }

                FadeCoroutine = null;
            }
        }
        else
        {
            if (uIPanel != null)
            {
                uIPanel.alpha = 0.0f;
            }

            gameObject.SetActive(IsActive);
        }
        HideWidget();
    }

    private IEnumerator CanvasFadeCoroutine(float duration, bool inverse, System.Action completed)
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t + Time.deltaTime / duration);

            if (uIPanel != null)
            {
                uIPanel.alpha = (inverse == true) ? 1.0f - t : t;
            }
        }

        if (completed != null)
        {
            completed();
        }
    }
}