using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casa : MonoBehaviour, IBuilding
{
    [SerializeField] private GameObject visualConstruccion;
    [SerializeField] private GameObject visualConstruido;
    [SerializeField] private GameObject visualDerribado;
    [SerializeField] private Transform puntoConstruccion;
    private bool construido = false;
    public bool EstáConstruido => construido;

    [SerializeField] private float tiempoConstruccion = 5f;
    public float TiempoConstruccion => tiempoConstruccion;
    public Transform GetPuntoConstruccion() => puntoConstruccion;

    [SerializeField] private List<CostoEdificio> costos;
    public List<CostoEdificio> Costos => costos;

    void Start()
    {
        MostrarSolo(visualConstruccion);
        StartCoroutine(ProcesoConstruccion(() => CompleteConstruction()));
    }

    public void BeginConstruction()
    {
        MostrarSolo(visualConstruccion);
        StartCoroutine(ProcesoConstruccion(() => CompleteConstruction()));
    }

    private void MostrarSolo(GameObject activo)
    {
        visualConstruccion.SetActive(activo == visualConstruccion);
        visualConstruido.SetActive(activo == visualConstruido);
        visualDerribado.SetActive(activo == visualDerribado);
    }

    public void CompleteConstruction()
    {
        MostrarSolo(visualConstruido);
        GestorOrdenVisualCamara.Instance?.ActualizarOrdenes();
        construido = true;
        GestionRecrsos.Instance.SumarPoblación(2);
    }

    public void Derribar()
    {
        MostrarSolo(visualDerribado);
        GestorOrdenVisualCamara.Instance?.ActualizarOrdenes();
        construido = false;
    }

    public IEnumerator ProcesoConstruccion(System.Action onConstruccionTerminada)
    {
        yield return new WaitForSeconds(tiempoConstruccion);
        onConstruccionTerminada?.Invoke();
    }
}