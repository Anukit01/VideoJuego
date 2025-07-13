using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Aldeano : UnidadJugador
{

    public enum EstadoAldeano { Idle, Atacando, Talando, Recolectando, VolverAEntregar, Construyendo }
    public EstadoAldeano estadoActual = EstadoAldeano.Idle;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject visualMadera;
    [SerializeField] private GameObject visualCarne;
    [SerializeField] private GameObject visualBolsaOro;

    [SerializeField] private AudioSource fuenteAldeano;
    [SerializeField] private AudioClip clipTalado;
    [SerializeField] private AudioClip clipConstruyo;
    [SerializeField] private AudioClip clipMinando;

    [SerializeField] private GameObject barraVida;
    public GameObject VisualBolsaOro => visualBolsaOro;
    public bool EsAtacable => !EstaOcupadoPrivado;
    public bool EstaOcupadoPrivado { get; private set; }

    public RecoleccionTemporal CargaRecoleccion = new();
    private IRecolectable ultimoRecurso;
    private Sheep ovejaActual;

    protected override void Start()
    {
        InicializarVida(70);
      
        ataque = 5;
        defensa = 5;
        velocidad = 2;
        EstaOcupadoPrivado = false;
        base.Start();

    }

    public override void EjecutarAccion(GameObject objetivo, Vector3 destino)
    {
        StopAllCoroutines();
        ResetearEstado();

        if (objetivo == null)
        {
            agent.SetDestination(destino);
            return;
        }

        if (objetivo.TryGetComponent<Sheep>(out var oveja))
        {
            AtacarOveja(oveja);
            SeleccionadorDeUnidad.Instance.DeseleccionarTodas();
            return;
        }

        if (objetivo.TryGetComponent<Oro>(out var mina))
        {
            StartCoroutine(RecolectarOroRutina(mina));
            SeleccionadorDeUnidad.Instance.DeseleccionarTodas();
            return;
        }

        if (objetivo.TryGetComponent<IRecolectable>(out var recurso))
        {
            CargaRecoleccion.tipo = recurso.Tipo;
            CargaRecoleccion.visual = ObtenerVisualPara(recurso.Tipo);
            ultimoRecurso = recurso;

            StartCoroutine(MoverYRecolectar(recurso, recurso.PuntoDeRecoleccion));
            SeleccionadorDeUnidad.Instance.DeseleccionarTodas();
            return;
        }

        if (objetivo.TryGetComponent<EdificioBase>(out var edificioPendiente))
        {
            Debug.Log("Edificio pendiente de construcción: " + edificioPendiente.name);
            if (edificioPendiente.EstáConstruido)
            {
                Debug.LogWarning("El edificio ya está construido.");
                return;
            }
            if (FaccionUtils.SonEnemigos(faccion, edificioPendiente.faccion))
            {
                Debug.Log("Son Enemigos");
                return;
            }
            if (!edificioPendiente.EstáConstruido && edificioPendiente.VidaActual < edificioPendiente.VidaMaxima)
            {
                StopAllCoroutines();
                StartCoroutine(ContinuarConstruccionExistente(objetivo));
                return;
            }
        }


        if (objetivo.TryGetComponent<IAtacable>(out var atacable) &&
            objetivo.TryGetComponent<EntidadBase>(out var entidad) &&
            FaccionUtils.SonEnemigos(faccion, entidad.faccion))
        {
            StartCoroutine(CombatirEntidadRutina(objetivo, atacable));
        }
    }

    private GameObject ObtenerVisualPara(TipoRecurso tipo) => tipo switch
    {
        TipoRecurso.Madera => visualMadera,
        TipoRecurso.Alimento => visualCarne,
        TipoRecurso.Oro => visualBolsaOro,
        _ => null
    };

    private void ResetearEstado()
    {
        estadoActual = EstadoAldeano.Idle;
        animator.SetBool("Talar", false);
        animator.SetBool("Construir", false);
        animator.SetBool("ManosOcupadas", false);
        CargaRecoleccion?.Vaciar();
        if (CargaRecoleccion?.visual != null)
            CargaRecoleccion.visual.SetActive(false);
        ultimoRecurso = null;
        ovejaActual = null;
    }

    private IEnumerator MoverYRecolectar(IRecolectable recurso, Transform punto)
    {
        agent.SetDestination(punto.position);
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        estadoActual = EstadoAldeano.Talando;
        animator.SetBool("Talar", true);
        if (fuenteAldeano != null)
        {
            ReproducirLoop(clipTalado);

        }

        yield return StartCoroutine(recurso.EjecutarRecoleccion(this));

        if (fuenteAldeano != null && fuenteAldeano.isPlaying)
            fuenteAldeano.Stop();

        animator.SetBool("Talar", false);
        estadoActual = EstadoAldeano.Idle;

    }

    public void TerminarRecoleccion()
    {
        animator.SetBool("Talar", false);

        if (CargaRecoleccion.EstaLleno && CargaRecoleccion.visual != null)
            CargaRecoleccion.visual.SetActive(true);

        Vector3 destino = BuscarPuntoDeEntrega(CargaRecoleccion.tipo);
        agent.SetDestination(destino);
        animator.SetBool("ManosOcupadas", true);
        estadoActual = EstadoAldeano.VolverAEntregar;

        StartCoroutine(DepositarRecoleccion());
    }

    private Vector3 BuscarPuntoDeEntrega(TipoRecurso tipo)
    {
        var puntos = GameObject.FindObjectsOfType<PuntoDeEntrega>();
        foreach (var p in puntos)
        {
            if (!p.enabled) continue; // 👈 Salteamos puntos inactivos
            if (p.tipoAceptado == tipo)
                return p.transform.position;
        }

        return transform.position;
    }


    private IEnumerator DepositarRecoleccion()
    {
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        int cantidad = CargaRecoleccion.cantidad;
        switch (CargaRecoleccion.tipo)
        {
            case TipoRecurso.Madera: GestionRecrsos.Instance.SumarMadera(cantidad); break;
            case TipoRecurso.Alimento: GestionRecrsos.Instance.SumarAlimento(cantidad); break;
            case TipoRecurso.Oro: GestionRecrsos.Instance.SumarOro(cantidad); break;
        }

        if (CargaRecoleccion.visual != null)
            CargaRecoleccion.visual.SetActive(false);

        CargaRecoleccion.Vaciar();
        estadoActual = EstadoAldeano.Idle;
        animator.SetBool("ManosOcupadas", false);

        if (ultimoRecurso != null && PuedeVolverARecolectar(ultimoRecurso))
        {
            if (ultimoRecurso is Oro oro)
                StartCoroutine(RecolectarOroRutina(oro));
            else
                StartCoroutine(MoverYRecolectar(ultimoRecurso, ultimoRecurso.PuntoDeRecoleccion));
        }
    }

    private bool PuedeVolverARecolectar(IRecolectable recurso)
    {
        return recurso switch
        {
            Tree tree => tree.maderaDisponible > 0,
            Oro oro => oro.cantidad > 0,
            _ => false
        };
    }

    private void AtacarOveja(Sheep oveja)
    {
        ResetearEstado();
        ovejaActual = oveja;
        estadoActual = EstadoAldeano.Atacando;

        agent.SetDestination(oveja.transform.position);
        StartCoroutine(AtacarOvejaRutina());
    }
    private IEnumerator CombatirEntidadRutina(GameObject objetivo, IAtacable atacable)
    {
        estadoActual = EstadoAldeano.Atacando;

        float distanciaAtaque = 2f;
        float tolerancia = 0.35f;
        float stoppingOriginal = agent.stoppingDistance;
        agent.stoppingDistance = distanciaAtaque * 0.5f;
        agent.SetDestination(objetivo.transform.position);

        yield return new WaitUntil(() =>
            !agent.pathPending && agent.hasPath &&
            agent.remainingDistance <= distanciaAtaque + tolerancia);

        agent.SetDestination(transform.position);
      
        
        while (objetivo != null && atacable != null && atacable.EstaVivo())
        {
            Vector2 direccion = objetivo.transform.position - transform.position;


            if (TryGetComponent<OrientadorVisual>(out var orientador))
                orientador.GirarVisual(objetivo.transform.position);

            if (!animator.GetBool("Talar"))
                animator.SetBool("Talar", true);
            

            yield return new WaitForSeconds(0.2f); // impacto
            atacable.RecibirDanio(ataque, gameObject);
            
            yield return new WaitForSeconds(0.15f);
        }

        animator.SetBool("Talar", false);
        estadoActual = EstadoAldeano.Idle;
        agent.stoppingDistance = stoppingOriginal;
    }

    private IEnumerator AtacarOvejaRutina()
    {
        float distanciaAtaque = 2f;
        float tolerancia = 0.3f;
        float stoppingOriginal = agent.stoppingDistance;
        agent.stoppingDistance = distanciaAtaque * 0.5f;

        // Primer destino: oveja
        agent.SetDestination(ovejaActual.transform.position);

        yield return new WaitUntil(() =>
            ovejaActual != null &&
            !agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <= distanciaAtaque + tolerancia);

        yield return new WaitUntil(() => agent.velocity.magnitude <= 0.1f);

        // Bucle de combate y seguimiento
        while (ovejaActual != null && ovejaActual.vida > 0)
        {
            // Si la oveja se aleja, volver a perseguir
            if (Vector2.Distance(transform.position, ovejaActual.transform.position) > distanciaAtaque + tolerancia ||
                agent.velocity.magnitude > 0.1f)
            {
                agent.SetDestination(ovejaActual.transform.position);
                yield return new WaitUntil(() =>
                    Vector2.Distance(transform.position, ovejaActual.transform.position) <= distanciaAtaque + tolerancia &&
                    agent.velocity.magnitude <= 0.1f);
            }

            //  Orientar visual
            var orientador = GetComponent<OrientadorVisual>();
            if (orientador != null)
                orientador.GirarVisual(ovejaActual.transform.position);

            // Animar y aplicar daño
            animator.SetBool("Talar", true);
            yield return new WaitForSeconds(0.25f);
            ovejaActual.RecibirDanio(ataque, gameObject);

            animator.SetBool("Talar", false);

            yield return new WaitUntil(() => ovejaActual == null || !ovejaActual.EnHuida);

            if (ovejaActual == null || ovejaActual.vida <= 0)
                break;

            yield return new WaitForSeconds(1f);
        }

        //  Finalizar
        estadoActual = EstadoAldeano.Idle;
        agent.stoppingDistance = stoppingOriginal;

        if (ovejaActual == null)
        {
            GameObject carne = GameObject.FindObjectsOfType<Carne>()
                .FirstOrDefault(c => Vector2.Distance(transform.position, c.transform.position) < 2f)?.gameObject;

            if (carne != null)
                EjecutarAccion(carne, carne.transform.position);
        }
    }
    private IEnumerator RecolectarOroRutina(Oro mina)
    {
        estadoActual = EstadoAldeano.Recolectando;
        agent.SetDestination(mina.PuntoDeRecoleccion.position);

        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);


        spriteRenderer.enabled = false;
        mina.MostrarVisualActiva();
        barraVida.SetActive(false); 

        if (fuenteAldeano != null && clipMinando != null)
        {
            ReproducirLoop(clipMinando);
        }
      
        EstaOcupadoPrivado = true;
      
        yield return new WaitForSeconds(8f);
       
        EstaOcupadoPrivado = false;
        barraVida.SetActive(true);

        mina.Recolectar(this);

        if (fuenteAldeano != null)
            fuenteAldeano.Stop();

        if (mina.cantidad > 0)
            mina.RevertirVisualActiva();
        else
            mina.MostrarVisualDestruida();

        ultimoRecurso = mina;
        animator.SetBool("ManosOcupadas", true);
        spriteRenderer.enabled = true;
        visualBolsaOro.SetActive(true);

        Vector3 destino = BuscarPuntoDeEntrega(TipoRecurso.Oro);
        agent.SetDestination(destino);
        StartCoroutine(DepositarRecoleccion());
        estadoActual = EstadoAldeano.VolverAEntregar;
    }

    public void OrdenarConstruccion(GameObject prefab, Vector2 posicion)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefab nulo en construcción.");
            return;
        }

        StartCoroutine(Construir(prefab, posicion));
    }

    private IEnumerator Construir(GameObject prefab, Vector2 posicion)
    {
        agent.SetDestination(posicion);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, posicion) <= 0.5f);

        if (prefab.TryGetComponent<IBuilding>(out var construible))
        {
            foreach (var costo in construible.Costos)
                if (!GestionRecrsos.Instance.TieneRecurso(costo.nombreRecurso, costo.cantidad))
                    yield break;
        }

        GameObject edificio = Instantiate(prefab, posicion, Quaternion.identity);
        BuildingPlacementManager.Instance?.ActualizarNavMesh();
        yield return new WaitForSeconds(0.05f);
        FindObjectOfType<GestorOrdenVisualGlobal>()?.OrdenarTodoElMundo();

        if (edificio.TryGetComponent<IBuilding>(out var nuevoEdificio))
        {
            foreach (var costo in nuevoEdificio.Costos)
                GestionRecrsos.Instance.GastarRecurso(costo.nombreRecurso, costo.cantidad);
            Transform zona = edificio.transform.Find("ZonaConstruccion");
            float radioConstruccion = 1.5f;
            if (zona.TryGetComponent<CircleCollider2D>(out var collider))
            {
                radioConstruccion = collider.radius * zona.lossyScale.x; // escalado real
            }

            yield return new WaitUntil(() =>
                Vector2.Distance(transform.position, zona.position) <= radioConstruccion &&
                agent.velocity.magnitude <= 0.1f);


                var orientador = GetComponent<OrientadorVisual>();
                Transform centroVisual = edificio.transform.Find("visualConstruccion");
                if (centroVisual != null)
                    orientador.GirarVisual(centroVisual.position);
                else
                    orientador.GirarVisual(edificio.transform.position);                
                yield return new WaitForSeconds(0.4f);
                // Reemplazo 'objetivo.transform.position' con 'punto.position'  
            }
            nuevoEdificio.BeginConstruction(); // activa visual de construcción
           
            estadoActual = EstadoAldeano.Construyendo;
        animator.SetBool("Construir", true);

        if (fuenteAldeano != null && clipConstruyo != null)
        {
           ReproducirLoop(clipConstruyo);
        }




        float tiempoEntreConstrucciones = 1.2f;
            int cantidadConstruccion = 5;
            if (edificio.TryGetComponent<EdificioBase>(out var nuevoEdificio1))
            {
                while (nuevoEdificio1.VidaActual < nuevoEdificio1.VidaMaxima)
                {
                    // Si el aldeano fue interrumpido
                    if (estadoActual != EstadoAldeano.Construyendo)
                        yield break;

                    nuevoEdificio1.SumarVida(cantidadConstruccion);

                    yield return new WaitForSeconds(tiempoEntreConstrucciones);
                       
                }
            
            animator.SetBool("Construir", false); // finalizar animación
            if (fuenteAldeano != null && fuenteAldeano.isPlaying)
                fuenteAldeano.Stop();
        }
            
           

        }
    private IEnumerator ContinuarConstruccionExistente(GameObject edificioExistente)
    {
        if (!edificioExistente.TryGetComponent<EdificioBase>(out var nuevoEdificio))
            yield break;

        Transform zona = edificioExistente.transform.Find("ZonaConstruccion");
        float radioConstruccion = 1.5f;

        if (zona != null && zona.TryGetComponent<CircleCollider2D>(out var collider))
            radioConstruccion = collider.radius * zona.lossyScale.x;

        agent.SetDestination(zona != null ? zona.position : edificioExistente.transform.position);
        yield return new WaitUntil(() =>
            Vector2.Distance(transform.position, zona.position) <= radioConstruccion &&
            agent.velocity.magnitude <= 0.1f);

        var orientador = GetComponent<OrientadorVisual>();
        Transform centroVisual = edificioExistente.transform.Find("visualConstruccion");

        if (orientador != null)
        {
            if (centroVisual != null)
                orientador.GirarVisual(centroVisual.position);
            else
                orientador.GirarVisual(edificioExistente.transform.position);
        }

        yield return new WaitForSeconds(0.4f);

        nuevoEdificio.BeginConstruction();
        animator.SetBool("Construir", true);
        estadoActual = EstadoAldeano.Construyendo;
        if (fuenteAldeano != null && clipConstruyo != null)
        {
            ReproducirLoop(clipConstruyo);
        }

        float tiempoEntreConstrucciones = 1.2f;
        int cantidadConstruccion = 5;

        while (nuevoEdificio.VidaActual < nuevoEdificio.VidaMaxima)
        {
            if (estadoActual != EstadoAldeano.Construyendo)
                yield break;

            nuevoEdificio.SumarVida(cantidadConstruccion);
            yield return new WaitForSeconds(tiempoEntreConstrucciones);
        }
        if (fuenteAldeano != null && fuenteAldeano.isPlaying)
            fuenteAldeano.Stop();
        animator.SetBool("Construir", false); // finalizar animación
        estadoActual = EstadoAldeano.Idle;
    }

    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteAldeano == null) return;
        fuenteAldeano.clip = clip;
        fuenteAldeano.loop = true;
        fuenteAldeano.Play();
    }


}

