using UnityEngine;

public class Cargo: MonoBehaviour
{
    private float _degree;
    private float _mass;

    public float Degree => _degree;

    public void SetData(float degree, float mass)
    {
        _degree = degree;
        _mass = mass;
    }
}