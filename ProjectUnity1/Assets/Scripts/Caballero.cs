using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Caballero : UnidadJugador
{
    private Coroutine rutinaAtaque;
    private float tiempoEntreGolpes = 1.2f;
    private float tiempoUltimoGolpe = 0f;

    [SerializeField] private AudioSource fuenteCaballero;
    [SerializeField] private AudioClip clipGolpear;
    [SerializeField] private AudioClip clipMorir;

    private bool atacando = false;

    public bool EstaAtacando() => atacando;
    [SerializeField] private float radioDeteccion = 4f;

    protected override void Start()
    {
        InicializarVida(150);
        
        ataque = 20;
        defensa = 10;
        velocidad = 5;
        base.Start();
           
        rutinaAutodefensa = StartCoroutine(RutinaAutodefensa());
    
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
        if (objetivo != null && objetivo.TryGetComponent<Sheep>(out var oveja))       
            return;
        

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

        Debug.Log($"Clic recibido sobre: {objetivo?.name}, IAtacable: {objetivo?.GetComponent<IAtacable>() != null}");


        rutinaAtaque = StartCoroutine(CombatirObjetivo(objetivo));
    }

    private IEnumerator CombatirObjetivo(GameObject objetivo)
    {
        if (objetivo == null || !objetivo.TryGetComponent<IAtacable>(out var atacable)) yield break;
        if (!objetivo.TryGetComponent<EntidadBase>(out var entidad)) yield break;
        if (!FaccionUtils.SonEnemigos(faccion, entidad.faccion)) yield break;

        float distanciaAtaque = 2f;
        float margen = 0.35f;
        float originalStopping = agent.stoppingDistance;

        agent.stoppingDistance = distanciaAtaque * 0.5f;
        agent.SetDestination(objetivo.transform.position);

        yield return new WaitUntil(() =>
            !agent.pathPending && agent.hasPath &&
            agent.remainingDistance <= distanciaAtaque + margen);

        agent.SetDestination(transform.position); // Detener en posición actual

        while (objetivo != null && atacable.EstaVivo())
        {
            var direccion = CalcularDireccion(transform.position, objetivo.transform.position);

            if (TryGetComponent<OrientadorVisual>(out var orientador))
            {
                switch (direccion)
                {
                    case DireccionAtaque.Derecha:
                        orientador.ForzarGiroVisual(true);
                        break;
                    case DireccionAtaque.Izquierda:
                        orientador.ForzarGiroVisual(false);
                        break;
                }
            }

            ResetearAnimacionesAtaque();
            ActualizarAnimacionAtaque(direccion);
            atacando = true;


            if (Time.time >= tiempoUltimoGolpe + tiempoEntreGolpes)
            {
                if (fuenteCaballero != null && clipGolpear != null)
                {
                   ReproducirLoop(clipGolpear);
                }
                yield return new WaitForSeconds(0.15f); // Tiempo ajustado al impacto

                atacable.RecibirDanio(ataque, gameObject);
                tiempoUltimoGolpe = Time.time;
            }

            yield return null;
        }
        if (fuenteCaballero != null && clipGolpear != null)
        {
            fuenteCaballero.Stop();
        }
        atacando = false;

        ResetearAnimacionesAtaque();
        agent.stoppingDistance = originalStopping;
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


    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteCaballero == null) return;
        fuenteCaballero.clip = clip;
        fuenteCaballero.loop = true;
        fuenteCaballero.Play();
    }
    public void ReproducirUna(AudioClip clip)
    {
        if (fuenteCaballero == null || clip == null) return;
        fuenteCaballero.Stop();
        fuenteCaballero.clip = clip;
        fuenteCaballero.loop = false;
        fuenteCaballero.spatialBlend = 0.5f; // 2D sound
        fuenteCaballero.PlayOneShot(clip);
    }

    protected override void Morir()
    {
        if (fuenteCaballero != null && clipMorir != null)
        {
            ReproducirUna(clipMorir);
        }
        base.Morir();
    }

    private Coroutine rutinaAutodefensa;

    private IEnumerator RutinaAutodefensa()
    {
        const float intervaloEscaneo = 0.5f;

        while (true)
        {
            // Si ya está atacando, no escanea
            if (!atacando)
            {
                GameObject enemigo = DetectarEnemigoCercano();
                if (enemigo != null)
                {
                    EjecutarAccion(enemigo, transform.position); // o CombatirObjetivo(enemigo)
                }
            }

            yield return new WaitForSeconds(intervaloEscaneo);
        }
    }
    private GameObject DetectarEnemigoCercano()
    {
        Collider2D[] detectados = Physics2D.OverlapCircleAll(transform.position, radioDeteccion);

        foreach (var col in detectados)
        {
            if (col.TryGetComponent<EntidadBase>(out var entidad))
            {
                if (FaccionUtils.SonEnemigos(faccion, entidad.faccion) &&
                    entidad.TryGetComponent<IAtacable>(out _) &&
                    entidad.gameObject != gameObject) // evita atacarse a sí mismo
                {
                    return entidad.gameObject;
                }
            }
        }

        return null;
    }

}