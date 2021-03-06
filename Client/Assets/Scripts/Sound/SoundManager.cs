﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : GlobalManagerBase<ManagerSettingBase>
{
    public enum eBGMType
    {
        eBGM_Title,
        eBGM_Lobby,
        eBGM_Battle,
    }

    string[] m_stOnce = new string[]
    {
        "Blow1",
        "Blow2",
    };

    string[] m_stBGMPath = new string[]
    {
        "Title",
        "Lobby",
        "Battle",
    };

    public AudioClip[] audioSources;
    public Dictionary<string, AudioClip> audioClips;
    static AudioSource musicPlayer;
    Dictionary<string, Audio> aliveSounds;
    AudioListener al;

    public eBGMType m_eCurBGM = eBGMType.eBGM_Title;

    #region Events
    public override void OnAppStart(ManagerSettingBase managerSetting)
    {
        m_name = typeof(SoundManager).ToString();

        if (string.IsNullOrEmpty(m_name))
        {
            throw new System.Exception("manager name is empty");
        }

        m_setting = managerSetting as ManagerSettingBase;

        if (null == m_setting)
        {
            throw new System.Exception("manager setting is null");
        }

        CreateRootObject(m_setting.transform, "SoundManager");

        al = ComponentFactory.AddComponent<AudioListener>(RootObject);
        audioClips = new Dictionary<string, AudioClip>();
        foreach (var name in m_stOnce)
        {
            var clip = Global.ResourceMgr.CreateSoundResource("Sound/SFX/" + name);
            audioClips.Add(name, clip.AudioClip);
        }
        
        musicPlayer = ComponentFactory.AddComponent<AudioSource>(RootObject);
        aliveSounds = new Dictionary<string, Audio>();
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
        if (aliveSounds == null) return;

        if (aliveSounds.Count > 0)
        {
            foreach (Audio a in aliveSounds.Values)
            {
                a.StopSound();
            }
            aliveSounds.Clear();
        }

        if (!al.enabled)
        {
            al.enabled = true;
        }
    }

    public override void BhvLateUpdate(float dt)
    {

    }

    #endregion IBhvUpdatable


    public void PlaySoundOnce(string name)
    {
        //if (!GameSetting.hasSound)
        //{
        //    return;
        //}

        if (!audioClips.ContainsKey(name))
        {
            return;
        }

        var audio = Global.ResourceMgr.CreateSoundResource("Sound/Audio");
        GameObject go = Object.Instantiate(audio.ResourceData) as GameObject;
        //go.transform.parent = this.transform;
        Audio a = go.GetComponent<Audio>();
        a.PlaySoundOnce(audioClips[name]);        
    }

    public void PlayBGM(eBGMType eBGMType)
    {
        //if (!GameSetting.hasMusic)
        //{
        //    return;
        //}

        m_eCurBGM = eBGMType;

        string stName = m_stBGMPath[(int)m_eCurBGM];
        if (musicPlayer.clip == null || musicPlayer.clip.name != stName)
        {
            var clip = Global.ResourceMgr.CreateSoundResource("Sound/BGM/" + stName);
            musicPlayer.clip = clip.AudioClip;
            musicPlayer.Stop();
            musicPlayer.loop = true;
            musicPlayer.Play();
        }
        else
        {
            musicPlayer.loop = true;
            musicPlayer.Play();
        }
    }
    
    public void PlayCurrentBGM()
    {
        string stName = m_stBGMPath[(int)m_eCurBGM];
        if (musicPlayer.clip == null || musicPlayer.clip.name != stName)
        {
            var clip = Global.ResourceMgr.CreateSoundResource("Sound/BGM/" + stName);
            musicPlayer.clip = clip.AudioClip;
            musicPlayer.Stop();
            musicPlayer.loop = true;
            musicPlayer.Play();
        }
    }
}