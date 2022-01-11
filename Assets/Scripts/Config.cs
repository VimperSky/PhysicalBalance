using UnityEngine;

public class Config : MonoBehaviour
{
    public static readonly bool IsDebugMode = false;

    [SerializeField] private GameObject regularCamera;
    [SerializeField] private GameObject arCamera;
    [SerializeField] private GameObject imageTarget;
    [SerializeField] private GameObject game;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        if (IsDebugMode)
        {
            arCamera.SetActive(false);
            imageTarget.SetActive(false);
            
            regularCamera.SetActive(true);
            canvas.worldCamera = regularCamera.GetComponent<Camera>();
        }
        else
        {
            regularCamera.SetActive(false);

            game.transform.SetParent(imageTarget.transform);

            arCamera.SetActive(true);
            imageTarget.SetActive(true);
            canvas.worldCamera = arCamera.GetComponent<Camera>();
        }
    }
}
