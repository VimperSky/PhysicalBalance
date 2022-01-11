using UnityEngine;
using UnityEngine.UI;

public class CargoPickManager: MonoBehaviour
{
    [SerializeField] private Button cargoPick1;

    [SerializeField] private Button cargoPick2;

    [SerializeField] private Button cargoPick3;
    
    [SerializeField] private GameObject cargoPrefab;

    [SerializeField] private PlatformGround platformGround;

    private int _cargoId;
    
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
        var parent = cargoPick.transform.Find("CargoObj");
        var cargo = Instantiate(cargoPrefab, parent);
        //cargo.layer = 5; // UI
        cargo.transform.localRotation = Quaternion.Euler(90f, 150f, 0);
        //cargo.transform.localPosition += new Vector3(0, 0.25f, 0);
        var cargoScript = cargo.AddComponent<CargoDummy>();
        cargoScript.SetMass(value);
        //cargo.transform.Translate(0, 1f, 0);

        //cargoPick.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
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