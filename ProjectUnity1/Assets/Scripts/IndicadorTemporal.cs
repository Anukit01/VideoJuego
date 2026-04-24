using UnityEngine;

public class IndicadorTemporal : MonoBehaviour
{
    public float lifetime = 1f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}