using System.Collections.Generic;
using Data;
using UnityEngine;

public class PlatformGround : MonoBehaviour
{
    [SerializeField] private GameObject cargosHolder;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private GameObject linesHolder;
    [SerializeField] private GameObject linePrefab;
    
    [SerializeField] private GameObject ring;
    
    private const float RingRadius = 1.25f;
    private const float CargoPlaceRadius = 4.5f;
    
    private readonly List<Cargo> _cargos = new();
    
    [SerializeField] private GameObject winTextPrefab;
    [SerializeField] private GameObject loseTextPrefab;

    [SerializeField] private GameObject cargoPickPanel;

    [SerializeField] private Transform canvasTransform;

    private Vector3 _ringStartPosition;
    
    // Костыли, чтобы работал AR
    private Vector2 _ringStartPhysicsPosition;
    private Vector2 _ringPhysicsPosition;
    
    private LevelData _levelData;

    private GameState _gameState = GameState.NotStarted;

    public void TargetFound()
    {
        if (Config.IsDebugMode)
            return;
            
        InitGame();
    }
    
    public void TargetLost()
    {
        if (Config.IsDebugMode)
            return;
    }

    private void Start()
    {
        if (Config.IsDebugMode)
            InitGame();
    }

    private void InitGame()
    {
        if (_gameState != GameState.NotStarted) 
            return;
        
        _levelData  = LevelDataKeeper.Instance.LevelData;
        
        // Это все костыли для AR
        _ringStartPosition = ring.transform.localPosition;
        _ringStartPhysicsPosition = new Vector2(ring.transform.position.x, ring.transform.position.z);
        _ringPhysicsPosition = _ringStartPhysicsPosition;
        
        SpawnCargos(_levelData);
        CalcPlatformAngle();
        DrawLines();
        
        _gameState = GameState.Started;
    }
    
    private void SpawnCargos(LevelData levelData)
    {
        for (var i = 0; i < levelData.CargoDatas.Count; i++)
        {
            var cargoData = levelData.CargoDatas[i];
            var angleRad = cargoData.Angle * Mathf.PI / 180f;
            var cargoPos = new Vector3( CargoPlaceRadius * Mathf.Cos(angleRad), 0f, CargoPlaceRadius * Mathf.Sin(angleRad));
            cargoPos += cargosHolder.transform.position;
            var cargo = Instantiate(cargoPrefab, cargoPos, Quaternion.Euler(0, 90 - cargoData.Angle, 0), cargosHolder.transform);
            var cargoScript = cargo.AddComponent<Cargo>();
            cargoScript.SetData(cargoData.Mass, new Vector2(cargoPos.x, cargoPos.z), angleRad, i == levelData.UnknownCargoId);
            _cargos.Add(cargoScript);
        }
    }
    

    private void DrawLine(Cargo cargo, float angleRad)
    {
        var cargoPosition = new Vector2(cargo.transform.position.x, cargo.transform.position.z);
        var lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineObj.transform.SetParent(linesHolder.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
        lineRenderer.widthMultiplier = 0.1f;
        
        var startPos = new Vector3(RingRadius * Mathf.Cos(angleRad), 0f, RingRadius * Mathf.Sin(angleRad));
        startPos += ring.transform.position;
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, new Vector3(cargoPosition.x, linesHolder.transform.position.y, cargoPosition.y));
    }


    private static bool IsVictory(Vector2 force)
    {
        return Mathf.Abs(force.x) <= 0.01f && Mathf.Abs(force.y) <= 0.01f;
    }

    private void CalcPlatformAngle()
    {
        var resultForce = new Vector2();
        foreach (var cargo in _cargos)
        {
            resultForce += cargo.Force;
        }

        resultForce /= 400f;
        if (resultForce.x > 0.1f)
            resultForce.x = 0.1f;
        if (resultForce.y > 0.1f)
            resultForce.y = 0.1f;

        // * 5f это костыль для AR, по-другому хз как сделать.
        _ringPhysicsPosition = new Vector2(_ringStartPhysicsPosition.x + resultForce.x * 5f, _ringStartPhysicsPosition.y + resultForce.y * 5f);
        
        ring.transform.localPosition = new Vector3(_ringStartPosition.x + resultForce.x, _ringStartPosition.y, _ringStartPosition.z + resultForce.y);
        
        if (_gameState != GameState.Started) 
            return;
        _gameState = GameState.Finished;

        if (IsVictory(resultForce))
        {
            Instantiate(winTextPrefab, canvasTransform, false);
            cargoPickPanel.SetActive(false);
        }
        else
        { 
            Instantiate(loseTextPrefab, canvasTransform, false);
            cargoPickPanel.SetActive(false);
        }
    }
    

    private void ClearLines()
    {
        foreach (Transform child in linesHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void DrawLines()
    {
        ClearLines();
        foreach (var cargo in _cargos)
        {
            DrawLine(cargo, cargo.AngleRad);
            
            // Костыль для работы AR
            var centerPos = _ringPhysicsPosition;
            
            var newAngle = Mathf.Atan2(cargo.Position.y - centerPos.y, cargo.Position.x - centerPos.x) * Mathf.Rad2Deg;
            if (newAngle < 0)  
            {
                newAngle += 360;
            }

            cargo.gameObject.transform.rotation = Quaternion.Euler(0, 90 - newAngle, 0);
        }
    }
    

    public void ChangeMass(float value)
    {
        if (_gameState != GameState.Started)
            return;
        
        _cargos[_levelData.UnknownCargoId].SetMass(value, true);
        
        CalcPlatformAngle();
        DrawLines();
    }
}
