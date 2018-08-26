using UnityEngine;

public class ManagerSettingBase : MonoBehaviour
{
    #region Propeties

    public bool UseDebugLog = false;

    #endregion Propeties

    #region Log Methods
    public void Log(string msg)
    {
        msg = StringUtil.Format("<color=#ffffffff>{0}</color>", msg);
        if (UseDebugLog)
        {
            Debug.Log(msg);
        }
    }

    public void LogWarning(string msg)
    {
        msg = StringUtil.Format("<color=#ffff00ff>{0}</color>", msg);
        if (UseDebugLog)
        {
            Debug.LogWarning(msg);
        }
    }

    public void LogError(string msg)
    {
        msg = StringUtil.Format("<color=#ff0000ff>{0}</color>", msg);
        if (UseDebugLog)
        {
            Debug.LogError(msg);
        }
    }

    #endregion Log Methods
}