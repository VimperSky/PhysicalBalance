using System.Collections.Generic;
using Data;
using UnityEngine;

public class LevelDataKeeper : MonoBehaviour
{
    private static readonly List<LevelData> LevelDatas = new()
    {
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = 120, Mass = 5 },
                new() { Angle = 240, Mass = 5 },
                new() { Angle = 0, Mass = 0 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5},
            // Solution: 5
            DefeatMass = 6,
            UnknownCargoId = 2
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = -25f, Mass = 5 },
                new() { Angle = 25f, Mass = 5 },
                new() { Angle = 180, Mass = 0 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5}, 
            // Solution: 9
            DefeatMass = 11,
            UnknownCargoId = 2
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = 0f, Mass = 0 },
                new() { Angle = 90f, Mass = 7 },
                new() { Angle = 225, Mass = 10 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5}, 
            // Solution: 7
            DefeatMass = 8,
            UnknownCargoId = 0
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = -30, Mass = 7 },
                new() { Angle = 30, Mass = 5 },
                new() { Angle = 150, Mass = 7 },
                new() { Angle = 210, Mass = 0 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5}, 
            // Solution: 5
            DefeatMass = 6,
            UnknownCargoId = 3
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = -45f, Mass = 5 },
                new() { Angle = 0, Mass = 0 },
                new() { Angle = 45f, Mass = 5 },
                new() { Angle = 180, Mass = 12 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5}, 
            // Solution: 4 
            DefeatMass = 5,
            UnknownCargoId = 1
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = -45f, Mass = 5 },
                new() { Angle = 0, Mass = 0 },
                new() { Angle = 45f, Mass = 5 },
                new() { Angle = 180, Mass = 12 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5}, 
            // Solution: 4 
            DefeatMass = 5,
            UnknownCargoId = 1
        }
    };

    private int _currentLevelId;
    
    public static LevelDataKeeper Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    /// <summary>
    /// Сюда подается levelId, начиная с 1
    /// </summary>
    /// <param name="levelId"></param>
    public void SetLevelData(int levelId)
    {
        _currentLevelId = levelId;
        LevelData = LevelDatas[_currentLevelId - 1];
    }

    public void SetNextLevelData()
    {
        if (_currentLevelId < LevelDatas.Count)
        {
            SetLevelData(_currentLevelId + 1);
        }
        else
        {
            SetLevelData(_currentLevelId);
        }
    }

    public LevelData LevelData { get; private set; }
}
