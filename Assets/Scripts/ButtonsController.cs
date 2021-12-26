using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour
{
    // public static bool isGamePaused;
    // public GameObject pauseMenuUI;

    public void Play()
    {
        PlaySoundOnClick();

        SceneManager.LoadScene("LevelChoice");
    }

    public void Replay()
    {
        PlaySoundOnClick();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Close()
    {
        PlaySoundOnClick();

        SceneManager.LoadScene("Main");
    }

    public void LoadScene()
    {
        PlaySoundOnClick();

        SceneManager.LoadScene(gameObject.name);
    }

    public void LoadNextLevel()
    {
        PlaySoundOnClick();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

    public void PlaySoundOnClick()
    {
        GetComponent<AudioSource>().Play();
    }
}