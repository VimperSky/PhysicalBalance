using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoiceController : MonoBehaviour
{
    [SerializeField] private Button level1;
    [SerializeField] private Button level2;
    [SerializeField] private Button level3;
    [SerializeField] private Button level4;
    [SerializeField] private Button level5;
    
    [SerializeField] private Button close;

    private void Awake()
    {
        level1.onClick.AddListener(OpenLevel1);
        close.onClick.AddListener(OnClose);
    }

    private void OpenLevel1()
    {
        MusicProvider.Instance.PlayClick();
        SceneManager.LoadScene(2);
    }

    private void OnClose()
    {
        MusicProvider.Instance.PlayClick();
        SceneManager.LoadScene(0);
    }

    // public void Resume()
    // {
    //     PlaySoundOnClick();

    //     pauseMenuUI.SetActive(false);
    //     Time.timeScale = 1f;
    //     isGamePaused = false;
    // }

    // public void Pause()
    // {
    //     PlaySoundOnClick();

    //     pauseMenuUI.SetActive(true);
    //     Time.timeScale = 0f;
    //     isGamePaused = true;
    // }
    
}