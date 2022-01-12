using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    public CargoMediator CargoMediator { get; private set; }
    
    private int _totalMass;

    private float _lastMassY;

    private static readonly List<int> PossibleVariants = new() { 5, 2, 1 };


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
    
    public void SetData(int totalMassValue, CargoMediator cargoMediator)
    {
        CargoMediator = cargoMediator;
        
        var cargoMasses = SplitMass(totalMassValue);

        _lastMassY = transform.position.y;
        foreach (var mass in cargoMasses)
        {
            AddCargoMass(mass);
        }
        
        CargoMediator.SetDisplayedValue(_totalMass, _totalMass == 0);
    }

    public void AdjustCargoMass(int cargoMassValue)
    {
        AddCargoMass(cargoMassValue);
        CargoMediator.SetDisplayedValue(_totalMass, false);
    }
    
    private void AddCargoMass(int cargoMassValue)
    {
        var newObj = Instantiate(Resources.Load($"Prefabs/CargoMass/{cargoMassValue}"), transform) as GameObject;
        var newPosition = new Vector3(transform.position.x,  _lastMassY + newObj.transform.localScale.y / 14f, transform.position.z);
        newObj.transform.position = newPosition;
        _lastMassY = newObj.transform.position.y + newObj.transform.localScale.y / 14f;
        _totalMass += cargoMassValue;
    }

    public void SetColor(Color color)
    {
        CargoMediator.SetColor(color);
    }
    

    public Vector2 Force => CargoMediator.Position * _totalMass;

}