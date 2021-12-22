using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class PlatformGround : MonoBehaviour
{
    // Unity говно не умеет в массивы из сложных данных в инспекторе, поэтому такой костыль.
    [SerializeField] private int[] cargoAngles;

    [SerializeField] private int[] cargoMasses;
    
    [SerializeField] private GameObject cargosHolder;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private GameObject linesHolder;
    [SerializeField] private GameObject linePrefab;
    
    [SerializeField] private GameObject ring;
    
    private const float RingRadius = 1.25f;
    private const float CargoPlaceRadius = 4.5f;
    
    private readonly List<Cargo> _cargos = new();

    [SerializeField] private Button randomizeBtn;
    
    void Start()
    {
        randomizeBtn.onClick.AddListener(OnRandomizeBtnClick);
        
        SpawnCargos();
        CalcPlatformAngle();
        DrawLines();
    }

    private void OnRandomizeBtnClick()
    {
        foreach (var cargo in _cargos)
        {
            cargo.SetMass(Random.Range(1, 3) * 5);
        }
    }

    private void SpawnCargos()
    {
        if (cargoAngles.Length != cargoMasses.Length)
            throw new ArgumentException("CargoAngles length should be equal to CargoMasses");
        for (var i = 0; i < cargoAngles.Length; i++)
        {
            var angle = cargoAngles[i];
            var mass = cargoMasses[i];
            var angleRad = angle * Mathf.PI / 180f;
            var cargoPos = new Vector3(CargoPlaceRadius * Mathf.Sin(angleRad), 0f, CargoPlaceRadius * Mathf.Cos(angleRad));
            var cargo = Instantiate(cargoPrefab, cargoPos, Quaternion.Euler(0, angle, 0));
            cargo.transform.SetParent(cargosHolder.transform);
            var cargoScript = cargo.AddComponent<Cargo>();
            cargoScript.SetData(mass, new Vector2(cargoPos.x, cargoPos.z), angleRad);
            _cargos.Add(cargoScript);
        }
    }
    

    private void DrawLine(Vector2 cargoPosition, float angleRad)
    {
        var lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineObj.transform.SetParent(linesHolder.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
        lineRenderer.widthMultiplier = 0.2f;
        
        var startPos = new Vector3(RingRadius * Mathf.Sin(angleRad), 0f, RingRadius * Mathf.Cos(angleRad));
        startPos += ring.transform.position;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, new Vector3(cargoPosition.x, 0, cargoPosition.y));
    }



    private void CalcPlatformAngle()
    {
        var resultForce = new Vector2();
        foreach (var cargo in _cargos)
        {
            resultForce += cargo.Force;
        }

        // Чтобы не так сильно наклонялось, но пока не будем делать
        //var signVector = new Vector2(Math.Sign(resultForce.x), Math.Sign(resultForce.y));
        //var force = new Vector2(Mathf.Sqrt(Mathf.Abs(resultForce.x)), Mathf.Sqrt(Mathf.Abs(resultForce.y))) * signVector;
        
        //transform.rotation = Quaternion.Euler(_originalRotation.x + resultForce.y, _originalRotation.y, _originalRotation.z + resultForce.x);

        resultForce /= 50f;
        ring.transform.position = new Vector3(resultForce.x, ring.transform.position.y, resultForce.y);
        
        if (resultForce == Vector2.zero)
        {
            
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
            DrawLine(cargo.Position, cargo.AngleRad);
        }
    }
    

    private int _counter;
    void FixedUpdate()
    {
        // Это костыль, чтобы можно было в едиторе менять массу и смотреть как меняется равновесие.
        // Потом нужно заменить на что-то другое.
        _counter += 1;
        if (_counter % 30 != 0)
            return;
        
        CalcPlatformAngle();
        DrawLines();
    }
}
