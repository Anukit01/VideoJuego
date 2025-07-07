using System.Collections.Generic;
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
        // Evitar clics sobre la UI
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        // Evitar deseleccionar mientras estás colocando un edificio
        if (BuildingPlacementManager.Instance != null && BuildingPlacementManager.Instance.IsPlacing())
            return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.GetComponent<EntidadBase>() != null) { 

            var entidad = hit.collider.GetComponent<EntidadBase>();

        if (entidad is UnidadJugador)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                AlternarSeleccion(hit.collider.gameObject);
            else
                SeleccionarSolo(hit.collider.gameObject);
        }
        }

    }


    private void ManejarClickDerecho()
    {
        Vector2 destino = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(destino, Vector2.zero);
        GameObject objetivo = hit.collider?.gameObject;

        foreach (var unidad in unidadesSeleccionadas)
        {
            
            if (unidad.TryGetComponent<IAccionContextual>(out var accionable))
            {
                
                if (objetivo != null && objetivo.TryGetComponent<IRecolectable>(out var recurso))
                {
                    accionable.EjecutarAccion(objetivo, recurso.PuntoDeRecoleccion.position);
                }
                // Si hay objetivo e implementa IAtacable, se ejecuta como ataque
                else if (objetivo != null && objetivo.TryGetComponent<IAtacable>(out var atacable))
                {
                    accionable.EjecutarAccion(objetivo, destino);
                }
                else
                {
                   accionable.EjecutarAccion(null, destino);
                }
            }
            else if (unidad.TryGetComponent<Movimiento>(out var movimiento))
            {
                movimiento.MoverA(destino);
            }
        }
        
    }


    private void AlternarSeleccion(GameObject unidad)
    {
        bool agregar = !unidadesSeleccionadas.Contains(unidad);
        CambiarSeleccion(unidad, agregar);

        if (agregar)
            unidadesSeleccionadas.Add(unidad);
        else
            unidadesSeleccionadas.Remove(unidad);
    }

    private void SeleccionarSolo(GameObject unidad)
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

        foreach (var unidad in unidadesSeleccionadas)
        {
            if (unidad.GetComponent<Aldeano>() != null)
            {
                hayAldeano = true;
                break;
            }
        }

        canvasConstruccion.SetActive(hayAldeano);
    }

}