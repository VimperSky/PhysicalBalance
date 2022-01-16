using UnityEngine;

public class CargoDummy: MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(15f, 0, 15f) * Time.deltaTime);
    }
}