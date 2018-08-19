using UnityEngine;
using Firebase.Auth;

public class FirebaseAuth
{
    /** auth 용 instance */
    protected Firebase.Auth.FirebaseAuth auth;
    /** 사용자 */
    protected FirebaseUser user;

    public UILabel debugText;

    public virtual void InitializeFirebaseAuth()
    {
        // 초기화
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;        
    }

    /** firebase 앱 내에 가입 여부를 체크한다. */
    protected bool SingedInFirebase
    {
        get
        {
            return user != auth.CurrentUser && auth.CurrentUser != null;
        }
    }    
}