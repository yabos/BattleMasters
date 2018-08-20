using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;

public class FirebaseAuth_Facebook : FirebaseAuth_Base
{
    public override void InitializeFirebaseAuth(FirebaseAuthManager owner)
    {
        base.InitializeFirebaseAuth(owner);

        // facebook sdk 초기화
        if (!FB.IsInitialized)
        {
            FB.Init(FacebookInitCallBack, OnHideUnity);
        }
    }

    /** Facebook 초기화 콜백 */
    void FacebookInitCallBack()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("Successed to Initalize the Facebook SDK");
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initalize the Facebook SDK");
        }
    }

    /** Facebook 로그인이 활성화되는 경우 호출 */
    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // 게임 일시 중지
            Time.timeScale = 0;
        }
        else
        {
            // 게임 재시작
            Time.timeScale = 1;
        }
    }

    /** 페이스북 로그인 요청(버튼과 연결) */
    public void FacebookLogin()
    {
        var param = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(param, FacebookAuthCallback);        
    }

    /** 페이스북 로그인 결과 콜백 */
    void FacebookAuthCallback(ILoginResult result)
    {
        if (result.Error != null)
        {
            Debug.Log(string.Format("Facebook Auth Error: {0}", result.Error));
            return;
        }
        if (FB.IsLoggedIn)
        {
            var accessToken = AccessToken.CurrentAccessToken;
            Debug.Log(string.Format("Facebook access token: {0}", accessToken.TokenString));

            // 이미 firebase에 account 등록이 되었는지 확인
            if (mOwner.SingedInFirebase)
            {
                LinkFacebookAccount(accessToken);
            }
            else
            {
                // firebase facebook 로그인 연결 호출 부분
                RegisterFacebookAccountToFirebase(accessToken);
            }            
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    /** Facebook access token으로 Firebase 등록 요청 */
    void RegisterFacebookAccountToFirebase(AccessToken accessToken)
    {
        Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);

        mOwner.auth
          .SignInWithCredentialAsync(credential)
          .ContinueWith(task => {
              if (task.IsCanceled)
              {
                  Debug.Log("SignInWithCredentialAsync was canceled.");
                  return;
              }
              if (task.IsFaulted)
              {
                  Debug.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
                  return;
              }

              mOwner.user = task.Result;
              Debug.Log(string.Format("User signed in successfully: {0} ({1})",
              mOwner.user.DisplayName, mOwner.user.UserId));              
          });
    }

    /** Firebase에 등록된 account를 보유했을 때 새로운 인증을 연결한다. */
    void LinkFacebookAccount(AccessToken accessToken)
    {
        Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);

        mOwner.auth.CurrentUser
          .LinkWithCredentialAsync(credential)
          .ContinueWith(task => {
              if (task.IsCanceled)
              {
                  Debug.Log("LinkWithCredentialAsync was canceled.");
                  return;
              }
              if (task.IsFaulted)
              {
                  Debug.Log("LinkWithCredentialAsync encountered an error: " + task.Exception);
                  return;
              }

              mOwner.user = task.Result;
              Debug.Log(string.Format("Credentials successfully linked to Firebase user: {0} ({1})",
              mOwner.user.DisplayName, mOwner.user.UserId));
          });
    }
}
