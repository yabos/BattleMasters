using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public static class MethodInfoExtensions
{
    public static bool IsOverride(this MethodInfo m)
    {
        if (m == null) throw new System.ArgumentNullException("IsOverride");
        return (m.GetBaseDefinition().DeclaringType != m.DeclaringType) ? true : false;
    }
}

public class NotificationManager : GlobalManagerBase<ManagerSettingBase>
{
    public class WaitForNotifyInfo : System.IDisposable
    {
        private string m_methodName = string.Empty;

        public string Method
        {
            get
            {
                return m_methodName;
            }
        }

        private eNotifyHandler m_receiverHandlerTypes = eNotifyHandler.Default;

        public eNotifyHandler ReceiverHandlerTypes
        {
            get
            {
                return m_receiverHandlerTypes;
            }
        }

        private object[] m_args = null;

        public object[] Args
        {
            get
            {
                return m_args;
            }
        }

        private float m_duration = 0.0f;
        private float m_currentTime = 0.0f;

        private int m_currentFrameCount = -1;

        public WaitForNotifyInfo(string methodName, eNotifyHandler receiverHandlerTypes, params object[] args) : this(methodName, receiverHandlerTypes, 0.0f, args)
        {
        }

        public WaitForNotifyInfo(string methodName, eNotifyHandler receiverHandlerTypes, float seconds, params object[] args)
        {
            m_methodName = methodName;
            m_receiverHandlerTypes = receiverHandlerTypes;
            m_args = args;
            m_duration = seconds;
            m_currentTime = 0.0f;

            m_currentFrameCount = m_duration == 0.0f ? Time.frameCount : -1;
        }

        public bool CheckWaitStatus(float delta)
        {
            if (m_currentFrameCount != -1)
            {
                return (m_currentFrameCount != Time.frameCount) ? true : false;
            }
            else
            {
                m_currentTime = m_currentTime + delta;
                return (m_currentTime > m_duration) ? true : false;
            }
        }

        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
        }
    }

    protected List<INotifyHandler> m_handlers = new List<INotifyHandler>();

    protected List<WaitForNotifyInfo> m_waitForNotifyInfos = new List<WaitForNotifyInfo>();

    #region Events
    public override void OnAppStart(ManagerSettingBase managerSetting)
    {
        m_name = typeof(NotificationManager).ToString();

        if (string.IsNullOrEmpty(m_name))
        {
            throw new System.Exception("manager name is empty");
        }

        m_setting = managerSetting as ManagerSettingBase;

        if (null == m_setting)
        {
            throw new System.Exception("manager setting is null");
        }

        CreateRootObject(m_setting.transform, "NotificationManager");

        AllDisconnectHandler();
    }

    public override void OnAppEnd()
    {
        AllDisconnectHandler();

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

    #region IGraphUpdatable

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
        UpdateHandler(dt);

        UpdateWaitForNotifys(dt);
    }

    public override void BhvLateUpdate(float dt)
    {

    }

    //public override bool OnMessage(IMessage message)
    //{
    //    return false;
    //}

    #endregion IGraphUpdatable

    #region Methods

#if UNITY_EDITOR
    public List<INotifyHandler> GetEventHandlers()
    {
        return m_handlers;
    }
