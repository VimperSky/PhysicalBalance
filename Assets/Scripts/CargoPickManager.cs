using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoPickManager: MonoBehaviour
{
    [SerializeField] private Button cargoPick1;

    [SerializeField] private Button cargoPick2;

    [SerializeField] private Button cargoPick3;
    
    [SerializeField] private Button cargoPick4;

    [SerializeField] private GameObject cargoPrefab;

    [SerializeField] private PlatformGround platformGround;

    private int _cargoId;
    
    private void Start()
    {
        var levelData = LevelDataKeeper.Instance.LevelData;
        
        InitCargoPick(cargoPick1, levelData.CargoChoosingMasses[0]);
        InitCargoPick(cargoPick2, levelData.CargoChoosingMasses[1]);
        InitCargoPick(cargoPick3, levelData.CargoChoosingMasses[2]);
        InitCargoPick(cargoPick4, levelData.CargoChoosingMasses[3]);

        gameObject.SetActive(Config.IsDebugMode);
    }

    private void InitCargoPick(Button cargoPick, int value)
    {
        var parent = cargoPick.transform.Find("CargoObj");
        cargoPick.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = value.ToString();
        var cargo = Instantiate(Resources.Load($"Prefabs/CargoMass/{value}"), parent) as GameObject;
        //cargo.layer = 5; // UI
        //cargo.transform.localPosition += new Vector3(0, -cargo.transform.localScale.y / 20f, 0);
        cargo.transform.localRotation = Quaternion.Euler(0f, 75f, 75f);
        cargo.AddComponent<CargoDummy>();

        //cargoPick.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
        cargoPick.onClick.AddListener(() => AddCargoMass(cargoPick, value));
    }
    
    private void AddCargoMass(Button cargoPick, int mass)
    {
        //cargoPick.gameObject.SetActive(false);
        platformGround.AddCargoMass(mass);
        MusicProvider.Instance.PlayClick();
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