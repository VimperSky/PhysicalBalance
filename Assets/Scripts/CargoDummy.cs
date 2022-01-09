using TMPro;
using UnityEngine;

public class CargoDummy: MonoBehaviour
{
    public float Mass { get; private set; }
    
    public void SetMass(float mass)
    {
        Mass = mass;

        transform.Find("Canvas").Find("TextMass").gameObject.GetComponent<TextMeshProUGUI>().text = Mass.ToString();
    }

}