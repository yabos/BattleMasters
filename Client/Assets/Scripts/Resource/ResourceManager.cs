using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eResourceType
{
    Prefab,
    Sound,
    UI,
    Text,
    Max,
}

public class ResourceManager : GlobalManagerBase<ManagerSettingBase>
{
    private Dictionary<int, IResource>[] m_dicResource = new Dictionary<int, IResource>[(int)eResourceType.Max];

    public ResourceManager()
    {
        for (int i = 0; i < (int)eResourceType.Max; i++)
        {
            m_dicResource[i] = new Dictionary<int, IResource>();
        }
    }


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

    //public T Load<T>(string path) where T : Object
    //{
    //    var resourceCache = new Dictionary<string, object>();
    //    if (!resourceCache.ContainsKey(path))
    //        resourceCache[path] = Resources.Load<T>(path);

    //    return (T)resourceCache[path];
    //}

    public PrefabResource CreateUIResource(string path, bool dontDestroyOnLoad)
    {
        IResource res = FindResource(eResourceType.UI, path);
        if (res != null)
        {
            return (PrefabResource)res;
        }

        return (PrefabResource)CreateResource(eResourceType.UI, path);
    }

    public PrefabResource CreatePrefabResource(string path)
    {
        IResource res = FindResource(eResourceType.Prefab, path);
        if (res != null)
        {
            return (PrefabResource)res;
        }

        return (PrefabResource)CreateResource(eResourceType.Prefab, path);
    }

    public PrefabResource CreateTextResource(string path)
    {
        IResource res = FindResource(eResourceType.Text, path);
        if (res != null)
        {
            return (PrefabResource)res;
        }

        return (PrefabResource)CreateResource(eResourceType.Text, path);
    }

    public SoundResource CreateSoundResource(string path)
    {
        IResource res = null;
        res = FindResource(eResourceType.Sound, path);
        if (res != null)
        {
            return (SoundResource)res;
        }

        return (SoundResource)CreateResource(eResourceType.Sound, path);
    }

    private IResource CreateResource(eResourceType eType, string path)
    {
        // 사운드는 넘어오는 name이 실제 파일 이름이 아니라 내부 이름이므로 변경 프로세스가 필요
        //             if (eType == E_ResourceType.Sound)
        //             {
        //                 SoundInfo si;
        //                 if (DataManager.Instance.GetScriptData<SoundData>(E_GameScriptData.Sound).GetSoundInfo(name, out si))
        //                 {
        //                     name = si.Filename;
        //                 }
        //             }

        bool isAssetBundle = false;
        Object objresource = Resources.Load(path);

        if (objresource == null)
        {
            return null;
        }

        return CreateResource(eType, path, path, objresource, isAssetBundle);
    }

    private IResource CreateResource(eResourceType eType, string name, string assetpath, Object objresource, bool isAssetBundle)
    {
        IResource resource = null;
        switch (eType)
        {
            //case E_ResourceType.Actor:
            case eResourceType.UI:
            case eResourceType.Prefab:
            case eResourceType.Text:
                resource = new PrefabResource(objresource, eType, isAssetBundle);
                break;

            case eResourceType.Sound:
                resource = new SoundResource(objresource, isAssetBundle);
                break;

        }
        resource.InitLoad(name, assetpath);

        Dictionary<int, IResource> dicRes = GetDicResource(eType);
        if (dicRes.ContainsKey(resource.GetHashCode()))
        {
            //           LogManager.GetInstance().LogDebug("CreateResource name error" + name);
        }
        else
        {
            dicRes.Add(resource.GetHashCode(), resource);
        }

        return resource;
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

    public IEnumerator CreateResourceAsync(eResourceType resourceType, string path, System.Action<IResource> action)
    {
        if (string.IsNullOrEmpty(path) == true)
        {
            action(null);
            yield break;
        }

        bool isAssetBundle = false;
        UnityEngine.Object resourceData = null;

        ResourceRequest request = Resources.LoadAsync(path);
        yield return request;
        resourceData = request.asset;

        if (resourceData == null)
        {
            LogError("resource is null: " + resourceType.ToString() + ", " + path);
            action(null);
            yield break;
        }

        action(CreateResource(resourceType, path, path, resourceData, isAssetBundle));
    }


    public IEnumerator CreatePrefabResourceAsync(string path, string prefabName, System.Action<PrefabResource> action)
    {
        IResource res = FindResource(eResourceType.Prefab, prefabName);
        if (res != null)
        {
            action(res as PrefabResource);
        }
        else
        {
            yield return CreateResourceAsync(eResourceType.Prefab, path, result =>
            {
                action(result as PrefabResource);
            });
        }
    }


    public IEnumerator CreateUIResourceAsync(string path, bool dontDestroyOnLoad, System.Action<PrefabResource> action)
    {
        IResource res = FindResource(eResourceType.UI, path);
        if (res != null)
        {
            action(res as PrefabResource);
        }
        else
        {
            yield return CreateResourceAsync(eResourceType.UI, path, result =>
            {
                action(result as PrefabResource);
            });
        }
    }

    #endregion Asysc Methods

    public IResource FindResource(string name)
    {
        for (int i = 0; i < (int)eResourceType.Max; i++)
        {
            IResource res = FindResource((eResourceType)i, name);
            if (res != null)
                return res;
        }
        return null;
    }

    public IResource FindResource(eResourceType eType, string name)
    {
        Dictionary<int, IResource> dicresource = GetDicResource(eType);
        int hashcode = name.GetHashCode();
        if (dicresource.ContainsKey(hashcode))
        {
            return dicresource[hashcode];
        }
        return null;
    }

    private Dictionary<int, IResource> GetDicResource(eResourceType eType)
    {
        return m_dicResource[(int)eType];
    }
}
