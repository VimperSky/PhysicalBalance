using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject axisLinePrefab;

    [SerializeField] private GameObject axisLinesHolder;
    [SerializeField] private GameObject ring;
    
    private const float RingRadius = 1.15f;
    private const float AngleDrawRadius = 1.15f;
    private const float AxisDrawRadius = 5.05f;

    private readonly List<Cargo> _cargos = new();
    
    [SerializeField] private GameObject winTextPrefab;
    [SerializeField] private GameObject loseTextPrefab;

    [SerializeField] private GameUiController gameUiController;

    [SerializeField] private Transform canvasTransform;

    [SerializeField] private GameObject anglePrefab;
    [SerializeField] private GameObject anglePrefabHolder;

    [SerializeField] private GameObject hud;

    [SerializeField] private TextMeshProUGUI formula;

    private Vector3 _ringStartPosition;
    
    // Костыли, чтобы работал AR
    
    private LevelData _levelData;

    private GameState _gameState = GameState.NotStarted;

    [SerializeField] private Material lineMaterial;

    private float _resultAngle;

    private bool _init = true;
    
    private void SetAngleValueText(string value)
    {
        hud.transform.Find("AngleValue").GetComponent<TextMeshProUGUI>().text = value;
    }

    public void TargetFound()
    {
        if (MenuConfig.Instance.IsDebug)
            return;
        
        hud.gameObject.SetActive(true);
            
        InitGame();
    }
    
    public void TargetLost()
    {
        if (MenuConfig.Instance.IsDebug)
            return;
        
        hud.gameObject.SetActive(false);

    }

    private void Start()
    {
        if (MenuConfig.Instance.IsDebug)
            InitGame();
    }



    private void InitGame()
    {
        if (_gameState != GameState.NotStarted) 
            return;
        
        _levelData  = LevelDataKeeper.Instance.LevelData;
        
        // Это все костыли для AR
        _ringStartPosition = ring.transform.localPosition;
        
        SpawnCargos(_levelData);
        DrawAxis();
        CalculateGameState();

        _gameState = GameState.Started;
    }

    private static Vector3 GetAnglePos(float angleRad)
    {
        return new Vector3( AxisDrawRadius * Mathf.Cos(angleRad), 0f, AxisDrawRadius * Mathf.Sin(angleRad));
    }
    
    private void DrawAxis()
    {
        var angle0 = 0;
        var angle1 = 180 * Mathf.Deg2Rad;

        var angle2 = 90 * Mathf.Deg2Rad;
        var angle3 = 270 * Mathf.Deg2Rad;
        
        {
            var lineObj = Instantiate(axisLinePrefab, Vector3.zero, Quaternion.identity);
            lineObj.transform.position += linesHolder.transform.position;
            var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
            lineRenderer.widthMultiplier = 0.02f;
            lineRenderer.SetPosition(0, GetAnglePos(angle0));
            lineRenderer.SetPosition(1, GetAnglePos(angle1));
        }

        {
            var lineObj = Instantiate(axisLinePrefab, Vector3.zero, Quaternion.identity);
            lineObj.transform.position += linesHolder.transform.position;
            var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
            lineRenderer.widthMultiplier = 0.01f;
            lineRenderer.SetPosition(0, GetAnglePos(angle2));
            lineRenderer.SetPosition(1, GetAnglePos(angle3));
        }
    }
    
    private void SpawnCargos(LevelData levelData)
    {
        for (var i = 0; i < levelData.CargoDatas.Count; i++)
        {
            var cargoData = levelData.CargoDatas[i];
            var angleRad = cargoData.Angle * Mathf.Deg2Rad;
            var cargoBasePos = new Vector3( Cargo.PlaceRadius * Mathf.Cos(angleRad), 0f, Cargo.PlaceRadius * Mathf.Sin(angleRad));

            var mediatorPos = cargoBasePos + cargosMediatorHolder.transform.position;
            var cargoMediator = Instantiate(cargoMediatorPrefab, mediatorPos, 
                Quaternion.Euler(0, 90 - cargoData.Angle, 0), cargosMediatorHolder.transform);
            var cargoMediatorScript = cargoMediator.AddComponent<CargoMediator>();
            cargoMediatorScript.SetData(new Vector2(cargoBasePos.x, cargoBasePos.z), angleRad);

            var cargoPos = cargoBasePos + cargosHolder.transform.position;
            var cargo = Instantiate(cargoPrefab, cargoPos, Quaternion.Euler(0, 90 - cargoData.Angle, 0), cargosHolder.transform);
            var cargoScript = cargo.AddComponent<Cargo>();
            cargoScript.SetData(cargoData.Angle, cargoData.Mass, cargoMediatorScript);

            if (i == _levelData.UnknownCargoId)
            {
                cargoScript.SetColor(Color.yellow);
            }

            _cargos.Add(cargoScript);
        }
    }

    private static bool IsPlatformInBalance(Vector2 force)
    {
        return Mathf.Abs(force.x) <= 0.001f && Mathf.Abs(force.y) <= 0.001f;
    }

    private int _cargosLeft = 4;

    private bool IsDefeat => _cargosLeft <= -5 || _cargos[_levelData.UnknownCargoId].TotalMass >= _levelData.DefeatMass;


    
    private void CalcPlatformAngle()
    {
        var formulaX = "X: (";
        var formulaY = "Y: (";
        var resultForcePhys = new Vector2();
        var resultForce = new Vector2();
        for (var i = 0; i < _cargos.Count; i++)
        {
            resultForce += _cargos[i].Force;
            var angle = _cargos[i].Angle;
            
            var mediator = _cargos[i].CargoMediator;
            resultForcePhys += new Vector2(Mathf.Cos(mediator.AngleRad), Mathf.Sin(mediator.AngleRad)) * _cargos[i].TotalMass;
            
            if (i == _levelData.UnknownCargoId)
            {
                formulaX += "<b><color=yellow>";
                formulaY += "<b><color=yellow>";
            }
            formulaX += $"cos{angle} * {_cargos[i].TotalMass}";
            formulaY += $"sin{angle} * {_cargos[i].TotalMass}";
            if (i == _levelData.UnknownCargoId)
            {
                formulaX += "</b></color>";
                formulaY += "</b></color>";
            }
            
            if (i != _cargos.Count - 1)
            {
                formulaX += " + ";
                formulaY += " + ";
            }
        }
        
        var sumMass = _cargos.Sum(cargo => cargo.TotalMass);
     
        
        resultForcePhys /= sumMass;
        resultForce /= sumMass;
        resultForce /= 30f;


        formulaX += $") / {sumMass} = {resultForcePhys.x:0.00}";
        formulaY += $") / {sumMass}= {resultForcePhys.y:0.00}";

        if (_levelData.IsRotationAvailable)
        {
            formula.transform.parent.gameObject.SetActive(true);
            formula.text = formulaX + "\n" + formulaY;
        }

        
        var resultAngle = Mathf.Atan2(resultForcePhys.y , resultForcePhys.x) * Mathf.Rad2Deg;
        if (resultAngle < 0)
            resultAngle += 360;

        _resultAngle = resultAngle;

        if (_init)
        {
            _init = false;
        }
        else
        {
            ring.GetComponent<Animator>().CrossFade("RingMoving", 0.0f);
        }
        ring.transform.localPosition = new Vector3(_ringStartPosition.x + resultForce.x, _ringStartPosition.y, _ringStartPosition.z + resultForce.y);
        
        if (_gameState != GameState.Started) 
            return;
        
        if (IsPlatformInBalance(resultForce))
        {
            _gameState = GameState.Finished;
            _cargos[_levelData.UnknownCargoId].SetColor(Color.green);
            Instantiate(winTextPrefab, canvasTransform, false);
            formula.transform.parent.gameObject.SetActive(false);
            
            GameObject.Find("Home").SetActive(false);
            
            gameUiController.Victory();
            
            
            if (_levelData.IsFinalLevel)
                GameObject.Find("Next").SetActive(false);
        }
        else if (IsDefeat)
        {
            _gameState = GameState.Finished;
            _cargos[_levelData.UnknownCargoId].SetColor(Color.red);
            Instantiate(loseTextPrefab, canvasTransform, false);
            formula.transform.parent.gameObject.SetActive(false);

            GameObject.Find("Home").SetActive(false);

            gameUiController.Defeat();
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
        
        lineRenderer.widthMultiplier = 0.04f;
        lineRenderer.material = lineMaterial;
        
        var basePos = new Vector3(RingRadius * Mathf.Cos(cargo.AngleRad), 
            0f, RingRadius * Mathf.Sin(cargo.AngleRad));
        basePos += ring.transform.position;
        
        lineRenderer.SetPosition(0, basePos);
        lineRenderer.SetPosition(1, cargo.transform.position);
    }
    
    private void DrawLineToCargo(Cargo cargo)
    {
        var lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineObj.transform.SetParent(linesHolder.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
        lineRenderer.widthMultiplier = 0.04f;
        lineRenderer.material = lineMaterial;
        
        var startPos = cargo.transform.position;
        var endPos = cargo.CargoMediator.transform.position;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
    
    private void PutAngles()
    {
        foreach (Transform child in anglePrefabHolder.transform)
        {
            Destroy(child.gameObject);
        }
        
        var sortedCargos = _cargos.OrderBy(x => x.Angle).ToList();
        for (var i = 0; i < sortedCargos.Count; i++)
        {
            var cargo = sortedCargos[i];
            
            var prevAngleId = i == 0 ? sortedCargos.Count - 1 : i - 1;
            var prevAngle = sortedCargos[prevAngleId].Angle;
            
            var angleDelta = MathExt.Mod(cargo.Angle - prevAngle, 360);
            var anglePosition = MathExt.Mod(prevAngle + angleDelta / 2, 360);

            var posibleAngles = Enumerable.Range((int)prevAngle, (int)angleDelta);
            var findingAngle = (int)_resultAngle;
            if (posibleAngles.Contains(findingAngle))
                SetAngleValueText(angleDelta.ToString("0"));
            
            if (angleDelta == 0)
                continue;

            var targetAngleRad = anglePosition * Mathf.Deg2Rad;
            
            var startPos = new Vector3(AngleDrawRadius * Mathf.Cos(targetAngleRad), 0f,
                AngleDrawRadius * Mathf.Sin(targetAngleRad));
            var angleObj = Instantiate(anglePrefab, startPos, Quaternion.Euler(0, 180 - 90 - anglePosition, 0),
                anglePrefabHolder.transform);
            angleObj.transform.position += ring.transform.position;
            //angleObj.transform.position += anglePrefabHolder.transform.position;
            
            angleObj.GetComponentInChildren<TextMeshProUGUI>().text = angleDelta.ToString("0") + "°";
            // if (i == _levelData.UnknownCargoId && _levelData.IsRotationAvailable)
            //     angleObj.GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;
        }
    }

    private void DrawLines()
    {
        ClearLines();
        foreach (var cargo in _cargos)
        {
            DrawLineToMediator(cargo.CargoMediator);
            DrawLineToCargo(cargo);
        }
    }

    private void CalculateGameState()
    {
        CalcPlatformAngle();
        PutAngles();
        
        DrawLines();
    }

    public void AddCargoMass(int value)
    {
        if (_gameState != GameState.Started)
            return;
        
        _cargosLeft--;
        _cargos[_levelData.UnknownCargoId].AdjustCargoMass(value);
        CalculateGameState();
    }

    private void Rotate(float delta)
    {
        _cargos[_levelData.UnknownCargoId].Rotate(delta, cargosMediatorHolder, cargosHolder);
        CalculateGameState();
    }

    public void RotateLeft()
    {
        Rotate(5f);
    }

    public void RotateRight()
    {
        Rotate(-5f);
    }
}
