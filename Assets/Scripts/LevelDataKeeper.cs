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
            UnknownCargoId = 2,
            IsRotationAvailable = false
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = 0f, Mass = 5 },
                new() { Angle = 50f, Mass = 5 },
                new() { Angle = 205f, Mass = 0 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5}, 
            // Solution: 9
            DefeatMass = 11,
            UnknownCargoId = 2,
            IsRotationAvailable = false
        }, 
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = 0f, Mass = 0 },
                new() { Angle = 90f, Mass = 7 },
                new() { Angle = 225f, Mass = 10 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5}, 
            // Solution: 7
            DefeatMass = 8,
            UnknownCargoId = 0,
            IsRotationAvailable = false
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = 0, Mass = 7 },
                new() { Angle = 60f, Mass = 7 },
                new() { Angle = 250, Mass = 0 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5},
            // Solution: 12
            DefeatMass = 14,
            UnknownCargoId = 2,
            IsRotationAvailable = true
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = 0, Mass = 5 },
                new() { Angle = 180f, Mass = 0 },
                new() { Angle = 205f, Mass = 9 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5},
            // Solution: 9
            DefeatMass = 11,
            UnknownCargoId = 1,
            IsRotationAvailable = true
        },
        new LevelData
        {
            CargoDatas = new List<CargoData>
            {
                new() { Angle = 0, Mass = 6 },
                new() { Angle = 95, Mass = 1 },
                new() { Angle = 125, Mass = 0 }
            },
            CargoChoosingMasses = new () {1, 2, 3, 5},
            // Solution: 5
            DefeatMass = 7,
            UnknownCargoId = 2,
            IsRotationAvailable = true,
            IsFinalLevel = true
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
