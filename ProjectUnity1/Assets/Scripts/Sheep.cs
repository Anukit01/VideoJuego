using System;
using System.Collections;
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




    void Start()
    {
        seleccionador = FindObjectOfType<SeleccionadorDeUnidad>();
        animator = GetComponent<Animator>();

    }

    public void RecibirDanio(int cantidad)
    {
        vida -= cantidad;
        StartCoroutine(FlashRojo());
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
        GameObject carne = Instantiate(carnePrefab, transform.position, Quaternion.identity); ;

        // Aldeanos seleccionados van automáticamente
        foreach (var unidad in FindObjectOfType<SeleccionadorDeUnidad>().unidadesSeleccionadas)
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

}