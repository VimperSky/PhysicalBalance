using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoPickManager: MonoBehaviour
{
    [SerializeField] private Button cargoPick1;

    [SerializeField] private Button cargoPick2;

    [SerializeField] private Button cargoPick3;
    
    [SerializeField] private PlatformGround platformGround;

    private void Start()
    {
        var levelData = LevelDataKeeper.Instance.LevelData;
        
        InitCargoPick(cargoPick1, levelData.CargoChoosingMasses[0]);
        InitCargoPick(cargoPick2, levelData.CargoChoosingMasses[1]);
        InitCargoPick(cargoPick3, levelData.CargoChoosingMasses[2]);

        gameObject.SetActive(Config.IsDebugMode);
    }

    private void InitCargoPick(Button cargoPick, float value)
    {
        cargoPick.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
        cargoPick.onClick.AddListener(() => PickMass(value));
    }
    
    private void PickMass(float mass)
    {
        platformGround.ChangeMass(mass);
    }
    
    
    public void TargetFound()
    {
        gameObject.SetActive(true);
    }

    public void TargetLost()
    {
        gameObject.SetActive(false);
    }
}