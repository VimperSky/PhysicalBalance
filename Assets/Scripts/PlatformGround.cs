using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    private bool _gameIsStarted;

    private Vector3 _ringStartPosition;
    
    private LevelData _levelData;
    
    void Start()
    {
        _levelData  = LevelDataKeeper.Instance.LevelData;
        _ringStartPosition = ring.transform.position;
        SpawnCargos(_levelData);
        CalcPlatformAngle();
        DrawLines();
        
        _gameIsStarted = true;
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
    

    private void DrawLine(Vector2 cargoPosition, float angleRad)
    {
        var lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineObj.transform.SetParent(linesHolder.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        
        lineRenderer.widthMultiplier = 0.1f;
        
        var startPos = new Vector3(RingRadius * Mathf.Cos(angleRad), 0f, RingRadius * Mathf.Sin(angleRad));
        startPos += ring.transform.position;
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, new Vector3(cargoPosition.x, _ringStartPosition.y, cargoPosition.y));
    }


    private static bool IsVictory(Vector2 force)
    {
        return Mathf.Abs(force.x) <= 0.05f && Mathf.Abs(force.y) <= 0.05f;
    }

    private void CalcPlatformAngle()
    {
        var resultForce = new Vector2();
        foreach (var cargo in _cargos)
        {
            resultForce += cargo.Force;
        }

        resultForce /= 50f;
        if (resultForce.x > 0.8f)
            resultForce.x = 0.8f;
        if (resultForce.y > 0.8)
            resultForce.y = 0.8f;
        
        var ringPos = _ringStartPosition;
        ring.transform.position = new Vector3(ringPos.x + resultForce.x, ringPos.y, ringPos.z + resultForce.y);
        
        if (!_gameIsStarted) 
            return;
        _gameIsStarted = false;

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
            DrawLine(cargo.Position, cargo.AngleRad);
            var centerPos = new Vector2(ring.transform.position.x, ring.transform.position.z);
            //var centerPos = Vector2.zero;
            
            var newAngle = Mathf.Atan2(cargo.Position.y - centerPos.y, cargo.Position.x - centerPos.x) * Mathf.Rad2Deg;
            if (newAngle < 0)
                newAngle += 360;
            //var newAngle = Vector2.Angle(cargo.Position - centerPos, Vector2.right);

            var normalAngle = cargo.AngleRad * Mathf.Rad2Deg;
            cargo.gameObject.transform.rotation = Quaternion.Euler(0, 90 - newAngle, 0);
        }
    }
    

    public void ChangeMass(float value)
    {
        if (!_gameIsStarted)
            return;
        
        _cargos[_levelData.UnknownCargoId].SetMass(value, true);
        
        CalcPlatformAngle();
        DrawLines();
    }
}
