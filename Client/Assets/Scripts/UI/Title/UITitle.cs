using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITitle : UIBase
{
    #region IBhvUpdatable

    public override void BhvOnEnter()
    {
        Global.SoundMgr.PlayBGM(SoundManager.eBGMType.eBGM_Title);
    }

    public override void BhvOnLeave() { }

    public override void BhvFixedUpdate(float dt)
    {
    }

    public override void BhvLateFixedUpdate(float dt)
    {
    }

    public override void BhvUpdate(float dt)
    {
    }

    public override void BhvLateUpdate(float dt)
    {
    }

    #endregion // "IBhvUpdatable"

    protected override void ShowWidget(params object[] data) { }
    protected override void HideWidget() { }

    public override void OnNotify(INotify message)
    {

    }

    public void OnNextLevel()
    {
        Global.SceneMgr.Transition<LobbyScene>("LobbyScene", 0.5f, 0.3f, (code) =>
        {
            Global.SceneMgr.LogWarning(StringUtil.Format("Scene Transition -> {0}", "LobbyScene"));
        });
    }
}
