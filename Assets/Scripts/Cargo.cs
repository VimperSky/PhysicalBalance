using System;
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
        this.mass = mass;
        this.position = position;
        transform.localScale = _originalScale * mass;
    }

    public Vector2 Force => position * mass;

}