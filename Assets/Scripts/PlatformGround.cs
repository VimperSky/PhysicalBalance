using System.Collections.Generic;
using UnityEngine;

public class PlatformGround : MonoBehaviour
{
    [SerializeField] 
    private List<float> cargoDegrees;

    [SerializeField] private GameObject cargosHolder;
    [SerializeField] private GameObject cargoPrefab;

    private const float Radius = 4.5f;
    void Start()
    {
        SpawnCargos();
        
    }

    private void SpawnCargos()
    {
        foreach (var angle in cargoDegrees)
        {
            var angleRad = angle * Mathf.PI / 180f;
            var cargoPos = new Vector3(Radius * Mathf.Sin(angleRad), 0f, Radius * Mathf.Cos(angleRad));
            var cargo = Instantiate(cargoPrefab, cargoPos, Quaternion.Euler(0, angle, 0));
            cargo.transform.SetParent(cargosHolder.transform);
            var cargoScript = cargo.AddComponent<Cargo>();
            cargoScript.SetData(angle, 100);
        }
    }

    void Update()
    {
        
    }
}
