using System.Collections.Generic;
using UnityEngine;
using System;

public class UIRepository : IRepository<string, UIBase>, IGraphUpdatable
{
    private Dictionary<string, UIBase> m_widgets = new Dictionary<string, UIBase>(StringComparer.CurrentCultureIgnoreCase);

    public Dictionary<string, UIBase> Widgets
    {
        get { return m_widgets; }
    }

    public void Initialize()
    {
    }

    public void Terminate()
    {
        m_widgets.Clear();
    }

    public bool Get(string widgetType, out UIBase widget)
    {
        return m_widgets.TryGetValue(widgetType, out widget);
    }

    public void Insert(UIBase widget)
    {
        UIBase resultwidget;
        string widgetType = widget.WidgetName;
        if (Get(widgetType, out resultwidget) == false)
        {
            m_widgets.Add(widgetType, widget);
        }
        else
        {
            Debug.LogError("WidgetRepository Insert ID OverLap!!! " + widget.name);
        }
    }

    public bool Remove(UIBase widget)
    {
        string widgetType = widget.UniqueName;

        return Remove(widgetType);
    }

    public bool Remove(string widgetType)
    {
        return m_widgets.Remove(widgetType);
    }

    public bool GetWidgets(ref List<UIBase> lstActors)
    {
        foreach (KeyValuePair<string, UIBase> keyValuePair in m_widgets)
        {
            lstActors.Add(keyValuePair.Value);
        }
        return lstActors.Count > 0;
    }


    public void HideAllWidgets(float deactiveTime = 0.0f)
    {
        foreach (KeyValuePair<string, UIBase> keyValuePair in m_widgets)
        {
            UIBase widgetBase = keyValuePair.Value;


            //if (widgetBase is LoadingWidget)
            //{
            //    continue;
            //}

            //if (widgetBase is MessageBoxWidget)
            //{
            //    continue;
            //}

            if (widgetBase != null && widgetBase.IsActive == true)
            {
                widgetBase.Hide(deactiveTime);
            }
        }

    }

    #region IBhvUpdatable

    public void BhvOnEnter()
    {
    }

    public void BhvOnLeave()
    {
    }

    public void BhvUpdate(float dt)
    {
        using (var itor = m_widgets.GetEnumerator())
        {
            while (itor.MoveNext())
            {
                Debug.Assert(itor.Current.Value != null);
                if (itor.Current.Value.IsActive)
                {
                    itor.Current.Value.BhvUpdate(dt);
                }
            }
        }

    }

    public void BhvLateUpdate(float dt)
    {
        using (var itor = m_widgets.GetEnumerator())
        {
            while (itor.MoveNext())
            {
                Debug.Assert(itor.Current.Value != null);
                if (itor.Current.Value.IsActive)
                {
                    itor.Current.Value.BhvLateUpdate(dt);
                }
            }
        }
    }

    public void BhvFixedUpdate(float dt)
    {
        using (var itor = m_widgets.GetEnumerator())
        {
            while (itor.MoveNext())
            {
                Debug.Assert(itor.Current.Value != null);
                if (itor.Current.Value.IsActive)
                {
                    itor.Current.Value.BhvFixedUpdate(dt);
                }
            }
        }
    }

    public void BhvLateFixedUpdate(float dt)
    {
        using (var itor = m_widgets.GetEnumerator())
        {
            while (itor.MoveNext())
            {
                Debug.Assert(itor.Current.Value != null);
                if (itor.Current.Value.IsActive)
                {
                    itor.Current.Value.BhvLateFixedUpdate(dt);
                }
            }
        }
    }

    #endregion IBhvUpdatable
}