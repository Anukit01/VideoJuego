using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Gobling : UnidadEnemigo, IAtacable
{
    [SerializeField]
    private Transform[] puntosPatrulla = new Transform[0];
    private int indicePatrulla = 0;
    [SerializeField] private string tipoUnidad = "Gobling";


    private float tiempoIdleEnPatrulla = 2f;
    private bool esperando = false;

    private Coroutine rutinaAtaque;
    private float tiempoEntreGolpes = 1.5f;
    private float tiempoUltimoGolpe = 0f;

    [SerializeField] private AudioSource fuenteGobling;
    [SerializeField] private AudioClip clipGolpear;
    [SerializeField] private AudioClip clipRuido;
    [SerializeField] private AudioClip clipMorir;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        InicializarVida(80);
        ataque = 10;
        defensa = 3;

        GestorEntidades.Instance?.Registrar(tipoUnidad, gameObject);
        GestorEnemigos.Instance?.RegistrarEnemigo();

        if (puntosPatrulla != null && puntosPatrulla.Length > 0)
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
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
            yield break;

        yield return new WaitForSeconds(tiempoIdleEnPatrulla);

        if (puntosPatrulla != null)
        {

        indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
            Vector3 destino = puntosPatrulla[indicePatrulla].position;

            MoverHacia(puntosPatrulla[indicePatrulla].position);
        esperando = false;

            if (TryGetComponent<OrientadorVisual>(out var orientador))
                orientador.GirarPorDireccion((destino - transform.position).normalized);

        }

    }


    private GameObject DetectarJugador()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, 6f);
        foreach (var col in objetos)
        {
            if (col.TryGetComponent<UnidadJugador>(out var unidadJugador))
            {
                if (FaccionUtils.SonEnemigos(faccion, unidadJugador.faccion))
                {
                    ReproducirUna(clipRuido);
                    return unidadJugador.gameObject;               
                }
            }

            if (col.TryGetComponent<EdificioBase>(out var entidadBase))
            {
                if (FaccionUtils.SonEnemigos(faccion, entidadBase.faccion))
                {

                    ReproducirUna(clipRuido);
                    return entidadBase.gameObject;
                }
            }
         
        }

        return null; //solo si no encontró ningún objetivo válido
    }


    private IEnumerator CombatirObjetivo(GameObject objetivo)
    {
        if (objetivo == null) yield break;
        if (!objetivo.TryGetComponent<IAtacable>(out var atacable)) yield break;
        
        if (!objetivo.TryGetComponent<EntidadBase>(out var entidadObjetivo)) yield break;
        if (objetivo.TryGetComponent<Aldeano>(out var aldeano) && aldeano.EstaOcupadoPrivado)
            yield break; // no lo ataques si está ocupado

        if (!FaccionUtils.SonEnemigos(faccion, entidadObjetivo.faccion)) yield break;
       

        float distanciaAtaque = 2f;
        float margen = 0.3f;
        float stoppingOriginal = agent.stoppingDistance;
        animator.SetBool("IsMoving", true);
        agent.stoppingDistance = distanciaAtaque * 0.5f;
        agent.SetDestination(objetivo.transform.position);

        yield return new WaitUntil(() =>
            !agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <= distanciaAtaque + margen);

        agent.SetDestination(transform.position);
        animator.SetBool("IsMoving", false);
        if (fuenteGobling != null)
        {
            ReproducirLoop(clipGolpear);

        }
        while (objetivo != null && atacable.EstaVivo())
        {
            float distanciaActual = Vector2.Distance(transform.position, objetivo.transform.position);
            if (distanciaActual > distanciaAtaque + margen)
            {
                agent.SetDestination(objetivo.transform.position);

                yield return new WaitUntil(() =>
                    !agent.pathPending &&
                    agent.hasPath &&
                    agent.remainingDistance <= distanciaAtaque + margen);

                agent.SetDestination(transform.position);
                animator.SetBool("IsMoving", false);
            }

            //  Calcular dirección del objetivo
            DireccionAtaque direccion = CalcularDireccion(transform.position, objetivo.transform.position);

            // Girar sprite lateral si corresponde
            if (TryGetComponent<OrientadorVisual>(out var orientador))
            {
                if (direccion == DireccionAtaque.Izquierda)
                    orientador.ForzarGiroVisual(false);
                else if (direccion == DireccionAtaque.Derecha)
                    orientador.ForzarGiroVisual(true);
            }
            
            //  Activar animación
            ResetearAnimacionesAtaque();
            ActualizarAnimacionAtaque(direccion);

            //  Aplicar daño si corresponde
            if (Time.time >= tiempoUltimoGolpe + tiempoEntreGolpes)
            {
                
                yield return new WaitForSeconds(0.2f);
                atacable.RecibirDanio(ataque, gameObject);
                tiempoUltimoGolpe = Time.time;
            }

            yield return null;
        }
        if (fuenteGobling != null && fuenteGobling.isPlaying)
            fuenteGobling.Stop();
        ResetearAnimacionesAtaque();
        agent.stoppingDistance = stoppingOriginal;
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

    protected override void Morir()
    {
        if (fuenteGobling != null && clipMorir != null)
        {
            ReproducirUna(clipMorir);
        }
        GestorEntidades.Instance?.Eliminar(tipoUnidad, gameObject);
        GestorEnemigos.Instance?.NotificarMuerte();

        base.Morir();
    }
    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteGobling == null) return;
        fuenteGobling.clip = clip;
        fuenteGobling.loop = true;
        fuenteGobling.Play();
    }
    public void ReproducirUna(AudioClip clip)
    {
        if (fuenteGobling == null || clip == null) return;
        fuenteGobling.Stop();
        fuenteGobling.clip = clip;
        fuenteGobling.loop = false;
        fuenteGobling.spatialBlend = 0.5f; // 2D sound
        fuenteGobling.PlayOneShot(clip);
    }
}
