using System;
using TMPro;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    private Vector3 _originalScale;
    
    [SerializeField]
    private float mass;

    public Vector2 Position { get; private set; }
    public float AngleRad { get; private set; }
    
    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void SetData(float mass, Vector2 position, float angleRad, bool isUnknown)
    {
        SetMass(mass, isUnknown);
        
        Position = position;
        AngleRad = angleRad;
        transform.localScale = _originalScale * 5;
        // transform.localScale = _originalScale * mass;
    }

    public void SetMass(float mass, bool isUnknown)
    {
        this.mass = mass;

        transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = !isUnknown ? this.mass.ToString() : "?";
    }
    

    public Vector2 Force => Position * mass;

}