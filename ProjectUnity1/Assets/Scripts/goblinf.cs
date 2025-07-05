using UnityEngine;

public class Gobling : UnidadEnemigo, IAtacable
{
    [SerializeField] private float rangoPersecucion = 5f;

    protected override void Start()
    {
        base.Start();
        vida = 60;
        ataque = 8;
        defensa = 3;
        velocidad = 3;
        Debug.Log($"{name} tiene facción: {faccion}");

    }

    protected override void EjecutarIA()
    {
        GameObject objetivo = DetectarUnidadJugador();
        if (objetivo != null)
        {
            objetivoActual = objetivo.transform;
            MoverHacia(objetivoActual.position);
        }
        else
        {
            objetivoActual = null;
            // Podés agregar lógica de patrullaje si no hay objetivos
        }
    }

    private GameObject DetectarUnidadJugador()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, rangoPersecucion);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<UnidadJugador>(out var unidad))
                return unidad.gameObject;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoPersecucion);
    }
    public override void RecibirDanio(int cantidad)
    {
        vida -= cantidad;
        Debug.Log($"{name} recibió {cantidad} de daño. Vida restante: {vida}");

        if (vida <= 0)
        {
            // Podés poner animación de muerte o efectos visuales aquí
            Destroy(gameObject);
        }
    }

}