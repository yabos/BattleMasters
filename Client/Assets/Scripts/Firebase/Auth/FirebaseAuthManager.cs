using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    public enum EFirebaseAuthType
    {
        GUEST,
        FACEBOOK,
        TWITTER,
        GOOGLE,
    }

    FirebaseAuth[] FirebaseAuth = new FirebaseAuth[]
    {
        new FirebaseAuth_Guest(),
        new FirebaseAuth_Facebook(),
        new FirebaseAuth_Twitter(),
        new FirebaseAuth_Google(),
    };

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < FirebaseAuth.Length; ++i)
        {
            FirebaseAuth[i].InitializeFirebaseAuth();
        }
    }

    public void OnGuestLogin()
    {
        var guest = FirebaseAuth[(int)EFirebaseAuthType.GUEST] as FirebaseAuth_Guest;
        if (guest != null)
        {
            guest.GuestLogin();
        }
    }

    public void OnFacebookLogin()
    {
        var facebook = FirebaseAuth[(int)EFirebaseAuthType.FACEBOOK] as FirebaseAuth_Facebook;
        if (facebook != null)
        {
            facebook.FacebookLogin();
        }
    }

    public void OnTwitterLogin()
    {
        var twitter = FirebaseAuth[(int)EFirebaseAuthType.TWITTER] as FirebaseAuth_Twitter;
        if (twitter != null)
        {
            twitter.TwittertLogin();
        }
    }

    public void OnGoogleLogin()
    {
        var google = FirebaseAuth[(int)EFirebaseAuthType.GOOGLE] as FirebaseAuth_Google;
        if (google != null)
        {
            google.GoogleLogin();
        }
    }
}
