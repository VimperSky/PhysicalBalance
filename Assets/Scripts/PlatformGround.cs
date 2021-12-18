using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGround : MonoBehaviour
{
    // Unity говно не умеет в массивы из сложных данных в инспекторе, поэтому такой костыль.
    [SerializeField] private int[] cargoAngles;

    [SerializeField] private int[] cargoMasses;
    
    [SerializeField] private GameObject cargosHolder;
    [SerializeField] private GameObject cargoPrefab;

    private const float Radius = 4.5f;
    private readonly List<Cargo> _cargos = new();

    private Vector3 _originalRotation;
    void Start()
    {
        _originalRotation = transform.rotation.eulerAngles;
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
            cargo.transform.localScale *= mass;
            var cargoScript = cargo.AddComponent<Cargo>();
            cargoScript.SetData(mass, new Vector2(cargoPos.x, cargoPos.z));
            _cargos.Add(cargoScript);
        }
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

    private int aboba;
    void FixedUpdate()
    {
        aboba += 1;
        if (aboba % 30 != 0)
            return;
        
        CalcPlatformAngle();
    }
}
