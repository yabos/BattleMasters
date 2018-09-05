using System;


public interface IMessage : INotify, IDisposable
{
    bool IsDeliveryMsg { get; }
    bool IsNetMsg { get; }
}

public abstract class MessageBase : IMessage
{
    public abstract uint MsgCode { get; }

    public virtual bool IsDeliveryMsg
    {
        get { return false; }
    }

    public virtual bool IsNetMsg
    {
        get { return false; }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return string.Format("{0} DeliveryMsg {1} IsNetMsg {2}", this.GetType().Name, IsDeliveryMsg, IsNetMsg);
    }
}

public class SendMessage : MessageBase
{
    private uint m_msgCode;
    public SendMessage(uint code)
    {
        m_msgCode = code;
    }

    public override uint MsgCode
    {
        get { return m_msgCode; }
    }
}