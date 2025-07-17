using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorreEnemiga : EntidadBase
{
    public GameObject vidaVisual;

    [Header("Combate")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private float radioDeteccion = 5f;
    [SerializeField] private float tiempoEntreDisparos = 1.5f;
    private float tiempoUltimoDisparo = 0f;

    [SerializeField] protected GameObject visualConstruido;
    [SerializeField] protected GameObject visualDerribado;

    [SerializeField] private AudioSource fuenteEdificio;
    [SerializeField] private AudioClip clipFlecha;
    [SerializeField] private AudioClip clipDerrumbarse;

    public int defensa = 12;
    public int ataque = 14;

    protected bool construido = false;
    public bool EstáConstruido => construido;
    protected override void Start()
    {
        vida = 160;
        vidaMaxima = 160;
        ataque = 14;
        defensa = 6;
        construido = true;
        MostrarSolo(visualConstruido);

    }
    private void OnMouseDown()
    {
        if (vidaVisual != null)
            vidaVisual.SetActive(!vidaVisual.activeSelf);

    }
    private void Update()
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
            var entidad = c.GetComponent<EntidadBase>();
            if (entidad != null && entidad.faccion != this.faccion && entidad.faccion != Faccion.Neutral && entidad.EstaVivo())
                return c.gameObject;
        }
        return null;
    }

    private void Disparar(GameObject objetivo)
    {
        if (proyectilPrefab == null || puntoDisparo == null) return;

        Vector2 direccion = (objetivo.transform.position - puntoDisparo.position).normalized;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.Euler(0f, 0f, angulo));
        proyectil.layer = LayerMask.NameToLayer("Proyectiles");
        Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = direccion * 10f;
        if (fuenteEdificio != null && clipFlecha != null)
        {
            fuenteEdificio.PlayOneShot(clipFlecha);
        }
        if (proyectil.TryGetComponent<Flecha>(out var flecha))
        {
            flecha.SetDanio(ataque);
            flecha.SetEmisor(gameObject);
        }
        if (fuenteEdificio != null && fuenteEdificio.isPlaying)
        {
            fuenteEdificio.Stop();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }

    public override void RecibirDanio(int cantidad, GameObject atacante)
    {
        int defensaLocal = 0;
        if (this is TorreEnemiga unidadDefensora)
            defensaLocal = unidadDefensora.defensa;

        vida -= Mathf.Max(cantidad - defensaLocal, 0);
        ActualizarVidaVisual();
        if (vida <= 0)
        {
            Derribar();
        }
    }
    public void Derribar()
    {
        MostrarSolo(visualDerribado);
        construido = false;

        ActualizarVidaVisual();

        Destroy(gameObject, 3f);

    }
    private void OnDestroy()
    {
        BuildingPlacementManager.Instance?.ActualizarNavMesh();
    }
    protected void MostrarSolo(GameObject activo)
    {      
        visualConstruido.SetActive(activo == visualConstruido);
        visualDerribado.SetActive(activo == visualDerribado);

    }
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

}

