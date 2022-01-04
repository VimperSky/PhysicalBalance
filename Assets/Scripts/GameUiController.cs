using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUiController: MonoBehaviour
{
    [SerializeField] private Button homeButton;

    private void Awake()
    {
        homeButton.onClick.AddListener(() =>
        {
            MusicProvider.Instance.PlayClick();
            SceneManager.LoadScene(1);
        });
    }
}