using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GoblingTNT : UnidadEnemigo, IAtacable
{
    
    [Header("Patrullaje")]
    [SerializeField] private Transform[] puntosPatrulla = new Transform[0];
    private int indicePatrulla = 0;
    private float tiempoIdleEnPatrulla = 2f;
    private bool esperando = false;
    
    [Header("Combate a distancia")]
    [SerializeField] private Transform puntoLanzamiento;
    [SerializeField] private GameObject dinamitaPrefab;
    [SerializeField] private float radioDeteccion = 3.5f;
    [SerializeField] private float tiempoEntreLanzamientos = 2f;
    private float tiempoUltimoLanzamiento = 0f;
    private Coroutine rutinaAtaque;

    [Header("Audio")]
    [SerializeField] private AudioSource fuenteAudio;
    [SerializeField] private AudioClip clipLanzar;
    [SerializeField] private AudioClip clipRuido;
    [SerializeField] private AudioClip clipMorir;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        InicializarVida(80);
        ataque = 15;
        defensa = 2;

        GestorEnemigos.Instance?.RegistrarEnemigo();

        if (puntosPatrulla.Length > 0)
            MoverHacia(puntosPatrulla[indicePatrulla].position);
    }

    public override void EjecutarAccion(GameObject objetivo, Vector3 destino)
    {
        StopAllCoroutines();

        if (objetivo != null && objetivo.TryGetComponent<Sheep>(out var oveja))
            return;

        if (objetivo != null)
        {
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
            animator.SetBool("IsMoving", false);
            StartCoroutine(EsperarYPasarAlSiguientePunto());
        }
    }

    private IEnumerator EsperarYPasarAlSiguientePunto()
    {
        

        yield return new WaitForSeconds(tiempoIdleEnPatrulla);
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
            yield break;
        indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
        
        Vector3 destino = puntosPatrulla[indicePatrulla].position;
               MoverHacia(destino);
        esperando = false;

        if (TryGetComponent<OrientadorVisual>(out var orientador))
            orientador.GirarPorDireccion((destino - transform.position).normalized);
    }

    private GameObject DetectarJugador()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, radioDeteccion);
        foreach (var col in objetos)
        {
            if (col.TryGetComponent<UnidadJugador>(out var unidadJugador))
            {
                if (FaccionUtils.SonEnemigos(faccion, unidadJugador.faccion))
                {
                    if (unidadJugador.TryGetComponent<Aldeano>(out var aldeano) && aldeano.EstaOcupadoPrivado)
                        return null;

                    ReproducirUna(clipRuido);
                    return unidadJugador.gameObject;
                }
            }

            if (col.TryGetComponent<EdificioBase>(out var edificio) &&
                FaccionUtils.SonEnemigos(faccion, edificio.faccion))
            {
                ReproducirUna(clipRuido);
                return edificio.gameObject;
            }
        }

        return null;
    }

    private IEnumerator CombatirObjetivo(GameObject objetivo)
    {
        if (objetivo == null || !objetivo.TryGetComponent<IAtacable>(out var atacable)) yield break;

        agent.SetDestination(transform.position);
        animator.SetBool("IsMoving", false);

        while (objetivo != null && atacable.EstaVivo())
        {
            float distancia = Vector2.Distance(transform.position, objetivo.transform.position);

            if (distancia > radioDeteccion)
            {
                agent.SetDestination(objetivo.transform.position);
                animator.SetBool("IsMoving", true);
            }
            else
            {
                agent.SetDestination(transform.position);
                animator.SetBool("IsMoving", false);

                if (Time.time >= tiempoUltimoLanzamiento + tiempoEntreLanzamientos)
                {
                    animator.SetTrigger("Lanzar");

                    if (fuenteAudio != null && clipLanzar != null)
                        ReproducirLoop(clipLanzar);

                    Vector2 direccion = (objetivo.transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

                    StartCoroutine(LanzarConRetardo(direccion, angle));

                    tiempoUltimoLanzamiento = Time.time;

                    

                }
            }

            yield return null;
        }

        if (fuenteAudio != null && fuenteAudio.isPlaying)
            fuenteAudio.Stop();

        rutinaAtaque = null;
    }

    private IEnumerator LanzarConRetardo(Vector2 direccion, float angle)
    {
        yield return new WaitForSeconds(0.3f);

        GameObject dinamita = Instantiate(dinamitaPrefab, puntoLanzamiento.position, Quaternion.Euler(0, 0, angle));
        dinamita.GetComponent<Rigidbody2D>().velocity = direccion * 8f;

        if (dinamita.TryGetComponent<Dinamita>(out var scriptDinamita))
        {
            scriptDinamita.SetDanio(ataque);
            scriptDinamita.SetEmisor(gameObject);
        }
    }

    protected override void Morir()
    {
        if (fuenteAudio != null && clipMorir != null)
            ReproducirUna(clipMorir);

        GestorEnemigos.Instance?.NotificarMuerte();
        base.Morir();
    }

    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteAudio == null) return;
        fuenteAudio.clip = clip;
        fuenteAudio.loop = true;
        fuenteAudio.Play();
    }

    public void ReproducirUna(AudioClip clip)
    {
        if (fuenteAudio == null || clip == null) return;
        fuenteAudio.Stop();
        fuenteAudio.clip = clip;
        fuenteAudio.loop = false;
        fuenteAudio.spatialBlend = 0.5f;
        fuenteAudio.PlayOneShot(clip);
    }
}