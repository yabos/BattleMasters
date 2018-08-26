using UnityEngine;

public class SoundResource : IResource
{
    private AudioClip m_audioClip;

    public SoundResource(Object obj, bool isAssetBundle) : base(obj, isAssetBundle) { }

    public override eResourceType Type
    {
        get { return eResourceType.Sound; }
    }

    public AudioClip AudioClip
    {
        get { return m_audioClip; }
    }

    protected override bool InitUpdate()
    {
        GameObject gameObject = ResourceData as GameObject;
        if (gameObject != null)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                m_audioClip = audioSource.clip;
            }
        }
        else
        {
            m_audioClip = (AudioClip)ResourceData;
        }
        return true;
    }

    public override void UnLoad(bool unloadAllLoadedObjects)
    {
        if (AudioClip != null)
        {
            m_audioClip = null;
        }
        //m_assetpath = m_assetpath.Replace(AssetPathSettings.SFXExtension2, "");
        base.UnLoad(unloadAllLoadedObjects);
    }
}