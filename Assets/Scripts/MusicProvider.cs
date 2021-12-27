using UnityEngine;

public class MusicProvider : MonoBehaviour
{ 
    public static MusicProvider Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void PlayClick()
    {
        GetComponent<AudioSource>().Play();
    }
}
