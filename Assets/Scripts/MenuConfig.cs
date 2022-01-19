using UnityEngine;

public class MenuConfig: MonoBehaviour
{
    [SerializeField]
    private bool isDebugMode;

    public bool IsDebug => isDebugMode;
    
    public static MenuConfig Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

}