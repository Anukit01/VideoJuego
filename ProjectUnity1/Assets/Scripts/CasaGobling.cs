using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasaGobling : EntidadBase
{
    public GameObject vidaVisual;

    [SerializeField] protected GameObject visualConstruido;
    [SerializeField] protected GameObject visualDerribado;


    [SerializeField] private AudioSource fuenteEdificio;
    [SerializeField] private AudioClip clipDerrumbarse;

    protected bool construido = false;

    public int defensa = 5;
    

    protected override void Start()
    {
        vida = 50;
        vidaMaxima = 50;
        defensa = 5;
        construido = true;
        MostrarSolo(visualConstruido);
    }
    private void OnMouseDown()
    {
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);

    }
    public void ActivarVisuales()
    {
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);

    }
    protected void MostrarSolo(GameObject activo)
    {
        visualConstruido.SetActive(activo == visualConstruido);
        visualDerribado.SetActive(activo == visualDerribado);

    }
    public void Derribar()
    {
        MostrarSolo(visualDerribado);
        construido = false;

        ActualizarVidaVisual();       

        Destroy(gameObject, 3f);

    }
    public override void RecibirDanio(int cantidad, GameObject atacante)
    {
        int defensaLocal = 0;
        if (this is CasaGobling unidadDefensora)
            defensaLocal = unidadDefensora.defensa;

        vida -= Mathf.Max(cantidad - defensaLocal, 0);
        ActualizarVidaVisual();
        if (vida <= 0)
        {
            Derribar();
        }
    }
    private void OnDestroy()
    {
        BuildingPlacementManager.Instance?.ActualizarNavMesh();
    }

    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteEdificio == null) return;
        fuenteEdificio.clip = clip;
        fuenteEdificio.loop = true;
        fuenteEdificio.Play();
    }
    public void ReproducirUna(AudioClip clip)
    {
        if (fuenteEdificio == null || clip == null) return;
        fuenteEdificio.Stop();
        fuenteEdificio.clip = clip;
        fuenteEdificio.loop = false;
        fuenteEdificio.spatialBlend = 0.5f; // 2D sound
        fuenteEdificio.PlayOneShot(clip);
    }
    protected override void Morir()
    {
        if (fuenteEdificio != null && clipDerrumbarse != null)
        {
            ReproducirUna(clipDerrumbarse);
        }
        Derribar();  
    }
}
