using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUiController: MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject cargoPickManager;
    [SerializeField] private Button leftRotation;
    [SerializeField] private Button rightRotation;

    [SerializeField] private PlatformGround platformGround;
    [SerializeField] private GameObject targetInfo;

    [SerializeField] private GameObject hideBack;

    [SerializeField] private GameObject angle;

    [SerializeField] private GameObject angleRotationPanel;

    
    private void Awake()
    {
        homeButton.onClick.AddListener(() =>
        {
            MusicProvider.Instance.PlayClick();
            SceneManager.LoadScene(1);
        });
        var levelData = LevelDataKeeper.Instance.LevelData;

        if (levelData.IsRotationAvailable)
        {
            leftRotation.gameObject.SetActive(true);
            rightRotation.gameObject.SetActive(true);
            
            leftRotation.onClick.AddListener(OnLeftRotate);
            rightRotation.onClick.AddListener(OnRightRotate);
        }
    }

    private void Start()
    {
        cargoPickManager.gameObject.SetActive(MenuConfig.Instance.IsDebug);
        
        leftRotation.gameObject.SetActive(MenuConfig.Instance.IsDebug && LevelDataKeeper.Instance.LevelData.IsRotationAvailable);
        rightRotation.gameObject.SetActive(MenuConfig.Instance.IsDebug && LevelDataKeeper.Instance.LevelData.IsRotationAvailable);

        //targetInfo.gameObject.SetActive(!MenuConfig.Instance.IsDebug);
    }

    private void OnLeftRotate()
    {
        platformGround.RotateLeft();
    }
    
    private void OnRightRotate()
    {
        platformGround.RotateRight();
    }

    private void GameEnd()
    {
        cargoPickManager.gameObject.SetActive(false);
        leftRotation.gameObject.SetActive(false);
        rightRotation.gameObject.SetActive(false);
        hideBack.gameObject.SetActive(true);
    }
    
    public void Victory()
    {
        angle.gameObject.SetActive(false);
        GameEnd();
    }

    public void Defeat()
    {
        GameEnd();
    }

    private IEnumerator YouCanRotate()
    {
        angleRotationPanel.SetActive(true);

        yield return new WaitForSeconds(5);
        
        angleRotationPanel.SetActive(false);
    }
    
    
    public void TargetFound()
    {
        //targetInfo.gameObject.SetActive(false);
        cargoPickManager.gameObject.SetActive(true);
        if (LevelDataKeeper.Instance.LevelData.IsRotationAvailable)
        {
            StartCoroutine(YouCanRotate());
            
            leftRotation.gameObject.SetActive(true);
            rightRotation.gameObject.SetActive(true);
        }
    }

    public void TargetLost()
    {
        //targetInfo.gameObject.SetActive(true);
        cargoPickManager.gameObject.SetActive(false);
        leftRotation.gameObject.SetActive(false);
        rightRotation.gameObject.SetActive(false);
    }
}