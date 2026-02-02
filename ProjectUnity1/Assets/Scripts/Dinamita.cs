using System.Collections;
using UnityEngine;

public class Dinamita : MonoBehaviour
{
    private int danio;
    private GameObject emisor;
    private Vector2 puntoInicial;

    [Header("Configuración")]
    [SerializeField] private float distanciaMaxima = 12f;
    [SerializeField] private float tiempoAutoExplosion = 3f;
    [SerializeField] private GameObject prefabExplosionFuego;

    private bool explotado = false;

    public void SetDanio(int cantidad) => danio = cantidad;
    public void SetEmisor(GameObject quienDisparo) => emisor = quienDisparo;

    void Start()
    {
        puntoInicial = transform.position;
        StartCoroutine(ContadorAutoExplosion());

        // Ignorar colisión con el emisor
        if (emisor != null && emisor.TryGetComponent<Collider2D>(out var colEmisor))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), colEmisor);
        }

        Destroy(gameObject, 4f); // Seguridad extra
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, puntoInicial) >= distanciaMaxima && !explotado)
        {
            ExplotarEn(transform.position);
        }
    }


    private IEnumerator ContadorAutoExplosion()
    {
        yield return new WaitForSeconds(tiempoAutoExplosion);
        if (!explotado)
            ExplotarEn(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (explotado || collision.gameObject == emisor)
            return;

        if (collision.TryGetComponent<Sheep>(out _))
            return;

        if (emisor.TryGetComponent<EntidadBase>(out var emisorEntidad) &&
            collision.TryGetComponent<EntidadBase>(out var objetivoEntidad))
        {
            if (!FaccionUtils.SonEnemigos(emisorEntidad.faccion, objetivoEntidad.faccion))
            {
                Debug.Log("No son enemigos. No se aplica daño.");
                return;
            }
        }

        // Guardar posición de impacto
        Vector3 puntoImpacto = collision.ClosestPoint(transform.position);
        ExplotarEn(puntoImpacto);
    }


    private void ExplotarEn(Vector3 posicion)
    {
        if (explotado) return;
        explotado = true;

        if (prefabExplosionFuego != null)
        {
            GameObject fuego = Instantiate(prefabExplosionFuego, posicion, Quaternion.identity);

            if (fuego.TryGetComponent<ExplosionFuego>(out var explosion))
            {
                explosion.Configurar(danio, emisor, 1.5f);
            }
        }

        Destroy(gameObject);
    }
}