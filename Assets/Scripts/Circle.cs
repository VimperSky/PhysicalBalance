using UnityEngine;

public class Circle : MonoBehaviour
{
    public int segments;
    public float xradius;
    public float yradius;
    LineRenderer _line;
       
    void Start ()
    {
        _line = gameObject.GetComponent<LineRenderer>();
       
        _line.SetVertexCount (segments + 1);
        _line.useWorldSpace = false;
        CreatePoints ();
    }
   
   
    void CreatePoints ()
    {
        float x;
        float z;

        var angle = -90f;
       
        for (int i = 0; i < segments + 1; i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;

            var targetPosition = new Vector3(x, 0f, z);
            _line.SetPosition (i, targetPosition);
                   
            angle += 180f / segments;
        }
    }
}