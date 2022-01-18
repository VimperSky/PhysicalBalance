using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    public CargoMediator CargoMediator { get; private set; }
    
    private int _totalMass;

    private float _lastMassY;

    private static readonly List<int> PossibleVariants = new() { 5, 3, 2, 1 };
    
    public const float PlaceRadius = 5.05f;
    
    public float Angle { get; private set; }

    public int TotalMass => _totalMass;
    private List<int> SplitMass(int mass)
    {
        var variants = new List<int>();
        var massRest = mass;

        while (massRest > 0)
        {
            foreach (var variant in PossibleVariants.Where(variant => massRest >= variant))
            {
                variants.Add(variant);
                massRest -= variant;
                break;
            }
        }

        return variants;
    }
    
    public void SetData(float angle, int totalMassValue, CargoMediator cargoMediator)
    {
        Angle = angle;
        CargoMediator = cargoMediator;
        
        var cargoMasses = SplitMass(totalMassValue);

        _lastMassY = transform.localPosition.y;
        foreach (var mass in cargoMasses)
        {
            AddCargoMass(mass);
        }
        
        CargoMediator.SetDisplayedValue(_totalMass, _totalMass == 0);
    }

    public void Rotate(float rotationDelta, GameObject cargosMediatorHolder, GameObject cargosHolder)
    {
        Angle += rotationDelta;
        
        var angleRad = Angle * Mathf.Deg2Rad;
        var cargoBasePos = new Vector3( PlaceRadius * Mathf.Cos(angleRad), 0f, PlaceRadius * Mathf.Sin(angleRad));
        var mediatorPos = cargoBasePos + cargosMediatorHolder.transform.position;

        CargoMediator.transform.transform.position = mediatorPos;
        CargoMediator.transform.transform.rotation = Quaternion.Euler(0, 90 - Angle, 0);
        CargoMediator.SetData(new Vector2(cargoBasePos.x, cargoBasePos.z), angleRad);
        
        var cargoPos = cargoBasePos + cargosHolder.transform.position;
        transform.position = cargoPos;
        transform.rotation = Quaternion.Euler(0, 90 - Angle, 0);
    }

    public void AdjustCargoMass(int cargoMassValue)
    {
        AddCargoMass(cargoMassValue);
        CargoMediator.SetDisplayedValue(_totalMass, false);
    }
    
    private void AddCargoMass(int cargoMassValue)
    {
        var newObj = Instantiate(Resources.Load($"Prefabs/CargoMass/{cargoMassValue}"), transform) as GameObject;
        var newPosition = new Vector3(0f,  _lastMassY + newObj.transform.localScale.y / 50f, 0f);
        newObj.transform.localPosition = newPosition;
        _lastMassY = newObj.transform.localPosition.y + newObj.transform.localScale.y / 50f;
        _totalMass += cargoMassValue;
    }

    public void SetColor(Color color)
    {
        CargoMediator.SetColor(color);
    }

    

    public Vector2 Force => CargoMediator.Position * _totalMass;

}