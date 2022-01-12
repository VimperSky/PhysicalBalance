using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;

public class PlatformGround : MonoBehaviour
{
    [SerializeField] private GameObject cargosMediatorHolder;
    [SerializeField] private GameObject cargosHolder;

    [SerializeField] private GameObject cargoMediatorPrefab;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private GameObject linesHolder;
    [SerializeField] private GameObject linePrefab;
    
    [SerializeField] private GameObject ring;
    
    private const float RingRadius = 1.15f;
    private const float AngleDrawRadius = 2.5f;
    private const float CargoPlaceRadius = 5.05f;
    
    private readonly List<Cargo> _cargos = new();
    
    [SerializeField] private GameObject winTextPrefab;
    [SerializeField] private GameObject loseTextPrefab;

    [SerializeField] private GameObject cargoPickPanel;

    [SerializeField] private Transform canvasTransform;

    [SerializeField] private GameObject anglePrefab;
    [SerializeField] private GameObject anglePrefabHolder;

    private Vector3 _ringStartPosition;
    
    // Костыли, чтобы работал AR
    private Vector2 _ringStartPhysicsPosition;
    private Vector2 _ringPhysicsPosition;
    
    private LevelData _levelData;

    private GameState _gameState = GameState.NotStarted;

    [SerializeField] private Material lineMaterial;

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
            var angleRad = cargoData.Angle * Mathf.Deg2Rad;
            var cargoBasePos = new Vector3( CargoPlaceRadius * Mathf.Cos(angleRad), 0f, CargoPlaceRadius * Mathf.Sin(angleRad));
            
            var prevAngleId = i == 0 ? levelData.CargoDatas.Count - 1 : i - 1;
            var prevAngle = levelData.CargoDatas[prevAngleId].Angle;
           
            var angleDelta = Mathf.DeltaAngle(cargoData.Angle, prevAngle);
            var targetAngle = (cargoData.Angle + angleDelta / 2);
            var targetAngleRad = targetAngle  * Mathf.Deg2Rad;
            var startPos = new Vector3(AngleDrawRadius * Mathf.Cos(targetAngleRad), 0f, AngleDrawRadius * Mathf.Sin(targetAngleRad));
            var angleObj = Instantiate(anglePrefab, startPos, Quaternion.Euler(0, 180 - 90 - targetAngle, 0), anglePrefabHolder.transform);
            angleObj.transform.position += anglePrefabHolder.transform.position;
            angleObj.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Abs(angleDelta).ToString();
            
            var mediatorPos = cargoBasePos + cargosMediatorHolder.transform.position;
            var cargoMediator = Instantiate(cargoMediatorPrefab, mediatorPos, 
                Quaternion.Euler(0, 90 - cargoData.Angle, 0), cargosMediatorHolder.transform);
            var cargoMediatorScript = cargoMediator.AddComponent<CargoMediator>();
            cargoMediatorScript.SetData(new Vector2(cargoBasePos.x, cargoBasePos.y), angleRad);

            var cargoPos = cargoBasePos + cargosHolder.transform.position;
            var cargo = Instantiate(cargoPrefab, cargoPos, Quaternion.Euler(0, 90 - cargoData.Angle, 0), cargosHolder.transform);
            var cargoScript = cargo.AddComponent<Cargo>();
            cargoScript.SetData(cargoData.Mass, cargoMediatorScript);

            if (i == _levelData.UnknownCargoId)
            {
                cargoScript.SetColor(Color.yellow);
            }
            
            _cargos.Add(cargoScript);
        }
    }

    private static bool IsPlatformInBalance(Vector2 force)
    {
        return Mathf.Abs(force.x) <= 0.01f && Mathf.Abs(force.y) <= 0.01f;
    }

    private int _cargosLeft = 4;

    private bool IsDefeat => _cargosLeft <= 0 || _cargos[_levelData.UnknownCargoId].TotalMass >= _levelData.DefeatMass;


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
        _ringPhysicsPosition = new Vector2(_ringStartPhysicsPosition.x + resultForce.x, _ringStartPhysicsPosition.y + resultForce.y);
        
        ring.transform.localPosition = new Vector3(_ringStartPosition.x + resultForce.x, _ringStartPosition.y, _ringStartPosition.z + resultForce.y);
        
        if (_gameState != GameState.Started) 
            return;
        
        if (IsPlatformInBalance(resultForce))
        {
            _gameState = GameState.Finished;
            _cargos[_levelData.UnknownCargoId].SetColor(Color.green);
            Instantiate(winTextPrefab, canvasTransform, false);
            cargoPickPanel.SetActive(false);
        }
        else if (IsDefeat)
        {
            _gameState = GameState.Finished;
            _cargos[_levelData.UnknownCargoId].SetColor(Color.red);
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
    
    
    private void DrawLineToMediator(CargoMediator cargo)
    {
        var lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineObj.transform.SetParent(linesHolder.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.material = lineMaterial;
        
        var startPos = new Vector3(RingRadius * Mathf.Cos(cargo.AngleRad), 0f, RingRadius * Mathf.Sin(cargo.AngleRad));
        startPos += ring.transform.position;
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, cargo.transform.position);
    }
    
    private void DrawLineToCargo(Cargo cargo)
    {
        var lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineObj.transform.SetParent(linesHolder.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.material = lineMaterial;
        
        var startPos = cargo.transform.position;
        var endPos = cargo.CargoMediator.transform.position;
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
    
    
    private void DrawLines()
    {
        ClearLines();
        foreach (var cargo in _cargos)
        {
            // Костыль для работы AR
            var centerPos = _ringPhysicsPosition;
            
            var newAngle = Mathf.Atan2(cargo.CargoMediator.Position.y - centerPos.y, cargo.CargoMediator.Position.x - centerPos.x) * Mathf.Rad2Deg;
            if (newAngle < 0)  
            {
                newAngle += 360;
            }

            cargo.gameObject.transform.rotation = Quaternion.Euler(0, 90 - newAngle, 0);
            
            DrawLineToMediator(cargo.CargoMediator);
            DrawLineToCargo(cargo);
        }
    }
    

    public void AddCargoMass(int value)
    {
        if (_gameState != GameState.Started)
            return;
        
        _cargosLeft--;
        _cargos[_levelData.UnknownCargoId].AdjustCargoMass(value);
        
        CalcPlatformAngle();
        DrawLines();
    }
}
