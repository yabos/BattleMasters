using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Unity.Editor;
using System.Collections;

public class Title_Control : MonoBehaviour
{
    public GameObject mLoginType;

    private IEnumerator Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.SetResolution((Screen.width * 16) / 9, Screen.width, true);

        SoundManager.Instance.PlayBGM(SoundManager.eBGMType.eBGM_Title);

        SetFirebaseDatabase();

        TBManager.Instance.LoadTableAll();

        yield return new WaitForSeconds(3);

        var loginType = PlayerPrefs.GetString("LoginType");
        if (string.IsNullOrEmpty(loginType))
        {
            mLoginType.SetActive(true);            
        }
        else
        {
            mLoginType.SetActive(false);

            var userId = PlayerPrefs.GetString("UserId");
            Debug.Log(userId);
            FirebaseDBMamager.Instance.GetUsers(userId);
        }
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
        SceneManager.LoadScene("Battle");
    }	
}
