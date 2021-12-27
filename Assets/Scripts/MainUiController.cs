using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUiController : MonoBehaviour
{
    [SerializeField] private Button playBtn;

    private void Awake()
    {
        playBtn.onClick.AddListener(Play);
    }

    public void Play()
    {
        MusicProvider.Instance.PlayClick();
        SceneManager.LoadScene(1);
    }
    
}