using UnityEngine;

public class OrientadorVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Gira el sprite hacia la posición objetivo (horizontalmente).
    /// </summary>
    public void GirarVisual(Vector3 referencia)
    {
        if (spriteRenderer == null) return;

        float escalaX = referencia.x >= transform.position.x ? 1f : -1f;
        transform.localScale = new Vector3(escalaX, 1f, 1f);
    }

    /// <summary>
    /// Gira el sprite según la dirección (para NavMesh o flecha).
    /// </summary>
    public void GirarPorDireccion(Vector2 direccion)
    {
        if (spriteRenderer == null) return;
        if (Mathf.Abs(direccion.x) < 0.01f) return; // sin movimiento horizontal

        float escalaX = direccion.x >= 0 ? 1f : -1f;
        transform.localScale = new Vector3(escalaX, 1f, 1f);
    }

}