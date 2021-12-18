using UnityEngine;

public class Cargo: MonoBehaviour
{
    public float Degree { get; private set; }

    public float Mass { get; private set; }
    
    public Vector2 Position { get; private set; }

    public void SetData(float degree, float mass, Vector2 position)
    {
        Degree = degree;
        Mass = mass;
        Position = position;
    }

    public Vector2 Force => Position * Mass;

}