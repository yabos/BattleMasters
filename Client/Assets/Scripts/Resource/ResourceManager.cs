using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : GlobalManagerBase<ManagerSettingBase>
{
    static Dictionary<string, Object> resourceCache = new Dictionary<string, Object>();


    #region Events
    public override void OnAppStart(ManagerSettingBase managerSetting)
    {
        // Set backgroundLoadingPriority as High.
        Application.backgroundLoadingPriority = ThreadPriority.High;

        // Set Caching as Decompressed State.
        Caching.compressionEnabled = false;

        // Set Maximum Available Disk Space as "1 GB(1024 MB)".
        Caching.maximumAvailableDiskSpace = 1024 * 1024 * 1024;
    }

    public override void OnAppEnd()
    {
        DestroyRootObject();

        if (m_setting != null)
        {
            GameObjectFactory.DestroyComponent(m_setting);
            m_setting = null;
        }
    }

    public override void OnAppFocus(bool focused)
    {

    }

    public override void OnAppPause(bool paused)
    {

    }

    public override void OnPageEnter(string pageName)
    {
    }

    public override IEnumerator OnPageExit()
    {
        yield return new WaitForEndOfFrame();
    }

    #endregion Events

    #region IBhvUpdatable

    public override void BhvOnEnter()
    {

    }

    public override void BhvOnLeave()
    {

    }

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


    //public override bool OnMessage(IMessage message)
    //{
    //    return false;
    //}

    #endregion IBhvUpdatable

    public T Load<T>(string path) where T : Object
    {
        if (!resourceCache.ContainsKey(path))
            resourceCache[path] = Resources.Load<T>(path);

        return (T)resourceCache[path];
    }
    

    #region Asysc Methods

    public IEnumerator LoadSceneAsync(string sceneName, System.Action<float> OnLoadingProgressAction, System.Action OnSceneLoadingCompleteAction)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        if (asyncOperation == null)
        {
            throw new System.Exception(StringUtil.Format("Failed to LoadSceneAsync({0}.unity, asyncOperation == null )", sceneName));
        }

        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.isDone == false)
        {
            if (asyncOperation.progress < 0.9f)
            {
                if (OnLoadingProgressAction != null)
                {
                    OnLoadingProgressAction(asyncOperation.progress);
                }
            }
            else if (false == asyncOperation.allowSceneActivation)
            {
                asyncOperation.allowSceneActivation = true;
            }
            else
            {
                yield return asyncOperation;
            }
        }

        yield return new WaitForEndOfFrame();

        if (OnLoadingProgressAction != null)
        {
            OnLoadingProgressAction(1.0f);
        }

        if (null != OnSceneLoadingCompleteAction)
        {
            OnSceneLoadingCompleteAction();
        }

        yield return true;
    }


    //public IEnumerator CreateResourceAsync(eResourceType resourceType, string resourceName, ePath pathType, System.Action<IResource> action)
    //{
    //    if (string.IsNullOrEmpty(resourceName) == true)
    //    {
    //        action(null);
    //        yield break;
    //    }

    //    bool isAssetBundle = false;
    //    string resourcePath = StringUtil.Format("{0}/{1}", m_pathManager.GetPath(pathType), resourceName);
    //    UnityEngine.Object resourceData = null;

    //    ResourceRequest request = Resources.LoadAsync(resourcePath);
    //    yield return request;
    //    resourceData = request.asset;

    //    if (resourceData == null)
    //    {
    //        LogError("resource is null: " + resourceType.ToString() + ", " + pathType.ToString() + ", " + resourceName);
    //        action(null);
    //        yield break;
    //    }

    //    action(CreateResource(resourceType, resourceName, resourcePath, resourceData, isAssetBundle));
    //}


    //public IEnumerator CreatePrefabResourceAsync(ePath path, string prefabName, System.Action<PrefabResource> action)
    //{
    //    IResource res = FindResource(eResourceType.Prefab, prefabName);
    //    if (res != null)
    //    {
    //        action(res as PrefabResource);
    //    }
    //    else
    //    {
    //        yield return CreateResourceAsync(eResourceType.Prefab, prefabName, path, result =>
    //        {
    //            action(result as PrefabResource);
    //        });
    //    }
    //}


    //public IEnumerator CreateUIResourceAsync(string prefabName, ePath pathType, bool dontDestroyOnLoad, System.Action<PrefabResource> action)
    //{
    //    IResource res = FindResource(eResourceType.UI, prefabName);
    //    if (res != null)
    //    {
    //        action(res as PrefabResource);
    //    }
    //    else
    //    {
    //        yield return CreateResourceAsync(eResourceType.UI, prefabName, pathType, result =>
    //        {
    //            action(result as PrefabResource);
    //        });
    //    }
    //}

    #endregion Asysc Methods
}
