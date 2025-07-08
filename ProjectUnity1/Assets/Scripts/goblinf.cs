using System.Collections;
using UnityEngine;

public class Gobling : UnidadEnemigo, IAtacable
{
    [SerializeField] private Transform[] puntosPatrulla;
    private int indicePatrulla = 0;

    private float tiempoIdleEnPatrulla = 2f;
    private bool esperando = false;

    private Coroutine rutinaAtaque;
    private float tiempoEntreGolpes = 1.5f;
    private float tiempoUltimoGolpe = 0f;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        InicializarVida(100);
        ataque = 15;
        velocidad = 3.5f;


        if (puntosPatrulla.Length > 0)
            MoverHacia(puntosPatrulla[indicePatrulla].position);
    }
    public override void EjecutarAccion(GameObject objetivo, Vector3 destino)
    {
        if (objetivo != null && objetivo.TryGetComponent<Sheep>(out var oveja))     
            return;
        if (objetivo != null)
        {
            StopAllCoroutines();
            rutinaAtaque = StartCoroutine(CombatirObjetivo(objetivo));
        }
        else
        {
            MoverHacia(destino);
        }
    }

    protected override void EjecutarIA()
    {
        if (rutinaAtaque == null)
        {
            var objetivo = DetectarJugador();
            if (objetivo != null)
            {
                rutinaAtaque = StartCoroutine(CombatirObjetivo(objetivo));
            }
            else
            {
                Patrullar();
            }
        }
    }

       private void Patrullar()
    {
        if (esperando || agent.pathPending) return;

        if (agent.remainingDistance <= 0.5f)
        {
            esperando = true;
            animator.SetBool("IsMoving", false); // detener animación

            StartCoroutine(EsperarYPasarAlSiguientePunto());
        }
    }
    private IEnumerator EsperarYPasarAlSiguientePunto()
    {
        yield return new WaitForSeconds(tiempoIdleEnPatrulla);

        indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
        MoverHacia(puntosPatrulla[indicePatrulla].position);
        esperando = false;
    }


    private GameObject DetectarJugador()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, 6f);
        foreach (var col in objetos)
        {
            if (col.TryGetComponent<UnidadJugador>(out var unidadJugador))
            {
                if (FaccionUtils.SonEnemigos(faccion, unidadJugador.faccion))
                    return unidadJugador.gameObject;
            }
        }
        return null;
    }


    private IEnumerator CombatirObjetivo(GameObject objetivo)
    {
       
        if (objetivo == null) yield break;
        if (!objetivo.TryGetComponent<IAtacable>(out var atacable)) yield break;
        if (!objetivo.TryGetComponent<EntidadBase>(out var entidadObjetivo)) yield break;
       

        if (!FaccionUtils.SonEnemigos(faccion, entidadObjetivo.faccion)) yield break;

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
                DireccionAtaque direccion = CalcularDireccion(transform.position, objetivo.transform.position);
                ActualizarAnimacionAtaque(direccion);
                var orientador = GetComponent<OrientadorVisual>();
                if (orientador != null)
                    orientador.GirarVisual(objetivo.transform.position);

                if (Time.time >= tiempoUltimoGolpe + tiempoEntreGolpes)
                {
                    ActualizarAnimacionAtaque(direccion);
                    if (TryGetComponent<OrientadorVisual>(out var orientadorVisualGolpe))
                        orientadorVisualGolpe.GirarVisual(objetivo.transform.position);

                    yield return new WaitForSeconds(0.25f);
                    atacable.RecibirDanio(ataque, gameObject);
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
        animator.SetBool("AtacarLateral", false);
        animator.SetBool("AtacarArriba", false);
        animator.SetBool("AtacarAbajo", false);

        switch (direccion)
        {
            case DireccionAtaque.Arriba:
                animator.SetBool("AtacarArriba", true);
                break;
            case DireccionAtaque.Abajo:
                animator.SetBool("AtacarAbajo", true);
                break;
            default:
                animator.SetBool("AtacarLateral", true);
                break;
        }
    }

    private void ResetearAnimacionesAtaque()
    {
        animator.SetBool("AtacarLateral", false);
        animator.SetBool("AtacarArriba", false);
        animator.SetBool("AtacarAbajo", false);
    }

    public enum DireccionAtaque { Izquierda, Derecha, Arriba, Abajo }

    private DireccionAtaque CalcularDireccion(Vector2 origen, Vector2 destino)
    {
        Vector2 dir = destino - origen;
        return Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
            ? DireccionAtaque.Izquierda // solo usamos lateral, sin distinguir
            : (dir.y >= 0 ? DireccionAtaque.Arriba : DireccionAtaque.Abajo);
    }
}

/*    protected override void Start()
    {
        InicializarVida(60);
        ataque = 8;
        defensa = 3;
        velocidad = 3;
        base.Start();
        

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
        
        ActualizarVidaVisual();
        if (vida <= 0)
        {
            // Podés poner animación de muerte o efectos visuales aquí
            Destroy(gameObject);
        }
    }

}*/