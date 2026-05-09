using UnityEngine;

public class EscalaInmuneAlFlip : MonoBehaviour
{
    private Vector3 escalaInicial;

    void Start()
    {
        escalaInicial = transform.localScale;
    }

    void LateUpdate()
    {
        // Siempre preserva la escala original
        Vector3 padreScale = transform.parent.localScale;
        float signoX = Mathf.Sign(padreScale.x);

        transform.localScale = new Vector3(escalaInicial.x * signoX, escalaInicial.y, escalaInicial.z);
    }
}