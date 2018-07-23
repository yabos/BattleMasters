using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMain : MonoBehaviour  
{
    private static GameMain _instance;
    public static GameMain Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameMain)) as GameMain;
                if (_instance == null)
                {
                    GameObject dataManaer = new GameObject("GameMain", typeof(GameMain));
                    _instance = dataManaer.GetComponent<GameMain>();
                }
            }

            return _instance;
        }
    }

    public float mGameSpeed = 1f;

    GameObject mUIRoot = null;

    public GameObject UIRoot
    {
        set { mUIRoot = value; }
        get { return mUIRoot; }
    }

    void Awake()
    {
        Init();
    }

	// Update is called once per frame
	void Update () 
    {
        Time.timeScale = mGameSpeed;
    }

    void Init()
    {
        TBManager.Instance().LoadTableAll();

        ResourcesLoad();

        TitleUILoad();

        // Load save data.   
    }


    void ResourcesLoad()
    {
        // 3d model lad

        // effect load
        EffectManager.Instance().EffectLoad();
    }

    void TitleUILoad()
    {
        UIManager.Instance().LoadUI(UIManager.eUIState.UIState_Title, TitleLoadingDone);
    }

    void TitleLoadingDone()
    {
        GoLobby();
    }

    public void GoLobby()
    {
        UIManager.Instance().LoadUI(UIManager.eUIState.UIState_Lobby, LobbyLoadingDone);
    }

    void LobbyLoadingDone()
    {
        GoBattle();
    }

    public void GoBattle()
    {
        UIManager.Instance().LoadUI(UIManager.eUIState.UIState_Battle, BattleLoadingDone);
    }

    void BattleLoadingDone()
    {
        BattleManager.Instance.InitBattleManager();
    }
}
