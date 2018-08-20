using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuth_Guest : FirebaseAuth
{
    public override void InitializeFirebaseAuth(FirebaseAuthManager owner)
    {
        base.InitializeFirebaseAuth(owner);
    }

    /** 익명 로그인 요청 */
    public void GuestLogin()
    {
        mOwner.auth
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

              mOwner.user = task.Result;
              Debug.Log(string.Format("User signed in successfully: {0} ({1})",
              mOwner.user.DisplayName, mOwner.user.UserId));              
          });
    }
}
