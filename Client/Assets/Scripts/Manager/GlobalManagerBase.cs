public abstract class GlobalManagerBase<T> : ManagerBase where T : ManagerSettingBase
{
    #region Propeties
    // You must allocate value for Setting in a derived class.
    protected T m_setting = null;
    public T Setting
    {
        get { return m_setting; }
    }
    #endregion Propeties

    #region Log Methods

    public override void Log(string msg)
    {
        if (Setting != null)
        {
            Setting.Log(StringUtil.Format("[{0}] {1}", Name, msg));
        }
    }

    public override void LogWarning(string msg)
    {
        if (Setting != null)
        {
            Setting.LogWarning(StringUtil.Format("[{0}] {1}", Name, msg));
        }
    }

    public override void LogError(string msg)
    {
        if (Setting != null)
        {
            Setting.LogError(StringUtil.Format("[{0}] {1}", Name, msg));
        }
    }

    #endregion Log Methods
}