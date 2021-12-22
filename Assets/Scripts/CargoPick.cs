using UnityEngine;
using UnityEngine.UI;

public class CargoPick : MonoBehaviour
{
    [SerializeField] private int mass;
    [SerializeField] private PlatformGround platformGround;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        platformGround.ChangeMass(mass);
    }
}
