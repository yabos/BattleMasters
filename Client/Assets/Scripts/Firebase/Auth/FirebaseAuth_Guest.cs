using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuth_Guest : FirebaseAuth
{
    public override void InitializeFirebaseAuth()
    {
        base.InitializeFirebaseAuth();

        auth.StateChanged += AuthStateChanged;
    }

    /** 익명 로그인 요청 */
    public void GuestLogin()
    {
        auth
          .SignInAnonymouslyAsync()
          .ContinueWith(task => {
              if (task.IsCanceled)
              {
                  Debug.LogError("SignInAnonymouslyAsync was canceled.");
                  return;
              }
              if (task.IsFaulted)
              {
                  Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                  return;
              }

              user = task.Result;
              Debug.Log(string.Format("User signed in successfully: {0} ({1})",
              user.DisplayName, user.UserId));
              PlayerPrefs.SetString("LoginType", "GUEST");
              PlayerPrefs.SetString("UserId", user.UserId);
              FirebaseDBMamager.Instance.InitUserData(user.UserId, "anu");
          });
    }


    /** 상태변화 추적 */
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
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
                Debug.Log(string.Format("Signed in {0}", user.UserId));
                Debug.Log(string.Format("Signed in {0} _ {1}", user.DisplayName, user.Email));
            }
        }
    }
}
