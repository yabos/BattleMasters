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

public class FirebaseAuthManager : MonoBehaviour
{
    private static FirebaseAuthManager _instance;
    public static FirebaseAuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(FirebaseAuthManager)) as FirebaseAuthManager;
                if (_instance == null)
                {
                    GameObject manaer = new GameObject("FirebaseAuthManager", typeof(FirebaseAuthManager));
                    _instance = manaer.GetComponent<FirebaseAuthManager>();
                }
            }

            return _instance;
        }
    }

    FirebaseAuth[] FirebaseAuth = new FirebaseAuth[]
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
