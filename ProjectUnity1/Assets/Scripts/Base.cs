using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Base : MonoBehaviour, IBuilding
{
    public GameObject menuEdificio;

    [SerializeField] private GameObject visualConstruccion;
    [SerializeField] private GameObject visualConstruido;
    [SerializeField] private GameObject visualDerribado;
    [SerializeField] private Transform puntoConstruccion;

    private bool construido = false;
    public Transform GetPuntoConstruccion() => puntoConstruccion;

    [SerializeField] private float tiempoConstruccion = 10f;
    public float TiempoConstruccion => tiempoConstruccion;

    [SerializeField] private List<CostoEdificio> costos;
    public List<CostoEdificio> Costos => costos;

    void Start()
    {
        BeginConstruction();
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
        GestionRecrsos.Instance.SumarPoblación(3);
    }

    public bool EstáConstruido => construido;

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

    void OnMouseDown()
    {
        menuEdificio.SetActive(!menuEdificio.activeSelf);
    }
}