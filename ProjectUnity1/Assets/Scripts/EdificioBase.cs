using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EdificioBase : EntidadBase, IBuilding
{

    public int defensa;


    [Header("Visuales")]
    [SerializeField] protected GameObject visualConstruccion;
    [SerializeField] protected GameObject visualConstruido;
    [SerializeField] protected GameObject visualDerribado;
    [SerializeField] protected Transform puntoConstruccion;
    
    [Header("Construcción")]
    [SerializeField] protected float tiempoConstruccion = 5f;

    [Header("Costos")]
    [SerializeField] protected List<CostoEdificio> costos;

    [SerializeField] private AudioSource fuenteEdificio;
    [SerializeField] private AudioClip clipDerrumbarse;
    [SerializeField] private AudioClip clipCompleto;

    protected bool construido = false;

    public bool EstáConstruido => construido;
    public float TiempoConstruccion => tiempoConstruccion;
    public Transform GetPuntoConstruccion() => puntoConstruccion;
    public List<CostoEdificio> Costos => costos;
    

    public void SumarVida(int cantidad)
    {
        vida = Mathf.Min(VidaActual + cantidad, VidaMaxima);

        ActualizarVidaVisual();

        if (VidaActual == VidaMaxima && !construido)
        {
            CompleteConstruction(); // cambia sprite al estado final
                                       // activá navmesh, collider, menú, etc.
        }
    }

    public virtual void BeginConstruction()
    {
        MostrarSolo(visualConstruccion);
        construido = false;
       
    }

    public virtual void CompleteConstruction()
    {
        if (fuenteEdificio != null && clipCompleto != null)
        {
            ReproducirUna(clipCompleto);
        }
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
        
    }

   
    // Permite que los edificios sean destruidos al quedarse sin vida
    protected override void Morir()
    {
        if (fuenteEdificio != null && clipDerrumbarse != null)
        {
            ReproducirUna(clipDerrumbarse);
        }
        Derribar();
        base.Morir();
        Destroy(gameObject, 3f);
        BuildingPlacementManager.Instance?.ActualizarNavMesh();

    }

    public override void RecibirDanio(int cantidad, GameObject atacante)
    {
        int defensaLocal = 0;
        if (this is EdificioBase unidadDefensora)
            defensaLocal = unidadDefensora.defensa;

        vida -= Mathf.Max(cantidad - defensaLocal, 0);
        ActualizarVidaVisual();

        if (vida <= 0) 
            Morir();
    
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
    protected void OnDestroy()
    {
        BuildingPlacementManager.Instance?.ActualizarNavMesh();

    }
}
