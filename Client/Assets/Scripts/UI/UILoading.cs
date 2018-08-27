using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoading : UIBase
{
    UILabel LabelText;

    #region IBhvUpdatable

    public override void BhvOnEnter()
    {
        LabelText = transform.FindChildComponent<UILabel>("UIPanel/LabelText");
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

    public void SetLoadingPanelInfo(string currentPageName, string nextPageName)
    {
        if (string.IsNullOrEmpty(currentPageName))
        {

        }

        if (string.IsNullOrEmpty(nextPageName))
        {

        }

        {

        }
    }

    public void SetLoadingProgressInfo(float progress)
    {
        if (LabelText == null)
        {
            return;

        }

        {
            int percent = (int)(progress * 100.0f);

            if (percent >= 100)
            {
                LabelText.text = "Loading Completed.";
                //m_percentText.text = string.Format("{0}%", percent);
                //m_progressBar.fillAmount = 1;
            }
            else
            {
                //m_percentText.text = string.Format("{0}%", percent);
                //m_progressBar.fillAmount = progress;
            }
        }
    }
}
