using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Db { get; private set; }
    public bool IsInitialized { get; private set; } = false;


    private void Start()
    {
        Debug.Log("Starting Firebase initialization...");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.Result == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }

        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Initializing Firebase...");
        Auth = FirebaseAuth.DefaultInstance;
        Db = FirebaseFirestore.DefaultInstance;
        IsInitialized = true;
        Debug.Log("Firebase initialized successfully.");
    }
}


