using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private const float AngleDrawRadius = 2f;
    private const float CargoPlaceRadius = 5.05f;
    
    private readonly List<Cargo> _cargos = new();
    
    [SerializeField] private GameObject winTextPrefab;
    [SerializeField] private GameObject loseTextPrefab;

    [SerializeField] private GameObject cargoPickPanel;

    [SerializeField] private Transform canvasTransform;

    [SerializeField] private GameObject anglePrefab;
    [SerializeField] private GameObject anglePrefabHolder;

    [SerializeField] private GameObject hud;

    [SerializeField] private TextMeshProUGUI formula;

    private Vector3 _ringStartPosition;
    
    // Костыли, чтобы работал AR
    private Vector2 _ringStartPhysicsPosition;
    private Vector2 _ringPhysicsPosition;
    
    private LevelData _levelData;

    private GameState _gameState = GameState.NotStarted;

    [SerializeField] private Material lineMaterial;
    [SerializeField] private GameObject formulaInfo;

    private void SetAngleValueText(string value)
    {
        hud.transform.Find("AngleValue").GetComponent<TextMeshProUGUI>().text = value;
    }
    
    private void SetMassValueText(string value)
    {
        hud.transform.Find("MassValue").GetComponent<TextMeshProUGUI>().text = value;
    }

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

            var mediatorPos = cargoBasePos + cargosMediatorHolder.transform.position;
            var cargoMediator = Instantiate(cargoMediatorPrefab, mediatorPos, 
                Quaternion.Euler(0, 90 - cargoData.Angle, 0), cargosMediatorHolder.transform);
            var cargoMediatorScript = cargoMediator.AddComponent<CargoMediator>();
            cargoMediatorScript.SetData(new Vector2(cargoBasePos.x, cargoBasePos.z), angleRad);

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

        var totalMass = _cargos.Sum(x => x.TotalMass);
        SetMassValueText(totalMass.ToString());
    }

    private static bool IsPlatformInBalance(Vector2 force)
    {
        return Mathf.Abs(force.x) <= 0.001f && Mathf.Abs(force.y) <= 0.001f;
    }

    private int _cargosLeft = 4;

    private bool IsDefeat => _cargosLeft <= 0 || _cargos[_levelData.UnknownCargoId].TotalMass >= _levelData.DefeatMass;


    
    private void CalcPlatformAngle()
    {
        var massString = "Mass = -(";
        var formulaX = "X: (";
        var formulaY = "Y: (";
        var formulaString = "F = (";
        var resultForcePhys = new Vector2();
        var resultForce = new Vector2();
        for (var i = 0; i < _cargos.Count; i++)
        {
            resultForce += _cargos[i].Force;
            var angle = _levelData.CargoDatas[i].Angle;
            
            var mediator = _cargos[i].CargoMediator;
            resultForcePhys += new Vector2(Mathf.Cos(mediator.AngleRad), Mathf.Sin(mediator.AngleRad)) * _cargos[i].TotalMass;
            massString += $"cos{angle}sin{angle} * {_cargos[i].TotalMass}";

            formulaX += $"cos{angle} * {_cargos[i].TotalMass}";
            formulaY += $"sin{angle} * {_cargos[i].TotalMass}";
            
            if (i != _cargos.Count - 1)
            {
                massString += " + ";
                formulaX += " + ";
                formulaY += " + ";
            }
            formulaString += $"(cos({angle})sin({angle})) * {_cargos[i].TotalMass}";
            
            if (i != _cargos.Count - 1)
                formulaString += " + ";
        }

        formulaString += " ) / ( ";

        int sumMass = 0;
        for (var i = 0; i < _cargos.Count; i++)
        {
            var cargo = _cargos[i];
            sumMass += cargo.TotalMass;

            formulaString += cargo.TotalMass;
            if (i != _cargos.Count - 1)
                formulaString += " + ";
        }
        
        formulaX += $") = {resultForce.x:0.00}";
        formulaY += $") = {resultForce.y:0.00}";

        formula.text = formulaX + "\n" + formulaY;
        
        resultForcePhys /= sumMass;
        resultForce /= sumMass;
        resultForce /= 30f;


        //formulaString += $") / {sumMassFake} = " + (new Vector2(MathF.Truncate(resultForceFake.x * 100) / 100, MathF.Truncate(resultForce.y * 100) / 100)).ToString("0.0");
        formulaString += ") = " + resultForcePhys.ToString("0.00");

        var resultAngle = Mathf.Atan2(resultForce.x , resultForce.y) * Mathf.Rad2Deg;
        if (resultAngle < 0)
            resultAngle += 360;
        if (IsPlatformInBalance(resultForce))
            SetAngleValueText("0");
        else
            SetAngleValueText(resultAngle.ToString("0.0"));

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
            
            var formulaInfoObj = Instantiate(formulaInfo, canvasTransform, false);
            formulaInfoObj.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = formulaString;
            
            cargoPickPanel.SetActive(false);
        }
        else if (IsDefeat)
        {
            _gameState = GameState.Finished;
            _cargos[_levelData.UnknownCargoId].SetColor(Color.red);
            Instantiate(loseTextPrefab, canvasTransform, false);
            
            var formulaInfoObj = Instantiate(formulaInfo, canvasTransform, false);
            formulaInfoObj.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = formulaString;
            
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
        for (var i = 0; i < _cargos.Count; i++)
        {
            var cargo = _cargos[i];
            var cargoData = _levelData.CargoDatas[i];
            
            // Костыль для работы AR
            var centerPos = _ringPhysicsPosition;

            // var newAngle = Mathf.Atan2(cargo.CargoMediator.Position.y - centerPos.y,
            //     cargo.CargoMediator.Position.x - centerPos.x) * Mathf.Rad2Deg;
            // if (newAngle < 0)
            // {
            //     newAngle += 360;
            // }
            //
            // cargo.gameObject.transform.rotation = Quaternion.Euler(0, 90 - newAngle, 0);

            var prevAngleId = i == 0 ? _levelData.CargoDatas.Count - 1 : i - 1;
            var prevAngle = _levelData.CargoDatas[prevAngleId].Angle;

            var angleDelta = Mathf.DeltaAngle(cargoData.Angle, prevAngle);
            //var targetAngle = cargoData.Angle + angleDelta / 2;
            //var targetAngleRad = targetAngle * Mathf.Deg2Rad;
            var targetAngle = _levelData.CargoDatas[i].Angle;
            var targetAngleRad = _cargos[i].CargoMediator.AngleRad;
            
            
            var startPos = new Vector3(AngleDrawRadius * Mathf.Cos(targetAngleRad), 0f,
                AngleDrawRadius * Mathf.Sin(targetAngleRad));
            var angleObj = Instantiate(anglePrefab, startPos, Quaternion.Euler(0, 180  - targetAngle, 0),
                anglePrefabHolder.transform);
            var position = angleObj.transform.position;
            position += anglePrefabHolder.transform.position;
            angleObj.transform.position = new Vector3(position.x + _ringPhysicsPosition.x * 5,
                position.y, position.z + _ringPhysicsPosition.y * 5);
            
            angleObj.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Abs(targetAngle).ToString();
            // var circle = angleObj.AddComponent<Circle>();
            // circle.segments = 32;
            // circle.xradius = 2f;
            // circle.yradius = 2f;

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
        
        var totalMass = _cargos.Sum(x => x.TotalMass);
        SetMassValueText(totalMass.ToString());
        CalcPlatformAngle();
        DrawLines();
    }
}
