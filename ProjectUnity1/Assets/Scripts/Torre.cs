using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torre : MonoBehaviour, IBuilding
{
    public Faccion faccion;

    [Header("Combate")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private float radioDeteccion = 5f;
    [SerializeField] private float tiempoEntreDisparos = 1.5f;
    private float tiempoUltimoDisparo = 0f;

    [Header("Visuales")]
    [SerializeField] private GameObject visualConstruccion;
    [SerializeField] private GameObject visualConstruido;
    [SerializeField] private GameObject visualDerribado;
    [SerializeField] private Transform puntoConstruccion;

    private bool construido = false;
    public bool EstáConstruido => construido;

    [Header("Construcción")]
    [SerializeField] private float tiempoConstruccion = 7f;
    public float TiempoConstruccion => tiempoConstruccion;
    public Transform GetPuntoConstruccion() => puntoConstruccion;

    [Header("Costos")]
    [SerializeField] private List<CostoEdificio> costos;
    public List<CostoEdificio> Costos => costos;

    void Start()
    {
        MostrarSolo(visualConstruccion);
        StartCoroutine(ProcesoConstruccion(() => CompleteConstruction()));
    }

    void Update()
    {
        if (!EstáConstruido || !PuedeDisparar()) return;

        GameObject objetivo = BuscarObjetivo();
        if (objetivo != null)
        {
            Disparar(objetivo);
            tiempoUltimoDisparo = Time.time;
        }
    }

    private bool PuedeDisparar() => Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos;

    private GameObject BuscarObjetivo()
    {
        Collider2D[] posibles = Physics2D.OverlapCircleAll(transform.position, radioDeteccion);
        foreach (var c in posibles)
        {
            if (c.CompareTag("Enemigo"))
                return c.gameObject;
        }
        return null;
    }

    private void Disparar(GameObject objetivo)
    {
        if (proyectilPrefab == null || puntoDisparo == null) return;

        Vector2 direccion = (objetivo.transform.position - puntoDisparo.position).normalized;
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = direccion * 10f;

        if (proyectil.TryGetComponent<Flecha>(out var flecha))
        {
            flecha.SetDanio(10); // podés adaptar el daño si la torre escala
            flecha.SetEmisor(gameObject);
        }
    }

    public void BeginConstruction()
    {
        MostrarSolo(visualConstruccion);
        StartCoroutine(ProcesoConstruccion(() => CompleteConstruction()));
    }

    public void CompleteConstruction()
    {
        MostrarSolo(visualConstruido);
        GestorOrdenVisualCamara.Instance?.ActualizarOrdenes();
        construido = true;
    }

    public void Derribar()
    {
        MostrarSolo(visualDerribado);
        GestorOrdenVisualCamara.Instance?.ActualizarOrdenes();
        construido = false;
    }

    private void MostrarSolo(GameObject activo)
    {
        visualConstruccion.SetActive(activo == visualConstruccion);
        visualConstruido.SetActive(activo == visualConstruido);
        visualDerribado.SetActive(activo == visualDerribado);
    }

    public IEnumerator ProcesoConstruccion(System.Action onConstruccionTerminada)
    {
        yield return new WaitForSeconds(tiempoConstruccion);
        onConstruccionTerminada?.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}