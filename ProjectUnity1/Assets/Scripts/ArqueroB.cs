using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Arquero : UnidadJugador
{
    [Header("Puntos de disparo por dirección")]
    [SerializeField] private Transform disparoArriba;
    [SerializeField] private Transform disparoAbajo;
    [SerializeField] private Transform disparoLateral;
    [SerializeField] private Transform disparoDiagArriba;
    [SerializeField] private Transform disparoDiagAbajo;

    [Header("Combate")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private float radioDeteccion = 4.5f;
    [SerializeField] private float tiempoEntreDisparos = 1.5f;

    [SerializeField] private AudioSource fuenteArquero;
    [SerializeField] private AudioClip clipGolpear;
    [SerializeField] private AudioClip clipMorir;

    [SerializeField] private string tipoUnidad = "Arquero";

    private float tiempoUltimoDisparo = 0f;
    private Coroutine rutinaAtaque;
    private bool atacando;
    private Transform puntoDisparoActual;
    public bool EstaAtacando() => atacando;

    protected override void Start()
    {
        InicializarVida(80);
       
        ataque = 18;
        defensa = 4;
       
        base.Start();
    }

    public override void EjecutarAccion(GameObject objetivo, Vector3 destino)
    {
        StopAllCoroutines();
        ResetearAnimacionesDisparo();

        if (objetivo == null)
        {
            agent.SetDestination(destino);
            return;
        }

        // Si el objetivo es una oveja, los arqueros no reaccionan
        if (objetivo.TryGetComponent<Sheep>(out var oveja))
            return;

        // Si el objetivo es aliado, cancelar ataque
        if (objetivo.TryGetComponent<EntidadBase>(out var entidadObjetivo))
        {
            if (!FaccionUtils.SonEnemigos(faccion, entidadObjetivo.faccion))
            {
                Debug.Log("Objetivo aliado. Cancelando acción de ataque.");
                return;
            }

            // Detener movimiento para no interferir con ataque melee
            agent.SetDestination(transform.position);

            //  Iniciar evasión si el enemigo está muy cerca
            float distanciaPeligro = 2f;
            float distancia = Vector2.Distance(transform.position, objetivo.transform.position);

            if (distancia <= distanciaPeligro)
            {
                StartCoroutine(EvadirEnemigo(objetivo));
            }
        }

        // Iniciar rutina de disparo
        if (rutinaAtaque != null)
            StopCoroutine(rutinaAtaque);

        rutinaAtaque = StartCoroutine(CombatirObjetivo(objetivo));
    }
    private IEnumerator EvadirEnemigo(GameObject enemigo)
    {
        yield return null;

        Vector2 direccionContraria = (transform.position - enemigo.transform.position).normalized;
        Vector2 destinoFinal = (Vector2)transform.position + direccionContraria * 2f;

        // Validar que el punto está en NavMesh
        if (NavMesh.SamplePosition(destinoFinal, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Si no es válido, se queda en su lugar
            Debug.LogWarning("Evasión cancelada: punto no válido en NavMesh.");
            agent.SetDestination(transform.position);
        }

    }

    private IEnumerator CombatirObjetivo(GameObject objetivo)
    {
        if (objetivo == null) yield break;
        if (!objetivo.TryGetComponent<IAtacable>(out var atacable)) yield break;

        atacando = true;

        while (objetivo != null && atacable.EstaVivo())
        {
            float distancia = Vector2.Distance(transform.position, objetivo.transform.position);

            if (distancia > radioDeteccion)
                agent.SetDestination(objetivo.transform.position);
            else
            {
                agent.SetDestination(transform.position);

                if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
                {
                    if (fuenteArquero != null && clipGolpear != null)
                    {
                        ReproducirLoop(clipGolpear);
                    }
                    //  Dirección del enemigo al momento de disparar
                    Vector2 direccion = (objetivo.transform.position - transform.position).normalized;

                    // Reforzar giro visual de combate
                    if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y) &&
                        TryGetComponent<OrientadorVisual>(out var orientador))
                    {
                        orientador.ForzarGiroVisual(direccion.x >= 0);
                    }

                    puntoDisparoActual = ElegirPuntoDisparo(transform.position, objetivo.transform.position);
                    ActivarAnimacionDisparo(transform.position, objetivo.transform.position);

                    float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
                    StartCoroutine(DispararConRetardo(direccion, angle));

                    tiempoUltimoDisparo = Time.time;
                }

            }

            yield return null;
        }
        if (fuenteArquero != null && clipGolpear != null)
        {
            fuenteArquero.Stop();
        }
        atacando = false;
        ResetearAnimacionesDisparo();
        rutinaAtaque = null;
    }

    //private void Disparar(GameObject objetivo)
    //{
    //    puntoDisparoActual = ElegirPuntoDisparo(transform.position, objetivo.transform.position);
    //    ActivarAnimacionDisparo(transform.position, objetivo.transform.position);

    //    Vector2 direccion = (objetivo.transform.position - puntoDisparoActual.position).normalized;
    //    float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

    //    if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y) && TryGetComponent<OrientadorVisual>(out var orientador))
    //        orientador.ForzarGiroVisual(direccion.x >= 0);

    //    StartCoroutine(DispararConRetardo(direccion, angle));

    //}

    private IEnumerator DispararConRetardo(Vector2 direccion, float angle)
    {
        yield return new WaitForSeconds(0.2f);

        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparoActual.position, Quaternion.Euler(0, 0, angle));
        proyectil.GetComponent<Rigidbody2D>().velocity = direccion * 10f;

        if (proyectil.TryGetComponent<Flecha>(out var flecha))
        {
            
            flecha.SetDanio(ataque);
            flecha.SetEmisor(gameObject);
            Physics2D.IgnoreCollision(flecha.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    private Transform ElegirPuntoDisparo(Vector2 origen, Vector2 destino)
    {
        CalcularDireccionDisparo(origen, destino,
            out bool arriba, out bool abajo, out bool lateral, out bool diagonalArriba, out bool diagonalAbajo);

        if (arriba) return disparoArriba;
        if (abajo) return disparoAbajo;
        if (diagonalArriba) return disparoDiagArriba;
        if (diagonalAbajo) return disparoDiagAbajo;
        return disparoLateral;
    }

    private void ActivarAnimacionDisparo(Vector2 origen, Vector2 destino)
    {
        CalcularDireccionDisparo(origen, destino,
            out bool arriba, out bool abajo, out bool lateral, out bool diagonalArriba, out bool diagonalAbajo);

        ResetearAnimacionesDisparo();

        if (arriba) animator.SetBool("DisparoArriba", true);
        else if (abajo) animator.SetBool("DisparoAbajo", true);
        else if (diagonalArriba) animator.SetBool("DisparoDiagArriba", true);
        else if (diagonalAbajo) animator.SetBool("DisparoDiagAbajo", true);
        else if (lateral) animator.SetBool("DisparoLateral", true);

    }

    private void ResetearAnimacionesDisparo()
    {
        animator.SetBool("DisparoLateral", false);
        animator.SetBool("DisparoAbajo", false);
        animator.SetBool("DisparoArriba", false);
        animator.SetBool("DisparoDiagArriba", false);
        animator.SetBool("DisparoDiagAbajo", false);
    }

    public void CalcularDireccionDisparo(Vector2 origen, Vector2 destino,
        out bool arriba, out bool abajo, out bool lateral,
        out bool diagonalArriba, out bool diagonalAbajo)
    {
        Vector2 dir = (destino - origen).normalized;

        arriba = false;
        abajo = false;
        lateral = false;
        diagonalArriba = false;
        diagonalAbajo = false;

        float absX = Mathf.Abs(dir.x);
        float absY = Mathf.Abs(dir.y);

        if (absX > 0.5f && absY < 0.5f)
        {
            lateral = true;
        }
        else if (absX < 0.5f && dir.y > 0)
        {
            arriba = true;
        }
        else if (absX < 0.5f && dir.y < 0)
        {
            abajo = true;
        }
        else if (dir.y > 0)
        {
            diagonalArriba = true;
        }
        else
        {
            diagonalAbajo = true;
        }
    }

    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteArquero == null) return;
        fuenteArquero.clip = clip;
        fuenteArquero.loop = true;
        fuenteArquero.Play();
    }
    public void ReproducirUna(AudioClip clip, float pitch = 1f, float volumen = 1f)
    {
        if (fuenteArquero == null || clip == null) return;
        fuenteArquero.Stop();
        fuenteArquero.pitch = pitch;
        fuenteArquero.volume = volumen;
        fuenteArquero.clip = clip;
        fuenteArquero.loop = false;
        fuenteArquero.spatialBlend = 0.5f; // 2D sound
        fuenteArquero.PlayOneShot(clip);
    }
    protected override void Morir()
    {
        if (fuenteArquero != null && clipMorir != null)
        {
            ReproducirUna(clipMorir, 1f, 0.5f);
        }
        GestorEntidades.Instance?.Eliminar(tipoUnidad, gameObject);
        base.Morir();
    }

}