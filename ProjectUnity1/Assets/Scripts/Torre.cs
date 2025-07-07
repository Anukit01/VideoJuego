using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torre : EdificioBase
{
    [Header("Combate")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private float radioDeteccion = 5f;
    [SerializeField] private float tiempoEntreDisparos = 1.5f;
    private float tiempoUltimoDisparo = 0f;

    protected override void Start()
    {
        InicializarVida(100); // o la vida que quieras
        BeginConstruction();
        base.Start();
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
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = direccion * 10f;

        if (proyectil.TryGetComponent<Flecha>(out var flecha))
        {
            flecha.SetDanio(10);
            flecha.SetEmisor(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}
