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
        if (GetComponent<Arquero>()?.EstaAtacando() == true)
            return;
        Transform visualPrincipal = spriteRenderer != null ? spriteRenderer.transform : transform;

        float escalaX = referencia.x >= visualPrincipal.position.x ? 1f : -1f;

        foreach (Transform hijo in transform)
        {
            if (hijo.CompareTag("Vida"))
                continue;

            Vector3 escala = hijo.localScale;
            escala.x = Mathf.Abs(escala.x) * escalaX;
            hijo.localScale = escala;
        }
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

        public void ForzarGiroVisual(bool mirarDerecha)
    {
        Transform visualPrincipal = spriteRenderer != null ? spriteRenderer.transform : transform;

        Vector3 escala = visualPrincipal.localScale;
        escala.x = mirarDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        visualPrincipal.localScale = escala;
    }

}