#endif //UNITY_EDITOR

    public void ConnectHandler(INotifyHandler handler)
    {
        if (handler == null)
        {
            LogWarning("NotificationManager.ConnectHandler( null )");
            return;
        }

        if (m_handlers != null)
        {
            m_handlers.Add(handler);

            m_handlers.Sort((a, b) =>
            {
                return b.GetOrder().CompareTo(a.GetOrder());
            });

            Log(StringUtil.Format("NotificationManager.ConnectHandler({0}) -> Handler Count: {1}", handler.HandlerName, m_handlers.Count.ToString()));
        }

        handler.OnConnectHandler();
    }

    public void DisconnectHandler(INotifyHandler handler)
    {
        if (handler == null)
        {
            LogWarning("NotificationManager.DisconnectHandler( null )");
            return;
        }

        handler.OnDisconnectHandler();

        if (m_handlers != null)
        {
            m_handlers.Remove(handler);

            Log(StringUtil.Format("NotificationManager.DisconnectHandler({0}) -> Handler Count: {1}", handler.HandlerName, m_handlers.Count.ToString()));
        }
    }

    protected void AllDisconnectHandler()
    {
        {
            for (int i = 0; i < m_waitForNotifyInfos.Count; ++i)
            {
                if (m_waitForNotifyInfos[i] == null)
                    continue;

                m_waitForNotifyInfos[i].Dispose();
            }

            m_waitForNotifyInfos.Clear();
        }

        {
            for (int i = 0; i < m_handlers.Count; ++i)
            {
                INotifyHandler handler = m_handlers[i];
                if (handler == null)
                    continue;

                handler.OnDisconnectHandler();
            }

            m_handlers.Clear();
        }
    }

    protected void UpdateHandler(float dleta)
    {
        List<INotifyHandler> eventHanlders = null;
        foreach (INotifyHandler handler in m_handlers)
        {
            if (handler != null && handler.IsActiveAndEnabled() == true)
            {
                continue;
            }

            if (eventHanlders == null)
            {
                eventHanlders = new List<INotifyHandler>();
            }

            eventHanlders.Add(handler);
        }

        if (eventHanlders != null)
        {
            foreach (INotifyHandler handler in eventHanlders)
            {
                DisconnectHandler(handler);
            }
        }
    }

    void UpdateWaitForNotifys(float dleta)
    {
        if (m_waitForNotifyInfos.Any() == false)
        {
            return;
        }

        List<WaitForNotifyInfo> removeWaitForNotifyInfos = new List<WaitForNotifyInfo>();
        for (int i = 0; i < m_waitForNotifyInfos.Count; ++i)
        {
            bool completed = m_waitForNotifyInfos[i].CheckWaitStatus(dleta);
            if (completed == true)
            {
                NotifyToEventHandler(m_waitForNotifyInfos[i].Method, m_waitForNotifyInfos[i].ReceiverHandlerTypes, m_waitForNotifyInfos[i].Args);

                removeWaitForNotifyInfos.Add(m_waitForNotifyInfos[i]);
            }
        }

        for (int i = 0; i < removeWaitForNotifyInfos.Count; ++i)
        {
            removeWaitForNotifyInfos[i].Dispose();

            m_waitForNotifyInfos.Remove(removeWaitForNotifyInfos[i]);
        }
    }

    public void WaitForFrameOfNextNotifyToEventHandler(string methodName, eNotifyHandler notifyHandlerTypes, params object[] args)
    {
        m_waitForNotifyInfos.Add(new WaitForNotifyInfo(methodName, notifyHandlerTypes, 0.0f, args));
    }

    public void WaitForSecondsNotifyToEventHandler(string methodName, eNotifyHandler notifyHandlerTypes, float seconds, params object[] args)
    {
        m_waitForNotifyInfos.Add(new WaitForNotifyInfo(methodName, notifyHandlerTypes, seconds, args));
    }

    public string NotifyToEventHandler(string methodName, eNotifyHandler notifyHandlerTypes, params object[] args)
    {
        string result = string.Empty;
        {
            List<INotifyHandler> eventHanlders = new List<INotifyHandler>();
            foreach (INotifyHandler handler in m_handlers)
            {
                if (handler == null)
                {
                    continue;
                }
                if (handler.IsActiveAndEnabled() == false)
                {
                    continue;
                }

                eNotifyHandler notifyHandlerType = handler.GetHandlerType();
                if ((notifyHandlerType & notifyHandlerTypes) == notifyHandlerType)
                {
                    eventHanlders.Add(handler);
                }
            }

            result = NotifyToHandler<INotifyHandler>(eventHanlders, false, true, methodName, args);
            if (!string.IsNullOrEmpty(result))
            {
                LogError(result);
            }
            else
            {
                Log(StringUtil.Format("NotifyToPluginHandler({0})", methodName));
            }
        }
        return result;
    }

    public string NotifyToEventHandler(string methodName, params object[] args)
    {
        string result = string.Empty;
        {
            List<INotifyHandler> eventHanlders = new List<INotifyHandler>();
            foreach (INotifyHandler handler in m_handlers)
            {
                if (handler == null)
                {
                    continue;
                }
                if (handler.IsActiveAndEnabled() == false)
                {
                    continue;
                }
                eventHanlders.Add(handler);
            }

            result = NotifyToHandler<INotifyHandler>(eventHanlders, false, true, methodName, args);
            if (!string.IsNullOrEmpty(result))
            {
                LogError(result);
            }
            else
            {
                Log(StringUtil.Format("NotifyToPluginHandler({0})", methodName));
            }
        }
        return result;
    }

    protected string NotifyToHandler<T>(List<T> handlers, bool hasOverride, bool hasReturnValue, string methodName, params object[] args)
    {
        if (handlers == null)
        {
            return StringUtil.Format("NotificationManager.NotifyToHandler -> null is handlers. method nmae : {0}", methodName);
        }

        if (handlers.Count == 0)
        {
            return StringUtil.Format("NotificationManager.NotifyToHandler -> zero count handler list. method name : {0}", methodName);
        }

        System.Type[] types = new System.Type[args.Length];
        for (int i = 0; i < types.Length; i++)
        {
            if (args[i] != null)
            {
                types[i] = args[i].GetType();
            }
        }

        string result = string.Empty;
        foreach (T handler in handlers)
        {
            if (null == handler)
            {
                continue;
            }

            MethodInfo method = handler.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                            System.Type.DefaultBinder, types, null);

            bool returnValue = false;
            bool IsNotify = false;
            if (method != null)
            {
                try
                {
                    IsNotify = true;
                    if (hasOverride == true)
                    {
                        IsNotify = method.IsOverride();
                    }

                    if (IsNotify == true)
                    {
                        var returnValueObject = method.Invoke(handler, args);
                        if (hasReturnValue == true && returnValueObject != null)
                        {
                            returnValue = System.Convert.ToBoolean(returnValueObject);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                if (result == string.Empty)
                    result = StringUtil.Format("NotificationManager.NotifyToHandler -> not find method. handler name : {0}, method name : {1}\n", handler.ToString(), methodName);
                else
                    result = StringUtil.Format("{0}NotificationManager.NotifyToHandler -> not find method. handler name : {1}, method name : {2}\n", result, handler.ToString(), methodName);
            }

            if (returnValue == true)
            {
                return result;
            }
        }

        return result;
    }

    #endregion Methods
}
