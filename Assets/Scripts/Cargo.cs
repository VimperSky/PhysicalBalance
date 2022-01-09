using TMPro;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    public CargoMediator CargoMediator { get; private set; }
    
    [SerializeField]
    private float mass;
    
    public void SetData(float mass, CargoMediator cargoMediator, bool isUnknown)
    {
        CargoMediator = cargoMediator;

        SetMass(mass, isUnknown);
    }

    public void SetMass(float mass, bool isUnknown)
    {
        this.mass = mass;

        CargoMediator.SetDisplayedValue(mass, isUnknown);
        //transform.Find("Canvas").Find("TextMass").gameObject.GetComponent<TextMeshProUGUI>().text = !isUnknown ? this.mass.ToString() : "?";
    }
    

    public Vector2 Force => CargoMediator.Position * mass;

}