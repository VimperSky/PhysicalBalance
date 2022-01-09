using UnityEngine;

public class CargoMediator: MonoBehaviour
{
    [SerializeField]
    private float mass;

    public Vector2 Position { get; private set; }
    public float AngleRad { get; private set; }
    

    public void SetData(Vector2 position, float angleRad)
    {
        Position = position;
        AngleRad = angleRad;
    }
    
    public Vector2 Force => Position * mass;

}