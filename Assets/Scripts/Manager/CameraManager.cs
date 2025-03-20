using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static CameraManager _instance;
    
    void Awake()
    {
        ManageSingleton();
    }
    
    // Applying singleton pattern
    void ManageSingleton()
    {
        if (_instance)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
