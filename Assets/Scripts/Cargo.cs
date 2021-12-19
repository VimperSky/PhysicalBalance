using System;
using TMPro;
using UnityEngine;

public class Cargo: MonoBehaviour
{
    private Vector3 _originalScale;
    [SerializeField] private float mass;

    [SerializeField] private Vector2 position;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void SetData(float mass, Vector2 position)
    {
        SetMass(mass);
        
        this.position = position;
        transform.localScale = _originalScale * mass;
    }

    public void SetMass(float mass)
    {
        this.mass = mass;
        
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = mass.ToString();
    }

    public Vector2 Force => position * mass;

}