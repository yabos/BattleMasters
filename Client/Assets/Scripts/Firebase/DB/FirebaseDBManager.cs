using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseDBMamager : MonoBehaviour
{
    private static FirebaseDBMamager _instance;
    public static FirebaseDBMamager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(FirebaseDBMamager)) as FirebaseDBMamager;
                if (_instance == null)
                {
                    GameObject DBManaer = new GameObject("FirebaseDBMamager", typeof(FirebaseDBMamager));
                    _instance = DBManaer.GetComponent<FirebaseDBMamager>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        // Set this before calling into the realtime database.
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://flushx-f0024.firebaseio.com/");
        //FirebaseApp.DefaultInstance.SetEditorP12FileName("flushx-f0024-424618bd0ee8.p12");
        //FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("flushx-f0024@appspot.gserviceaccount.com");
        //FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");

        DontDestroyOnLoad(this);
    }

    public void InitUserData(string userId, string userName)
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabaseRef.Child("users").Child(userId).Child("userdata").SetValueAsync(userName).ContinueWith(
            task => {
                Debug.Log(string.Format("OnClickSave::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );
    }

    public void GetUsers(string userId)
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabaseRef.Child("users").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
                // Handle the error...
                Debug.Log("fai1l");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("suc");
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot user in snapshot.Children)
                {
                    IDictionary dictUser = (IDictionary)user.Value;
                    Debug.Log("" + dictUser["email"] + " - " + dictUser["password"]);
                }
            }
        });
    }


    public void OnClickUpdateChildren()
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        string userId = "testUserId";

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/users/" + userId] = "editedTestUserName";

        mDatabaseRef.UpdateChildrenAsync(childUpdates).ContinueWith(
            task =>
            {
                Debug.Log(string.Format("OnClickUpdateChildren::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );      
    }

    public void OnClickPush()
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        string key = mDatabaseRef.Child("scores").Push().Key;
        int entryValues = 100;
        string userId = "testUserId";

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/scores/" + key] = entryValues;
        childUpdates["/user-scores/" + userId + "/" + key] = entryValues;
        childUpdates["/users/" + userId + "/" + "scoreKey"] = key;

        mDatabaseRef.UpdateChildrenAsync(childUpdates).ContinueWith(
            task =>
            {
                Debug.Log(string.Format("OnClickPush::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );
    }

    public void OnClickRemove()
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;


        mDatabaseRef.Child("users").Child("testUserId").Child("scoreKey")
        .GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string scoreKey = (string)snapshot.Value;
                Dictionary<string, object> childUpdates = new Dictionary<string, object>();
                childUpdates["/users/" + "testUserId"] = null;
                childUpdates["/scores/" + scoreKey] = null;
                childUpdates["/user-scores/" + "testUserId" + "/" + scoreKey] = null;
                mDatabaseRef.UpdateChildrenAsync(childUpdates).ContinueWith(
                      updateTask =>
                      {
                          Debug.Log(string.Format("OnClickRemove::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", updateTask.IsCompleted, updateTask.IsCanceled, updateTask.IsFaulted));
                      }
                  );
            }
        }
        );
    }

    public void OnClickMaxScores()
    {
        const int MaxScoreRecordCount = 5;
        int score = Random.Range(0, 100);
        string email = "testEmail";

        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabaseRef.Child("top5scores").RunTransaction(mutableData => {
            List<object> leaders = mutableData.Value as List<object>;

            if (leaders == null)
            {
                leaders = new List<object>();
            }
            else if (mutableData.ChildrenCount >= MaxScoreRecordCount)
            {
                long minScore = long.MaxValue;
                object minVal = null;
                foreach (var child in leaders)
                {
                    if (!(child is Dictionary<string, object>))
                        continue;
                    long childScore = (long)((Dictionary<string, object>)child)["score"];
                    if (childScore < minScore)
                    {
                        minScore = childScore;
                        minVal = child;
                    }
                }
                if (minScore > score)
                {
                    // The new score is lower than the existing 5 scores, abort.
                    return TransactionResult.Abort();
                }

                // Remove the lowest score.
                leaders.Remove(minVal);
            }

            Dictionary<string, object> entryValues = new Dictionary<string, object>();
            entryValues.Add("score", score);
            entryValues.Add("email", email);
            leaders.Add(entryValues);

            mutableData.Value = leaders;
            return TransactionResult.Success(mutableData);
        }).ContinueWith(
            task =>
            {
                Debug.Log(string.Format("OnClickMaxScores::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );
    }

    public void OnClickOrderBy()
    {
        FirebaseDatabase.DefaultInstance.GetReference("scores").OrderByValue().LimitToLast(3)
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                }
            });
    }

    public void OnClickListener()
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.GetReference("users");
        mDatabaseRef.ChildAdded += (object sender, ChildChangedEventArgs args) => {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            Debug.Log(string.Format("ChildAdded:{0}", args.Snapshot));
        };

        mDatabaseRef.ChildChanged += (object sender, ChildChangedEventArgs args) => {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            Debug.Log(string.Format("ChildChanged:{0}", args.Snapshot));
        };

        mDatabaseRef.ChildRemoved += (object sender, ChildChangedEventArgs args) => {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            Debug.Log(string.Format("ChildRemoved:{0}", args.Snapshot));
        };

        mDatabaseRef.ChildMoved += (object sender, ChildChangedEventArgs args) => {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            Debug.Log(string.Format("ChildMoved:{0}", args.Snapshot));
        };
    }

    public void OnClickSave2()
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        string key = mDatabaseRef.Child("scores").Push().Key;
        int entryValues = Random.Range(0, 100);
        string userId = "testUserId1";

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/users/" + userId + "/" + "username"] = "editedTestUserName";
        childUpdates["/scores/" + key] = entryValues;
        childUpdates["/user-scores/" + userId + "/" + key] = entryValues;

        mDatabaseRef.UpdateChildrenAsync(childUpdates).ContinueWith(
            task =>
            {
                Debug.Log(string.Format("OnClickSave2::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );
    }
}