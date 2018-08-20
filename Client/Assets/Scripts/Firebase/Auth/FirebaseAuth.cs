using UnityEngine;
using Firebase.Auth;

public class FirebaseAuth
{
    protected FirebaseAuthManager mOwner;


    public virtual void InitializeFirebaseAuth(FirebaseAuthManager owner)
    {
        // 초기화
        mOwner = owner;
    }    
}