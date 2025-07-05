using System.Collections;
using System.Collections.Generic;
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
    public GameObject VisualBolsaOro => visualBolsaOro;

    public RecoleccionTemporal CargaRecoleccion = new();
    private IRecolectable ultimoRecurso;
    private Sheep ovejaActual;

    protected override void Start()
    {
        base.Start();

        vida = 70;
        ataque = 5;
        defensa = 5;
        velocidad = 2;
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
        }
        else if (objetivo.TryGetComponent<Oro>(out var mina))
        {
            StartCoroutine(RecolectarOroRutina(mina));
        }
        else if (objetivo.TryGetComponent<IRecolectable>(out var recurso))
        {
            CargaRecoleccion.tipo = recurso.Tipo;
            CargaRecoleccion.visual = ObtenerVisualPara(recurso.Tipo);
            ultimoRecurso = recurso;

            StartCoroutine(MoverYRecolectar(recurso, recurso.PuntoDeRecoleccion));
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
        yield return StartCoroutine(recurso.EjecutarRecoleccion(this));
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
            if (p.tipoAceptado == tipo)
                return p.transform.position;

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

    private IEnumerator AtacarOvejaRutina()
    {
        yield return new WaitUntil(() => ovejaActual != null && Vector2.Distance(transform.position, ovejaActual.transform.position) <= 1f);

        while (ovejaActual != null && ovejaActual.vida > 0)
        {
            MirarHacia(ovejaActual.transform.position);
            animator.SetBool("Talar", true);
            yield return new WaitForSeconds(0.1f);
            ovejaActual.RecibirDanio(ataque);

            yield return new WaitUntil(() => ovejaActual == null || !ovejaActual.EnHuida);

            if (ovejaActual == null || ovejaActual.vida <= 0)
                break;

            yield return new WaitForSeconds(1f);
        }

        animator.SetBool("Talar", false);
        estadoActual = EstadoAldeano.Idle;
    }

    private void MirarHacia(Vector3 objetivo)
    {
        float direccion = objetivo.x - transform.position.x;
        transform.localScale = new Vector3(direccion >= 0 ? 1 : -1, 1, 1);
    }

    private IEnumerator RecolectarOroRutina(Oro mina)
    {
        estadoActual = EstadoAldeano.Recolectando;
        agent.SetDestination(mina.PuntoDeRecoleccion.position);

        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        spriteRenderer.enabled = false;
        mina.MostrarVisualActiva();

        yield return new WaitForSeconds(8f);

        mina.Recolectar(this);
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

            Transform punto = nuevoEdificio.GetPuntoConstruccion();
            if (punto != null)
            {
                agent.SetDestination(punto.position);
                yield return new WaitUntil(() => Vector2.Distance(transform.position, punto.position) <= 0.5f);
                MirarHacia(edificio.transform.position);
            }

            animator.SetBool("Construir", true);
            estadoActual = EstadoAldeano.Construyendo;

            yield return StartCoroutine(nuevoEdificio.ProcesoConstruccion(() =>
            {
                animator.SetBool("Construir", false);
                estadoActual = EstadoAldeano.Idle;
            }));
        }
    }
}

