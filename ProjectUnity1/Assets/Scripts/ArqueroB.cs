using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Arquero : UnidadJugador
{
    [Header("Puntos de disparo por dirección")]
    [SerializeField] private Transform disparoArriba;
    [SerializeField] private Transform disparoAbajo;
    [SerializeField] private Transform disparoLateral;
    [SerializeField] private Transform disparoDiagArriba;
    [SerializeField] private Transform disparoDiagAbajo;

    [Header("Combate")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private float radioDeteccion = 3f;
    [SerializeField] private float tiempoEntreDisparos = 1.5f;

    private float tiempoUltimoDisparo = 0f;
    private Coroutine rutinaAtaque;
    private bool atacando;
    private Transform puntoDisparoActual;

    protected override void Start()
    {
        InicializarVida(80);
        ataque = 15;
        defensa = 5;
        velocidad = 5;
       
        base.Start();
    }

    public override void EjecutarAccion(GameObject objetivo, Vector3 destino)
    {
        StopAllCoroutines();
        ResetearAnimacionesDisparo();

        if (objetivo == null)
        {
            agent.SetDestination(destino);
            return;
        }
        if (objetivo.TryGetComponent<EntidadBase>(out var entidadObjetivo))
        {
            if (!FaccionUtils.SonEnemigos(faccion, entidadObjetivo.faccion))
            {
                Debug.Log(" Objetivo aliado. Cancelando acción de ataque.");
                return;
            }
        }

        if (rutinaAtaque != null)
            StopCoroutine(rutinaAtaque);

        rutinaAtaque = StartCoroutine(CombatirObjetivo(objetivo));
    }

    private IEnumerator CombatirObjetivo(GameObject objetivo)
    {
        if (objetivo == null) yield break;
        if (!objetivo.TryGetComponent<IAtacable>(out var atacable)) yield break;

        atacando = true;

        while (objetivo != null && atacable.EstaVivo())
        {
            float distancia = Vector2.Distance(transform.position, objetivo.transform.position);

            if (distancia > radioDeteccion)
                agent.SetDestination(objetivo.transform.position);
            else
            {
                agent.SetDestination(transform.position);

                if (Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
                {
                    Disparar(objetivo);
                    tiempoUltimoDisparo = Time.time;
                }
            }

            yield return null;
        }

        atacando = false;
        ResetearAnimacionesDisparo();
        rutinaAtaque = null;
    }

    private void Disparar(GameObject objetivo)
    {
        puntoDisparoActual = ElegirPuntoDisparo(transform.position, objetivo.transform.position);
        Vector2 direccion = (objetivo.transform.position - puntoDisparoActual.position).normalized;
        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        ActivarAnimacionDisparo(transform.position, objetivo.transform.position);
        OrientadorVisual orientador = GetComponent<OrientadorVisual>();
        if (orientador != null)
            orientador.GirarVisual(objetivo.transform.position);

        StartCoroutine(DispararConRetardo(objetivo, direccion, angle));
    }

    private IEnumerator DispararConRetardo(GameObject objetivo, Vector2 direccion, float angle)
    {
        yield return new WaitForSeconds(0.2f); // Ajusta según la animación

        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparoActual.position, Quaternion.Euler(0, 0, angle));
        proyectil.GetComponent<Rigidbody2D>().velocity = direccion * 10f;

        if (proyectil.TryGetComponent<Flecha>(out var flecha))
        {
            flecha.SetDanio(ataque);
            flecha.SetEmisor(gameObject);
            Physics2D.IgnoreCollision(flecha.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    private Transform ElegirPuntoDisparo(Vector2 origen, Vector2 destino)
    {
        CalcularDireccionDisparo(origen, destino,
            out bool arriba, out bool abajo, out bool lateral, out bool diagonalArriba, out bool diagonalAbajo);

        if (arriba) return disparoArriba;
        if (abajo) return disparoAbajo;
        if (diagonalArriba) return disparoDiagArriba;
        if (diagonalAbajo) return disparoDiagAbajo;
        return disparoLateral;
    }

    private void ActivarAnimacionDisparo(Vector2 origen, Vector2 destino)
    {
        CalcularDireccionDisparo(origen, destino,
            out bool arriba, out bool abajo, out bool lateral, out bool diagonalArriba, out bool diagonalAbajo);

        ResetearAnimacionesDisparo();

        if (arriba) animator.SetBool("DisparoArriba", true);
        else if (abajo) animator.SetBool("DisparoAbajo", true);
        else if (diagonalArriba) animator.SetBool("DisparoDiagArriba", true);
        else if (diagonalAbajo) animator.SetBool("DisparoDiagAbajo", true);
        else if (lateral) animator.SetBool("DisparoLateral", true);

    }

    private void ResetearAnimacionesDisparo()
    {
        animator.SetBool("DisparoLateral", false);
        animator.SetBool("DisparoAbajo", false);
        animator.SetBool("DisparoArriba", false);
        animator.SetBool("DisparoDiagArriba", false);
        animator.SetBool("DisparoDiagAbajo", false);
    }

    public void CalcularDireccionDisparo(Vector2 origen, Vector2 destino,
        out bool arriba, out bool abajo, out bool lateral,
        out bool diagonalArriba, out bool diagonalAbajo)
    {
        Vector2 dir = (destino - origen).normalized;

        arriba = false;
        abajo = false;
        lateral = false;
        diagonalArriba = false;
        diagonalAbajo = false;

        float absX = Mathf.Abs(dir.x);
        float absY = Mathf.Abs(dir.y);

        if (absX > 0.5f && absY < 0.5f)
        {
            lateral = true;
        }
        else if (absX < 0.5f && dir.y > 0)
        {
            arriba = true;
        }
        else if (absX < 0.5f && dir.y < 0)
        {
            abajo = true;
        }
        else if (dir.y > 0)
        {
            diagonalArriba = true;
        }
        else
        {
            diagonalAbajo = true;
        }
    }
}