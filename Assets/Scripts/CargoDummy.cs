using TMPro;
using UnityEngine;

public class CargoDummy: MonoBehaviour
{
    public float Mass { get; private set; }
    
    public void SetMass(float mass)
    {
        Mass = mass;

        transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = Mass.ToString();
    }

}