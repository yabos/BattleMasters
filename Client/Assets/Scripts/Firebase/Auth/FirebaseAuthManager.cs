using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public enum EFirebaseProvider
{
    GUEST,
    FACEBOOK,
    TWITTER,
    GOOGLE,
}

public class FirebaseAuthManager : GlobalManagerBase<ManagerSettingBase>
{
    FirebaseAuth_Base[] FirebaseAuth = new FirebaseAuth_Base[]
    {
        new FirebaseAuth_Guest(),
        new FirebaseAuth_Facebook(),
        new FirebaseAuth_Twitter(),
        new FirebaseAuth_Google(),
    };


    /** auth 용 instance */
    public Firebase.Auth.FirebaseAuth auth;
    /** 사용자 */
    public FirebaseUser user;

    EFirebaseProvider currentProvider;

    /** firebase 앱 내에 가입 여부를 체크한다. */
    public bool SingedInFirebase
    {
        get
        {
            return user != auth.CurrentUser && auth.CurrentUser != null;
        }
    }

    #region Events
    public override void OnAppStart(ManagerSettingBase managerSetting)
    {
        
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

    // Use this for initialization
    void Start ()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        for (int i = 0; i < FirebaseAuth.Length; ++i)
        {
            FirebaseAuth[i].InitializeFirebaseAuth(this);
        }

        auth.StateChanged += AuthStateChanged;
    }

    public void SetProvider(string provider)
    {
        
    }

    public void Login(string userId)
    {
        //Firebase.Auth.FirebaseUser user = auth
    }

    /** 상태변화 추적 */
    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            if (!SingedInFirebase && user != null)
            {
                Debug.LogFormat("Signed out {0}", user.UserId);
            }
            user = auth.CurrentUser;
            if (SingedInFirebase)
            {
                PlayerPrefs.SetString("LoginType", user.ProviderId);
                PlayerPrefs.SetString("UserId", user.UserId);
                //FirebaseDBMamager.Instance.InitUserData(user.UserId, "anu");
            }
        }
    }

    public void OnGuestLogin()
    {
        var guest = FirebaseAuth[(int)EFirebaseProvider.GUEST] as FirebaseAuth_Guest;
        if (guest != null)
        {
            guest.GuestLogin();
        }
    }

    public void OnFacebookLogin()
    {
        var facebook = FirebaseAuth[(int)EFirebaseProvider.FACEBOOK] as FirebaseAuth_Facebook;
        if (facebook != null)
        {
            facebook.FacebookLogin();
        }
    }

    public void OnTwitterLogin()
    {
        var twitter = FirebaseAuth[(int)EFirebaseProvider.TWITTER] as FirebaseAuth_Twitter;
        if (twitter != null)
        {
            twitter.TwittertLogin();
        }
    }

    public void OnGoogleLogin()
    {
        var google = FirebaseAuth[(int)EFirebaseProvider.GOOGLE] as FirebaseAuth_Google;
        if (google != null)
        {
            google.GoogleLogin();
        }
    }

    public void LogOut()
    {
        auth.SignOut();
    }
}
