using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    public CargoMediator CargoMediator { get; private set; }
    
    [SerializeField]
    private int totalMass;

    private float _lastMassY;

    private static readonly List<int> PossibleVariants = new() { 5, 2, 1 };
    
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
        
        SetMass(totalMassValue);
    }

    public void SetMass(int totalMassValue)
    {
        totalMass = totalMassValue;

        var cargoMasses = SplitMass(totalMassValue);

        _lastMassY = transform.position.y;
        foreach (var mass in cargoMasses)
        {
            AddCargoMass(mass);
        }
        
        CargoMediator.SetDisplayedValue(totalMass, totalMass == 0);
    }

    private void AddCargoMass(int cargoMassValue)
    {
        var newObj = Instantiate(Resources.Load($"Prefabs/CargoMass{cargoMassValue}"), transform) as GameObject;
        var newPosition = new Vector3(transform.position.x,  _lastMassY + newObj.transform.localScale.y / 14f, transform.position.z);
        newObj.transform.position = newPosition;
        _lastMassY = newObj.transform.position.y + newObj.transform.localScale.y / 14f;
    }

    public void SetColor(Color color)
    {
        CargoMediator.SetColor(color);
    }
    

    public Vector2 Force => CargoMediator.Position * totalMass;

}