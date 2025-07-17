using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeleccionadorDeUnidad : MonoBehaviour
{
    public static SeleccionadorDeUnidad Instance { get; private set; }

    public List<GameObject> unidadesSeleccionadas = new();
    public List<GameObject> todasLasUnidades = new();
    [SerializeField] private GameObject canvasConstruccion;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        todasLasUnidades.Clear();
    }

    private void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
            ManejarClickIzquierdo();

        if (Input.GetMouseButtonDown(1) && unidadesSeleccionadas.Count > 0)
            ManejarClickDerecho();

        ActualizarCanvasDeConstruccion();
    }

    private void ManejarClickIzquierdo()
    {
        if (BuildingPlacementManager.Instance != null && BuildingPlacementManager.Instance.IsPlacing())
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
       
        
        if (hit != null && hit.GetComponent<UnidadJugador>() is UnidadJugador unidad)
        {
            if (unidad.TryGetComponent<Aldeano>(out var aldeano) && aldeano.EstaOcupadoPrivado)
                return; // no seleccionarlo


            if (Input.GetKey(KeyCode.LeftShift))
            {
                AlternarSeleccion(unidad.gameObject);
            }
            else
            {
                if (unidadesSeleccionadas.Contains(unidad.gameObject))
                    DeseleccionarTodas();
                else
                    SeleccionarSolo(unidad.gameObject);
            }
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
                DeseleccionarTodas();
        }

        if (!SelectorPorArrastreActivo() && hit == null)
            DeseleccionarTodas();
    }

    private bool SelectorPorArrastreActivo()
    {
        var arrastrador = FindObjectOfType<SelectorPorArrastre>();
        return arrastrador != null && arrastrador.EstaArrastrando;
    }

    private void ManejarClickDerecho()
    {
        Vector2 destino = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(destino);

        GameObject objetivo = null;

        foreach (var col in colliders)
        {
            GameObject golpeado = col.gameObject;
            if (golpeado.GetComponent<Flecha>() != null) continue;
            if (golpeado.CompareTag("IgnorarClick")) continue;

            if (golpeado.GetComponent<IRecolectable>() != null || golpeado.GetComponent<IAtacable>() != null)
            {
                objetivo = golpeado;
                break;
            }
        }

        foreach (var unidad in unidadesSeleccionadas.ToList())
        {
            if (unidad.TryGetComponent<Aldeano>(out var aldeano) && aldeano.EstaOcupadoPrivado)
                continue; // Ignorar si está ocupado

            if (unidad.TryGetComponent<IAccionContextual>(out var accionable))
            {
                if (objetivo != null && objetivo.TryGetComponent<IRecolectable>(out var recurso))
                    accionable.EjecutarAccion(objetivo, recurso.PuntoDeRecoleccion.position);
                else if (objetivo != null && objetivo.TryGetComponent<IAtacable>(out var atacable))
                    accionable.EjecutarAccion(objetivo, destino);              
                else
                    accionable.EjecutarAccion(null, destino);                
            }
            else if (unidad.TryGetComponent<Movimiento>(out var movimiento))
            {
                movimiento.MoverA(destino);
            }
        }
    }

    public void AlternarSeleccion(GameObject unidad)
    {
        bool agregar = !unidadesSeleccionadas.Contains(unidad);
        CambiarSeleccion(unidad, agregar);

        if (agregar)
            unidadesSeleccionadas.Add(unidad);
        else
            unidadesSeleccionadas.Remove(unidad);
    }

    public void SeleccionarSolo(GameObject unidad)
    {
        DeseleccionarTodas();
        unidadesSeleccionadas.Add(unidad);
        CambiarSeleccion(unidad, true);
    }

    public void DeseleccionarTodas()
    {
        foreach (var unidad in unidadesSeleccionadas)
            CambiarSeleccion(unidad, false);

        unidadesSeleccionadas.Clear();
    }

    private void CambiarSeleccion(GameObject unidad, bool estaSeleccionada)
    {
        if (unidad.TryGetComponent<Movimiento>(out var movimiento))
            movimiento.MostrarIndicadorSeleccion(estaSeleccionada);
    }

    public void SeleccionDrag(GameObject unidad)
    {
        if (!unidadesSeleccionadas.Contains(unidad))
        {
            unidadesSeleccionadas.Add(unidad);
            CambiarSeleccion(unidad, true);
        }
    }

    private void ActualizarCanvasDeConstruccion()
    {
        bool hayAldeano = false;

        unidadesSeleccionadas.RemoveAll(unidad => unidad == null); //  limpia referencias destruidas

        foreach (var unidad in unidadesSeleccionadas)
        {
            if (unidad != null && unidad.GetComponent<Aldeano>() != null)
            {
                hayAldeano = true;
                break;
            }
        }

        canvasConstruccion.SetActive(hayAldeano);
    }


    public void Deseleccionar(GameObject unidad)
    {
        CambiarSeleccion(unidad, false);
        unidadesSeleccionadas.Remove(unidad);

    }


}