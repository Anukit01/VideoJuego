using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Caballero : UnidadJugador
{
    private Coroutine rutinaAtaque;
    private float tiempoEntreGolpes = 1.2f;
    private float tiempoUltimoGolpe = 0f;

    protected override void Start()
    {
        InicializarVida(150);
        ataque = 20;
        defensa = 10;
        velocidad = 5;
        base.Start();
    }


    public override void EjecutarAccion(GameObject objetivo, Vector3 destino)
    {
        StopAllCoroutines();
        ResetearAnimacionesAtaque();

        if (objetivo == null)
        {
            agent.SetDestination(destino);
            return;
        }

        if (objetivo.TryGetComponent<EntidadBase>(out var entidadObjetivo))
        {
            if (!FaccionUtils.SonEnemigos(faccion, entidadObjetivo.faccion))
            {
                Debug.Log("Objetivo aliado. Cancelando acción de ataque.");
                return;
            }
        }

        if (rutinaAtaque != null)
            StopCoroutine(rutinaAtaque);

        rutinaAtaque = StartCoroutine(CombatirObjetivo(objetivo));
    }

    private IEnumerator CombatirObjetivo(GameObject objetivo)
    {
        if (objetivo == null) yield break;
        if (!objetivo.TryGetComponent<IAtacable>(out var atacable)) yield break;
        if (!objetivo.TryGetComponent<EntidadBase>(out var entidadObjetivo)) yield break;

        if (!FaccionUtils.SonEnemigos(faccion, entidadObjetivo.faccion))
        {
            Debug.Log("El objetivo no es enemigo. Cancelando combate.");
            yield break;
        }

        while (objetivo != null && atacable.EstaVivo())
        {
            float distancia = Vector2.Distance(transform.position, objetivo.transform.position);

            if (distancia > 1.5f)
            {
                agent.SetDestination(objetivo.transform.position);
            }
            else
            {
                agent.SetDestination(transform.position);

                var direccion = CalcularDireccion(transform.position, objetivo.transform.position);
                ActualizarAnimacionAtaque(direccion);

                // Solución para CS0136: Renombrar la variable local para evitar conflictos  
                OrientadorVisual orientadorVisual = GetComponent<OrientadorVisual>();
                if (orientadorVisual != null)
                    orientadorVisual.GirarVisual(objetivo.transform.position);

                if (Time.time >= tiempoUltimoGolpe + tiempoEntreGolpes)
                {
                    ActualizarAnimacionAtaque(direccion);

                    // Solución para UNT0026: Usar TryGetComponent para evitar asignaciones innecesarias  
                    if (TryGetComponent<OrientadorVisual>(out var orientadorVisualGolpe))
                        orientadorVisualGolpe.GirarVisual(objetivo.transform.position);

                    // Espera el "impacto" de la animación antes de aplicar el daño  
                    yield return new WaitForSeconds(0.25f); // Ajusta según la animación  

                    atacable.RecibirDanio(ataque);
                    tiempoUltimoGolpe = Time.time;
                    ResetearAnimacionesAtaque();
                }
            }

            yield return null;
        }

        ResetearAnimacionesAtaque();
        rutinaAtaque = null;
    }

    private void ActualizarAnimacionAtaque(DireccionAtaque direccion)
    {
        switch (direccion)
        {
            case DireccionAtaque.Izquierda:
            case DireccionAtaque.Derecha:
                animator.SetBool("AtacarLateral", true);
                break;
            case DireccionAtaque.Arriba:
                animator.SetBool("AtacarArriba", true);
                break;
            case DireccionAtaque.Abajo:
                animator.SetBool("AtacarAbajo", true);
                break;
        }
    }

    private void ResetearAnimacionesAtaque()
    {
        animator.SetBool("AtacarLateral", false);
        animator.SetBool("AtacarArriba", false);
        animator.SetBool("AtacarAbajo", false);
    }

    public enum DireccionAtaque
    {
        Izquierda,
        Derecha,
        Arriba,
        Abajo
    }

    public DireccionAtaque CalcularDireccion(Vector2 origen, Vector2 destino)
    {
        Vector2 dir = destino - origen;
        return Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
            ? (dir.x >= 0 ? DireccionAtaque.Derecha : DireccionAtaque.Izquierda)
            : (dir.y >= 0 ? DireccionAtaque.Arriba : DireccionAtaque.Abajo);
    }
    
}