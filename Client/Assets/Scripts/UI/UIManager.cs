using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : GlobalManagerBase<ManagerSettingBase>
{
    private UIRepositories m_widgetRepositories;
    private string m_currentUIName = string.Empty;

    #region Events

    public override void OnAppStart(ManagerSettingBase managerSetting)
    {
        m_name = typeof(UIManager).ToString();

        if (string.IsNullOrEmpty(m_name))
        {
            throw new System.Exception("manager name is empty");
        }

        m_setting = managerSetting as ManagerSettingBase;

        if (null == m_setting)
        {
            throw new System.Exception("manager setting is null");
        }

        CreateRootObject(m_setting.transform, "UIManager");
        m_widgetRepositories = new UIRepositories(this);

        BhvOnEnter();
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
        UnLoad();
        yield return new WaitForEndOfFrame();
    }

    #endregion Events

    #region IBhvUpdatable

    public override void BhvOnEnter()
    {
        m_widgetRepositories.BhvOnEnter();
    }

    public override void BhvOnLeave()
    {
        m_widgetRepositories.BhvOnLeave();
    }

    public override void BhvFixedUpdate(float dt)
    {
        m_widgetRepositories.BhvFixedUpdate(dt);
    }

    public override void BhvLateFixedUpdate(float dt)
    {
        m_widgetRepositories.BhvLateFixedUpdate(dt);
    }

    public override void BhvUpdate(float dt)
    {
        m_widgetRepositories.BhvUpdate(dt);

    }

    public override void BhvLateUpdate(float dt)
    {
        m_widgetRepositories.BhvLateUpdate(dt);
    }


    #endregion IBhvUpdatable


    #region Methods

    public void UnLoad()
    {
        m_currentUIName = string.Empty;
        m_widgetRepositories.FinalizeWidgets(false);
    }

    public void ShowLoadingWidget(float activeTime = 0.0f, string currentPageName = "", string nextPageName = "")
    {
        UILoading widget = m_widgetRepositories.FindWidget("UILoading") as UILoading;
        if (widget == null)
        {
            widget = m_widgetRepositories.CreateWidget<UILoading>("UI/Prefabs/System/UILoading");
        }

        if (widget != null)
        {
            widget.Show(activeTime);
            widget.SetLoadingPanelInfo(currentPageName, nextPageName);
            widget.SetLoadingProgressInfo(0.0f);
        }
    }

    public void SetLoadingPanelInfo(string currentPageName, string nextPageName)
    {
        UILoading widget = m_widgetRepositories.FindWidget("UILoading") as UILoading;
        if (widget != null)
        {
            widget.SetLoadingPanelInfo(currentPageName, nextPageName);
        }
    }

    public void SetLoadingProgressInfo(float progress)
    {
        UILoading widget = m_widgetRepositories.FindWidget("UILoading") as UILoading;
        if (widget != null)
        {
            widget.SetLoadingProgressInfo(progress);
        }
    }

    public void HideLoadingWidget(float deactiveTime = 0.0f)
    {
        UILoading widget = m_widgetRepositories.FindWidget("UILoading") as UILoading;
        if (widget != null)
        {
            widget.Hide(deactiveTime);
        }
    }


    //public void ShowMessageBox(string title, string message, eMessageBoxType messageBoxType,
    //    System.Action<bool> completed = null, float activeTime = 0.0f)
    //{
    //    MessageBoxWidget widget =
    //        m_widgetRepositories.FindWidget("MessageBoxWidget") as MessageBoxWidget;
    //    if (widget == null)
    //    {
    //        widget = m_widgetRepositories.CreateWidget<MessageBoxWidget>("System/MessageBoxWidget");
    //    }

    //    if (widget != null)
    //    {
    //        if (widget.IsActive == true)
    //        {
    //            m_messageBoxQueue.Enqueue(
    //                new MessageBoxDataParam(title, message, messageBoxType, completed, activeTime));
    //        }
    //        else
    //        {
    //            MessageBoxDataParam messageBoxDataParam =
    //                new MessageBoxDataParam(title, message, messageBoxType, completed, activeTime);
    //            widget.Show(activeTime, messageBoxDataParam);
    //        }
    //    }
    //}

    //protected void ClearMessageBoxQueue()
    //{
    //    if (m_messageBoxQueue.Any() == false)
    //    {
    //        return;
    //    }

    //    for (int i = 0; i < m_messageBoxQueue.Count; ++i)
    //    {
    //        MessageBoxDataParam messageBoxDataParam = m_messageBoxQueue.Dequeue();
    //        if (messageBoxDataParam != null)
    //        {
    //            messageBoxDataParam.Dispose();
    //        }
    //    }

    //    m_messageBoxQueue.Clear();
    //}

    //protected void UpdateMessageBoxQueue(float delta)
    //{
    //    if (m_messageBoxQueue.Any() == false)
    //    {
    //        return;
    //    }

    //    MessageBoxWidget widget = m_widgetRepositories.FindWidget("MessageBoxWidget") as MessageBoxWidget;
    //    if (widget != null && widget.IsGameOjectActive != true)
    //    {
    //        MessageBoxDataParam messageBoxDataParam = m_messageBoxQueue.Dequeue();
    //        if (messageBoxDataParam != null)
    //        {
    //            widget.Show(messageBoxDataParam.ActiveTime, messageBoxDataParam);
    //        }
    //        messageBoxDataParam.Dispose();
    //    }
    //}    

    public UIBase ShowWidget(string widgetName, float activeTime = 0.0f, params object[] data)
    {
        UIBase widget = FindWidget(widgetName);

        if (widget == null)
        {
            widget = CreateWidget<UIBase>(widgetName);
        }

        widget.Show(activeTime, data);
        return widget;
    }

    public void Hide(string widgetName, float activeTime = 0.0f)
    {
        UIBase widget = FindWidget(widgetName);

        if (widget != null)
        {
            widget.Hide(activeTime);
        }
    }

    public T CreateWidget<T>(string path, bool dontDestroyOnLoad = false) where T : UIBase
    {
        return m_widgetRepositories.CreateWidget<T>(path, dontDestroyOnLoad);
    }

    public IEnumerator OnCreateWidgetAsync<T>(string path, System.Action<T> action, bool dontDestroyOnLoad = false)
        where T : UIBase
    {
        yield return m_widgetRepositories.OnCreateWidgetAsync<T>(path, action, dontDestroyOnLoad);
    }

    public UIBase FindWidget(string widgetType)
    {
        return m_widgetRepositories.FindWidget(widgetType);
    }

    public void HideAllWidgets(float deactiveTime = 0.0f)
    {
        m_widgetRepositories.HideAllWidgets(deactiveTime);
    }
    
    #endregion Methods
}