using UnityEngine;

public class OrdenarPorY : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (sr != null)
            sr.sortingOrder = -(int)(transform.position.y * 100);
    }
}
