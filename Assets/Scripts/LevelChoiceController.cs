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
        level1.onClick.AddListener(() => OpenLevelWithData(1));
        level2.onClick.AddListener(() => OpenLevelWithData(2));
        level3.onClick.AddListener(() => OpenLevelWithData(3));
        level4.onClick.AddListener(() => OpenLevelWithData(4));
        level5.onClick.AddListener(() => OpenLevelWithData(5));

        close.onClick.AddListener(OnClose);
    }

    private void OpenLevelWithData(int levelId)
    {
        LevelDataKeeper.Instance.SetLevelData(levelId);
        MusicProvider.Instance.PlayClick();
        SceneManager.LoadScene(2);
    }
    
    private void OnClose()
    {
        MusicProvider.Instance.PlayClick();
        SceneManager.LoadScene(0);
    }
}