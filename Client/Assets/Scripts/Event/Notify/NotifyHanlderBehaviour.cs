using UnityEngine;
using System.Collections;

public abstract class NotifyHanlderBehaviour : MonoBehaviour, INotifyHandler
{
    protected bool m_isConnected = false;

    #region Event

    void OnEnable()
    {
        ConnectHandler();
    }

    void OnDisable()
    {
        DisconnectHandler();
    }


    #endregion Event

    #region IEventHandler

    public string HandlerName
    {
        get { return gameObject.name; }
        set { gameObject.name = value; }
    }

    public virtual bool IsConnected
    {
        get { return m_isConnected; }
    }

    public bool IsActiveAndEnabled()
    {
        if (m_isConnected == false)
            return false;

        if (enabled == false)
            return false;

        if (gameObject == null)
            return false;

        if (gameObject.activeSelf == false)
            return false;

        return isActiveAndEnabled;
    }

    public virtual eNotifyHandler GetHandlerType()
    {
        return eNotifyHandler.Default;
    }

    public virtual int GetOrder()
    {
        return (int)GetHandlerType();
    }

    public virtual void OnConnectHandler()
    {
        m_isConnected = true;
    }

    public virtual void OnDisconnectHandler()
    {
        m_isConnected = false;
    }

    public abstract void ConnectHandler();

    public abstract void DisconnectHandler();

    public abstract void OnNotify(INotify message);

    #endregion IEventHandler

}