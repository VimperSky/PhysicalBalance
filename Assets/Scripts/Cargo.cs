using TMPro;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    public CargoMediator CargoMediator { get; private set; }
    
    [SerializeField]
    private float mass;
    
    public void SetData(float mass, CargoMediator cargoMediator, bool isUnknown)
    {
        SetMass(mass, isUnknown);
        
        CargoMediator = cargoMediator;
    }

    public void SetMass(float mass, bool isUnknown)
    {
        this.mass = mass;

        transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = !isUnknown ? this.mass.ToString() : "?";
    }
    

    public Vector2 Force => CargoMediator.Position * mass;

}