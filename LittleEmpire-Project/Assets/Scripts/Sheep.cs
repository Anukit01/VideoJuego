using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
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
    [SerializeField] private float distanciaHuida = 0.5f;
    [SerializeField] private float velocidadHuida = 1f;
    private bool enHuida = false;
    public bool EnHuida => enHuida;

    public static Vector3 UltimaPosicionDeOvejaMuerta;

    [SerializeField] private AudioSource fuenteSheep;
    [SerializeField] private AudioClip clipGolpeada;


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
              

        if (vida <= 0)
        {
            Morir(); // no hay huida si ya está muerta
            return;
        }

        HuirDelGolpe(atacante.transform.position); // solo si sigue viva
      
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
     
        GameObject carne = Instantiate(carnePrefab, transform.position, Quaternion.identity); ;

        // Aldeanos seleccionados van automáticamente
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