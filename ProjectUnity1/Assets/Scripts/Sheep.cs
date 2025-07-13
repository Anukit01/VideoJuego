using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour, IAtacable

{
    public bool EstaVivo() => vida > 0;
    public int vida = 20;
    public GameObject carnePrefab; // Prefab con script Carne y Collider
    private SeleccionadorDeUnidad seleccionador;
    private Animator animator;
    [SerializeField] private float distanciaHuida = 1.5f;
    [SerializeField] private float velocidadHuida = 2f;
    private bool enHuida = false;
    public bool EnHuida => enHuida;

    [SerializeField] private AudioSource fuenteSheep;
    [SerializeField] private AudioClip clipGolpeada;
    [SerializeField] private AudioClip clipMorir;


    void Start()
    {
        seleccionador = FindObjectOfType<SeleccionadorDeUnidad>();
        animator = GetComponent<Animator>();

    }

    public void RecibirDanio(int cantidad, GameObject atacante)
    {
        vida -= cantidad;
        StartCoroutine(FlashRojo());
        ReproducirUna(clipGolpeada);
        HuirDelGolpe(Camera.main.transform.position);
        if (vida <= 0)
            Morir();
    }
    private void HuirDelGolpe(Vector3 origen)
    {
        if (enHuida) return;

        enHuida = true;
        Vector2 direccion = (transform.position - origen).normalized;
        Vector2 destino = (Vector2)transform.position + direccion * distanciaHuida;

        StartCoroutine(MoverConHuida(destino));
    }
    private IEnumerator FlashRojo()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }


    private void Morir()
    {
        if (fuenteSheep != null && clipMorir != null)
        {
            ReproducirUna(clipMorir);
        }
        GameObject carne = Instantiate(carnePrefab, transform.position, Quaternion.identity); ;

        // Aldeanos seleccionados van autom�ticamente
        foreach (var unidad in FindObjectOfType<SeleccionadorDeUnidad>().unidadesSeleccionadas.ToList())
        {
            if (unidad.TryGetComponent<Aldeano>(out Aldeano aldeano))
                aldeano.EjecutarAccion(carne, carne.transform.position);
        }
        Destroy(gameObject);
    }
    private IEnumerator MoverConHuida(Vector2 destino)
    {
        animator.SetTrigger("Saltar");

        while (Vector2.Distance(transform.position, destino) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destino, velocidadHuida * Time.deltaTime);
            yield return null;
        }

        enHuida = false;
    }
    public void ReproducirLoop(AudioClip clip)
    {
        if (fuenteSheep == null) return;
        fuenteSheep.clip = clip;
        fuenteSheep.loop = true;
        fuenteSheep.Play();
    }
    public void ReproducirUna(AudioClip clip)
    {
        if (fuenteSheep == null || clip == null) return;
        fuenteSheep.Stop();           
        fuenteSheep.clip = clip;
        fuenteSheep.loop = false;
        fuenteSheep.spatialBlend = 0.5f; // 2D sound
        fuenteSheep.PlayOneShot(clip);
    }


}