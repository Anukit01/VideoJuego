using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EdificioBase : EntidadBase, IBuilding
{
    [Header("Visuales")]
    [SerializeField] protected GameObject visualConstruccion;
    [SerializeField] protected GameObject visualConstruido;
    [SerializeField] protected GameObject visualDerribado;
    [SerializeField] protected Transform puntoConstruccion;

    [Header("Construcción")]
    [SerializeField] protected float tiempoConstruccion = 5f;

    [Header("Costos")]
    [SerializeField] protected List<CostoEdificio> costos;

    protected bool construido = false;

    public bool EstáConstruido => construido;
    public float TiempoConstruccion => tiempoConstruccion;
    public Transform GetPuntoConstruccion() => puntoConstruccion;
    public List<CostoEdificio> Costos => costos;

    public virtual void BeginConstruction()
    {
        MostrarSolo(visualConstruccion);
        StartCoroutine(ProcesoConstruccion(CompleteConstruction));
    }

    public virtual void CompleteConstruction()
    {
        MostrarSolo(visualConstruido);
        construido = true;
        GestorOrdenVisualCamara.Instance?.ActualizarOrdenes();
    }

    public virtual void Derribar()
    {
        MostrarSolo(visualDerribado);
        construido = false;
        GestorOrdenVisualCamara.Instance?.ActualizarOrdenes();
    }

    public IEnumerator ProcesoConstruccion(System.Action cuandoTermina)
    {
        yield return new WaitForSeconds(tiempoConstruccion);
        cuandoTermina?.Invoke();
    }

    protected void MostrarSolo(GameObject activo)
    {
        visualConstruccion.SetActive(activo == visualConstruccion);
        visualConstruido.SetActive(activo == visualConstruido);
        visualDerribado.SetActive(activo == visualDerribado);
        GestorOrdenVisualGlobal.ActualizarOrdenVisualGlobal();
    }

    // Permite que los edificios sean destruidos al quedarse sin vida
    protected override void Morir()
    {
        Derribar();
        base.Morir();
    }
}
