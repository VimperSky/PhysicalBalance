using System.Collections;
using System.Collections.Generic;
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
    
    private readonly List<Button> _animatedButtons = new();

    private int _cargoId;
    
    private void Awake()
    {
        var levelData = LevelDataKeeper.Instance.LevelData;
        
        InitCargoPick(cargoPick1, levelData.CargoChoosingMasses[0]);
        InitCargoPick(cargoPick2, levelData.CargoChoosingMasses[1]);
        InitCargoPick(cargoPick3, levelData.CargoChoosingMasses[2]);
        InitCargoPick(cargoPick4, levelData.CargoChoosingMasses[3]);
    }
    
    private void InitCargoPick(Button cargoPick, int value)
    {
        var parent = cargoPick.transform.Find("CargoObj");
        cargoPick.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = value.ToString();
        var cargo = Instantiate(Resources.Load($"Prefabs/CargoMass/{value}"), parent) as GameObject;
        cargo.transform.localRotation = Quaternion.Euler(0f, 75f, 75f);
        cargo.AddComponent<CargoDummy>();

        cargoPick.onClick.AddListener(() => AddCargoMass(cargoPick, value));
    }
    
    private void AddCargoMass(Button cargoPick, int mass)
    {
        if (!_animatedButtons.Contains(cargoPick))
            StartCoroutine(ChangeColor(cargoPick));
        //cargoPick.gameObject.SetActive(false);
        platformGround.AddCargoMass(mass);
        MusicProvider.Instance.PlayClick();
    }

    private IEnumerator ChangeColor(Button cargoPick)
    {
        var text = cargoPick.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        var defaultColor = text.color;
        text.color = Color.green;

        _animatedButtons.Add(cargoPick);
        yield return new WaitForSeconds(1f);
        
        text.color = defaultColor;
        _animatedButtons.Remove(cargoPick);
    }
}