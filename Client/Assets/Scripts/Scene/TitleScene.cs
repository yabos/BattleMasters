using System.Collections;
using UnityEngine;

public class TitleScene : SceneBase
{
    protected bool IsPageLogin = false;

    public override IEnumerator OnEnter(float progress)
    {
        yield return base.OnEnter(progress);

        yield return Global.UIMgr.OnCreateWidgetAsync<TitleUI>(ResourcePath.TitleUI, widget =>
        {
            if (widget != null)
            {
                widget.Show();
                SetEnterPageProgressInfo(0.5f);
            }
        });

        yield return new WaitForSeconds(1.0f);
    }

    public override void OnExit()
    {
        base.OnExit();

        //Global.WidgetMgr.HideAllWidgets(0.3f);
    }

    public override void OnInitialize()
    {
#if USE_SDKPLUGIN

#if UNITY_EDITOR
        AssetBundleDownloadWidget widget = Global.UIMgr.CreateWidget<AssetBundleDownloadWidget>("Popup/AssetBundleDownloadWidget");
        if (widget != null)
        {
            widget.ShowWidget(0.5f);
        }
#else // UNITY_EDITOR
        IsPageLogin = false;
        if (Global.PluginMgr.IsNCSDKAgreement == true)
        {
            AssetBundleDownloadWidget widget = Global.UIMgr.CreateWidget<AssetBundleDownloadWidget>("Popup/AssetBundleDownloadWidget");
            if (widget != null)
            {
                widget.ShowWidget(0.5f);
            }
        }
        else
        {
            Global.PluginMgr.SendNCSDKShowAgreement(1, null);
        }
#endif // UNITY_EDITOR

#endif //USE_SDKPLUGIN
    }

    public override void OnFinalize()
    {
    }

    public override void OnRequestEvent(string netClentTypeName, string requestPackets)
    {
        //Global.WidgetMgr.ShowLoadingWidget(0.3f);
    }

    public override void OnReceivedEvent(string netClentTypeName, string receivePackets)
    {
        //Global.WidgetMgr.HideLoadingWidget(0.1f);
    }

    //private void ShowMessageBoxWithPluginNotifyInfo(string message, eMessageBoxType boxType = eMessageBoxType.OK, System.Action<bool> completed = null)
    //{
    //    string title = StringUtil.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(Color.blue), "System Info Message");
    //    Global.WidgetMgr.ShowMessageBox(title, message, boxType, completed);
    //}

    public override void OnNotify(INotify notify)
    {
        base.OnNotify(notify);
    }
}