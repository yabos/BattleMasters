public enum eNotifyHandler : int
{
    Default = 0x00000001,
    Page = 0x00000002,
    Widget = 0x00000004,
    Node = 0x00000008,

    // util
    Util_GameFlow = Page | Widget,
    Util_All = Default | Util_GameFlow,
}

public interface INotifyHandler
{
    string HandlerName { get; }

    bool IsConnected { get; }

    eNotifyHandler GetHandlerType();

    int GetOrder();

    bool IsActiveAndEnabled();

    void OnConnectHandler();

    void OnDisconnectHandler();

    void OnNotify(INotify notify);
}