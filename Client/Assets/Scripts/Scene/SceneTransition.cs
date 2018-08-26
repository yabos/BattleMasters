using UnityEngine;
using System;
using System.Collections;

public enum eSceneTransitionErrorCode : int
{
    Success = 0,
    Failure = 1,
}

[System.Serializable]
public class SceneTransition
{
    // type
    public string PageType;

    // data
    public string ResourceName;
    public float FadeInDuration;
    public float FadeOutDuration;
    public Action<eSceneTransitionErrorCode> Completed;

    public SceneTransition(string type) :
        this(type, string.Empty, 0.0f, 0.0f, null)
    {
    }

    public SceneTransition(string type, string resource) :
        this(type, resource, 0.0f, 0.0f, null)
    {
    }

    public SceneTransition(string type, string resource, float fadeInDuration, float fadeOutDuration, Action<eSceneTransitionErrorCode> completed)
    {
        PageType = type;
        ResourceName = resource;
        FadeInDuration = fadeInDuration;
        FadeOutDuration = fadeOutDuration;
        Completed = completed;
    }

    public static SceneTransition CreatePageTransition(string transitionData, Action<eSceneTransitionErrorCode> completed = null)
    {
        if (string.IsNullOrEmpty(transitionData) == true)
        {
            return null;
        }

        string[] eventNotifyDatas = StringUtil.Split(transitionData, "/");
        if (eventNotifyDatas.Length <= 0)
        {
            return null;
        }

        string pageType = string.Empty;
        if (eventNotifyDatas.Length >= 1)
        {
            pageType = eventNotifyDatas[0];
        }

        string resourceName = string.Empty;
        if (eventNotifyDatas.Length >= 2)
        {
            resourceName = eventNotifyDatas[1];
        }

        float fadeInDuration = 0.0f;
        if (eventNotifyDatas.Length >= 3)
        {
            float.TryParse(eventNotifyDatas[2], out fadeInDuration);
        }

        float fadeOutDuration = 0.0f;
        if (eventNotifyDatas.Length >= 3)
        {
            float.TryParse(eventNotifyDatas[3], out fadeOutDuration);
        }

        return new SceneTransition(pageType, resourceName, fadeInDuration, fadeOutDuration, completed);
    }

    public Type[] GetTransitionDataTypes()
    {
        return new Type[]
        {
            ResourceName.GetType(),
            FadeInDuration.GetType(),
            FadeOutDuration.GetType(),
            Completed != null ? Completed.GetType() : typeof(Action<eSceneTransitionErrorCode>)
        };
    }

    public object[] GetTransitionDataArgs()
    {
        return new object[]
        {
            ResourceName,
            FadeInDuration,
            FadeOutDuration,
            Completed
        };
    }
}
