using TMPro;
using UnityEngine;

public class CargoMediator: MonoBehaviour
{
    public Vector2 Position { get; private set; }
    public float AngleRad { get; private set; }
    

    public void SetData(Vector2 position, float angleRad)
    {
        Position = position;
        AngleRad = angleRad;
    }
    public void SetColor(Color color)
    {
        transform.Find("Canvas").Find("TextMass").gameObject.GetComponent<TextMeshProUGUI>().color = color;
    }

    public void SetDisplayedValue(float value, bool isUnknown)
    {
        transform.Find("Canvas").Find("TextMass").gameObject.GetComponent<TextMeshProUGUI>().text = !isUnknown ? value.ToString() : "?";
    }
}