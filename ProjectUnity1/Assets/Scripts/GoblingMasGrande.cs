using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GoblingMasGrande : UnidadEnemigo, IAtacable
{
    [SerializeField, Tooltip("Opcional. Si se asignan puntos de patrulla, la unidad patrullará.")]
    private Transform[] puntosPatrulla = new Transform[0];
    private int indicePatrulla = 0;

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
        InicializarVida(200);
        ataque = 30;
        velocidad = 3f;


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
                {
                    ReproducirUna(clipRuido, 1f, 0.5f);
                    return unidadJugador.gameObject;
                }
            }

            if (col.TryGetComponent<EdificioBase>(out var entidadBase))
            {
                if (FaccionUtils.SonEnemigos(faccion, entidadBase.faccion))
                {
                    ReproducirUna(clipRuido, 1f, 0.5f);
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

        float distanciaAtaque = 1.5f;
        float margen = 0.3f;
        float stoppingOriginal = agent.stoppingDistance;

        agent.stoppingDistance = distanciaAtaque * 0.5f;
        agent.SetDestination(objetivo.transform.position);

        yield return new WaitUntil(() =>
            !agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <= distanciaAtaque + margen);

        //  Detener movimiento
        agent.SetDestination(transform.position);
        animator.SetBool("IsMoving", false);

                if (fuenteGobling != null)
                {
                    ReproducirLoop(clipGolpear);

                }
        while (objetivo != null && atacable.EstaVivo())
        {
            DireccionAtaque direccion = CalcularDireccion(transform.position, objetivo.transform.position);

            ResetearAnimacionesAtaque();
            ActualizarAnimacionAtaque(direccion);

            if (TryGetComponent<OrientadorVisual>(out var orientador))
                orientador.GirarVisual(objetivo.transform.position);

            if (Time.time >= tiempoUltimoGolpe + tiempoEntreGolpes)
            {
                yield return new WaitForSeconds(0.2f); // Delay para sincronizar animación
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
    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteGobling == null) return;
        fuenteGobling.clip = clip;
        fuenteGobling.loop = true;
        fuenteGobling.Play();
    }
    public void ReproducirUna(AudioClip clip, float pitch = 1f, float volumen = 1f)
    {
        if (fuenteGobling == null || clip == null) return;
        fuenteGobling.Stop();
        fuenteGobling.pitch = pitch;
        fuenteGobling.volume = volumen;
        fuenteGobling.clip = clip;
        fuenteGobling.loop = false;
        fuenteGobling.spatialBlend = 0.5f; // 2D sound
        fuenteGobling.PlayOneShot(clip);
    }
    protected override void Morir()
    {
        if (fuenteGobling != null && clipMorir != null)
        {
            ReproducirUna(clipMorir, 1f, 0.5f);
        }
        base.Morir();
    }
}
