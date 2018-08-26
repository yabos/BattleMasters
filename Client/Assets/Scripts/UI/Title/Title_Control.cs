using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Unity.Editor;
using System.Collections;

public class Title_Control : MonoBehaviour
{
    public GameObject mLoginType;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution((Screen.width * 16) / 9, Screen.width, true);

        Global.SoundMgr.PlayBGM(SoundManager.eBGMType.eBGM_Title);

        SetFirebaseDatabase();

        TBManager.Instance.LoadTableAll();

        var loginType = PlayerPrefs.GetString("LoginType");
        if (string.IsNullOrEmpty(loginType))
        {
            mLoginType.SetActive(true);
        }
        else
        {
            mLoginType.SetActive(false);

            var userId = PlayerPrefs.GetString("UserId");
            Global.AuthMgr.SetProvider(loginType);
        }
    }

    public void TestGetData()
    {
        FirebaseDBMamager.Instance.GetUsers("");
    }

    public void TestUpdate()
    {
        FirebaseDBMamager.Instance.OnClickUpdateChildren();
    }

    public void TestLogOut()
    {
        Global.AuthMgr.LogOut();
    }

    void SetFirebaseDatabase()
    {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://flushx-f0024.firebaseio.com/");
        FirebaseApp.DefaultInstance.SetEditorP12FileName("flushx-f0024-424618bd0ee8.p12");
        FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("flushx-f0024@appspot.gserviceaccount.com");
        FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");
    }

    public void OnNextLevel()
    {
        //Global.SceneMgr.Transition(new SceneTransition(typeof(LoginScene).ToString(), "Battle", 0.5f, 0.3f, (code) =>
        //{
        //    Global.SceneMgr.LogWarning(StringUtil.Format("Page Transition -> {0}", "BattleScene"));
        //}));
    }	
}
