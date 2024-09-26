using Firebase;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            Debug.Log("Firebase inicializado correctamente");
        });
    }
}
