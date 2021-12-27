using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameController: MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button nextLevelButton;

    private void Awake()
    {
        retryButton.onClick.AddListener(Retry);
        homeButton.onClick.AddListener(GoHome);
        
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(NextLevel);
    }

    private void NextLevel()
    {
        MusicProvider.Instance.PlayClick();
    }

    private void GoHome()
    {
        MusicProvider.Instance.PlayClick();
        SceneManager.LoadScene(1);
    }

    private void Retry()
    {
        MusicProvider.Instance.PlayClick();
        SceneManager.LoadScene(2);
    }
}