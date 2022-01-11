using TMPro;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    public CargoMediator CargoMediator { get; private set; }
    
    [SerializeField]
    private float mass;
    
    public void SetData(float mass, CargoMediator cargoMediator)
    {
        CargoMediator = cargoMediator;

        SetMass(mass);
    }

    public void SetMass(float mass)
    {
        this.mass = mass;

        CargoMediator.SetDisplayedValue(mass, mass == 0f);
        //transform.Find("Canvas").Find("TextMass").gameObject.GetComponent<TextMeshProUGUI>().text = !isUnknown ? this.mass.ToString() : "?";
    }

    public void SetColor(Color color)
    {
        CargoMediator.SetColor(color);
    }
    

    public Vector2 Force => CargoMediator.Position * mass;

}