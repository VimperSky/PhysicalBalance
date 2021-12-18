using UnityEngine;

public class Cargo: MonoBehaviour
{
    [SerializeField] private float mass;

    [SerializeField] private Vector2 position;

    public void SetData(float mass, Vector2 position)
    {
        this.mass = mass;
        this.position = position;
    }

    public Vector2 Force => position * mass;

}