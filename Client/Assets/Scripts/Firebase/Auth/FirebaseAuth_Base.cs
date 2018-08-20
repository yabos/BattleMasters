using UnityEngine;
using Firebase.Auth;

public class FirebaseAuth_Base
{
    protected FirebaseAuthManager mOwner;


    public virtual void InitializeFirebaseAuth(FirebaseAuthManager owner)
    {
        // 초기화
        mOwner = owner;
    }    
}