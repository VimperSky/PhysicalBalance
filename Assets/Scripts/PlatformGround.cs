using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlatformGround : MonoBehaviour
{
    // Unity говно не умеет в массивы из сложных данных в инспекторе, поэтому такой костыль.
    [SerializeField] private int[] cargoAngles;

    [SerializeField] private int[] cargoMasses;
    
    [SerializeField] private GameObject cargosHolder;
    [SerializeField] private GameObject cargoPrefab;

    private const float Radius = 4.5f;
    private readonly List<Cargo> _cargos = new();
    public LineRenderer Line1;
    public LineRenderer Line2;
    public LineRenderer Line3;

    private Vector3 _originalRotation;
    void Start()
    {
        _originalRotation = transform.rotation.eulerAngles;
        InitLines();
        SpawnCargos();
        CalcPlatformAngle();
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
            var cargoPos = new Vector3(Radius * Mathf.Sin(angleRad), 0f, Radius * Mathf.Cos(angleRad));
            var cargo = Instantiate(cargoPrefab, cargoPos, Quaternion.Euler(0, angle, 0));
            cargo.transform.SetParent(cargosHolder.transform);
            var cargoScript = cargo.AddComponent<Cargo>();

            GameObject textMassObj = cargo.transform.GetChild(0).GetChild(0).gameObject;
            textMassObj.GetComponent<TextMeshProUGUI>().text = mass.ToString();

            if (i == 0)
            {
                DrawLine(Line1, cargoPos);
            }
            else if (i == 1)
            {
                DrawLine(Line2, cargoPos);
            }
            else
            {
                DrawLine(Line3, cargoPos);
            }    

            cargoScript.SetData(mass, new Vector2(cargoPos.x, cargoPos.z));
            _cargos.Add(cargoScript);
        }
    }

    private void InitLines() 
    {
        Line1.startWidth = 0.05f;
        Line1.endWidth = 0.05f;
        Line1.positionCount = 2;

        Line2.startWidth = 0.05f;
        Line2.endWidth = 0.05f;
        Line2.positionCount = 2;

        Line3.startWidth = 0.05f;
        Line3.endWidth = 0.05f;
        Line3.positionCount = 2;
    }

    private void DrawLine(LineRenderer currentLine, Vector3 cargoPosition)
    {
        currentLine.SetPosition(0, new Vector3(0f, 0f, 0f));
        currentLine.SetPosition(1, cargoPosition);
        Debug.Log(currentLine.name);
        Debug.Log(cargoPosition);
        Debug.Log(cargoPosition);
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
        
        transform.rotation = Quaternion.Euler(_originalRotation.x + resultForce.y, _originalRotation.y, _originalRotation.z + resultForce.x);
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
    }
}
